using System;
using Accord.Extensions.Imaging;
using System.Security;
using System.Runtime.ExceptionServices;

namespace GOCO
{
    /// <summary>
    /// Orientation feature map.
    /// <para>Feature map creation has 3 stages:</para>
    /// <para>     1) Orientation quantization: [0..360] => [0..NUM_OF_QUNATIZED_ORIENTATIONS].</para>
    /// <para>     2) Filtering quantized orientations (neighborhood must be the same oriented) 
    ///               and representing them in a binary form: [0..NUM_OF_QUNATIZED_ORIENTATIONS] => [1, 2, 4, 8, 16...]</para>
    /// <para>     3) Spreading binary represented orientations to local neighborhood. This is a trade-off between accuracy and the noise resistance.</para>
    /// </summary>
    /// <remarks>
    /// See: <a href="http://cvlabwww.epfl.ch/~lepetit/papers/hinterstoisser_pami11.pdf" />
    /// </remarks>
    public unsafe static class FeatureMap
    {
        /// <summary>
        /// Number of quantized orientations. Needed for feature extraction.
        /// <para>Maximum value is 8 (number of bits in byte). 3rd i 4th quadrant are mapped to 2nd and 1st</para>
        /// <para>Although this number can be lower than 8 precision can be lost, but the preprocessing stage can be speeded-up (lower number of response maps).</para>
        /// </summary>
        const int NUM_OF_QUNATIZED_ORIENTATIONS = 8; 

        /// <summary>
        /// Marker for invalid orientation (magnitude too small)
        /// </summary>
        public const int INVALID_ORIENTATION = 360 + 1; //used where magnitude to small; 360+1 => look quantization table

        /// <summary>
        /// Marker for invalid quantized orientation (magnitude too small)
        /// </summary>
        public static byte INVALID_QUANTIZED_ORIENTATION = NUM_OF_QUNATIZED_ORIENTATIONS + 1;

        /// <summary>
        /// Angle quantization lookup table. It transforms [0..360] => [0..NUM_OF_QUNATIZED_ORIENTATIONS-1].
        /// <para>For input value INVALID_ORIENTATION INVALID_QUANTIZED_ORIENTATION will be returned. 
        /// This special value is needed during feature map building to discard pixels which have small magnitude.</para>
        /// </summary>
        public static readonly byte[] AngleQuantizationTable = null;

        #region Quantization table calculation

        static FeatureMap()
        {
            FeatureMap.AngleQuantizationTable = CalculateAngleQuantizationTable();
        }

        private static byte[] CalculateAngleQuantizationTable()
        {
            byte[] angleQuantizationTable = new byte[360 + 1/*0..360*/ + 1/*invalid orientation*/];

            for (int angle = 0; angle <= 360; angle++)  
            {
                int directedAngle;

                directedAngle = (int)System.Math.Round(2f * NUM_OF_QUNATIZED_ORIENTATIONS / 360 * angle);
                directedAngle &= 7;
         
                angleQuantizationTable[angle] = (byte)directedAngle;
            }

            angleQuantizationTable[INVALID_ORIENTATION] = INVALID_QUANTIZED_ORIENTATION;

            return angleQuantizationTable;
        }

        #endregion

        #region Orientation quantization and filtering

        private static Image<Gray, Byte> QuantizeOrientations(Image<Gray, int> orientDegImg)
        {
            int* orientDegImgPtr = (int*)orientDegImg.ImageData;

            Image<Gray, Byte> quantizedUnfilteredOrient = new Image<Gray, byte>(orientDegImg.Width, orientDegImg.Height);
            byte* qOrinetUnfilteredPtr = (byte*)quantizedUnfilteredOrient.ImageData;
            int qOrinetUnfilteredStride = quantizedUnfilteredOrient.Stride;

            int imgWidth = orientDegImg.Width;
            int imgHeight = orientDegImg.Height;

            for (int j = 0; j < imgHeight; j++)
            {
                for (int i = 0; i < imgWidth; i++)
                {
                    int angle = orientDegImgPtr[i];
                    qOrinetUnfilteredPtr[i] = AngleQuantizationTable[angle]; //[0-360] -> [...] -> [0-7] (for mapping see "CalculateAngleQuantizationTable()")
                }

                orientDegImgPtr += imgWidth; //<Gray, int> is always alligned
                qOrinetUnfilteredPtr += qOrinetUnfilteredStride;
            }

            return quantizedUnfilteredOrient;
        }

        /// <summary>
        /// Take only those orientations that have MINIMAL_NUM_OF_SAME_ORIENTED_PIXELS in 3x3 negborhood.
        /// Performs angle transformation into binary form ([0..7] -> [1, 2, 4, 8, ..., 128]) as well.
        /// </summary>
        /// <param name="qunatizedOrientionImg">Quantized orientation image where angles are represented by lables [0..GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS] (invalid orientation label included).</param>
        /// <param name="minSameOrientations">Minimal number of same orientations for 3x3 neigborhood. The range is: [0..9] (3x3 neigborhood).</param>
        private static Image<Gray, Byte> RetainImportantQuantizedOrientations(Image<Gray, Byte> qunatizedOrientionImg, int minSameOrientations)
        {
            if (minSameOrientations < 0 || minSameOrientations > 9 /*3x3 neigborhood*/)
                throw new Exception("Minimal number of same orientations should be in: [0..9].");

            //debugImg = new Image<Hsv, byte>(orientDegImg.Width, orientDegImg.Height);
            //debugImg = null;
            int qOrinetStride = qunatizedOrientionImg.Stride;
            int qOrinetAllign = qunatizedOrientionImg.Stride - qunatizedOrientionImg.Width;

            byte* qOrinetUnfilteredPtr = (byte*)qunatizedOrientionImg.ImageData + qOrinetStride + 1;

            Image<Gray, Byte> quantizedFilteredOrient = qunatizedOrientionImg.CopyBlank();
            byte* qOrinetFilteredPtr = (byte*)quantizedFilteredOrient.ImageData + qOrinetStride + 1;

            int imgWidth = qunatizedOrientionImg.Width;
            int imgHeight = qunatizedOrientionImg.Height;

            for (int j = 1; j < imgHeight - 1; j++)
            {
                for (int i = 1; i < imgWidth - 1; i++)
                {
                    if (*qOrinetUnfilteredPtr != INVALID_QUANTIZED_ORIENTATION)
                    {
                        byte[] histogram = new byte[INVALID_QUANTIZED_ORIENTATION + 1]; //gleda se susjedstvo 3x3

                        histogram[qOrinetUnfilteredPtr[-qOrinetStride - 1]]++; histogram[qOrinetUnfilteredPtr[-qOrinetStride + 0]]++; histogram[qOrinetUnfilteredPtr[-qOrinetStride + 1]]++;
                        histogram[qOrinetUnfilteredPtr[-1]]++;                 histogram[qOrinetUnfilteredPtr[0]]++;                  histogram[qOrinetUnfilteredPtr[+1]]++;
                        histogram[qOrinetUnfilteredPtr[+qOrinetStride - 1]]++; histogram[qOrinetUnfilteredPtr[+qOrinetStride + 0]]++; histogram[qOrinetUnfilteredPtr[+qOrinetStride + 1]]++;

                        int maxBinVotes = 0; byte quantizedAngle = 0;
                        for (byte histBinIdx = 0; histBinIdx < NUM_OF_QUNATIZED_ORIENTATIONS /*discard invalid orientation*/; histBinIdx++)
                        {
                            if (histogram[histBinIdx] > maxBinVotes)
                            {
                                maxBinVotes = histogram[histBinIdx];
                                quantizedAngle = histBinIdx;
                            }
                        }

                        if (maxBinVotes >= minSameOrientations)
                            *qOrinetFilteredPtr = (byte)(1 << quantizedAngle); //[1,2,4,8...128] (8 orientations)
                    }

                    qOrinetUnfilteredPtr++;
                    qOrinetFilteredPtr++;
                }

                qOrinetUnfilteredPtr += 1 + qOrinetAllign + 1;
                qOrinetFilteredPtr += 1 + qOrinetAllign + 1; //preskoči zadnji piksel, poravnanje, i početni piksel
            }

            //magnitudeImg.Save("magnitude.bmp");
            //quantizedFilteredOrient.Save("quantizedImg.bmp");
            return quantizedFilteredOrient;
        }

        #endregion

        #region Orientation spreading

        private static Image<Gray, Byte> SpreadOrientations(Image<Gray, Byte> quantizedOrientationImage, int neghborhood)
        {
            byte* srcImgPtr = (byte*)quantizedOrientationImage.ImageData;
            int imgStride = quantizedOrientationImage.Stride;

            Image<Gray, Byte> destImg = quantizedOrientationImage.CopyBlank();
            byte* destImgPtr = (byte*)destImg.ImageData;

            int imgHeight = destImg.Height;
            int imgWidth = destImg.Width;

            for (int row = 0; row < neghborhood; row++)
            {
                int subImageHeight = imgHeight - row;
                for (int col = 0; col < neghborhood; col++)
                {
                    OrImageBits(&srcImgPtr[col], destImgPtr,
                                imgStride,
                                imgWidth - col, subImageHeight);
                }

                srcImgPtr += imgStride;
            }

            return destImg;
        }

        private static void OrImageBits(byte* srcAddr, byte* dstAddr, int imgStride, int subImageWidth, int subImageHeight)
        {
            int imgAllignLongWidth = subImageWidth - (subImageWidth % sizeof(long));

            for (int row = 0; row < subImageHeight; row++)
            {
                int column = 0;

                while (column < imgAllignLongWidth)
                {
                    *(long*)(dstAddr + column) |= *(long*)(srcAddr + column);
                    column += sizeof(long);
                }

                while (column < subImageWidth)
                {
                    dstAddr[column] |= srcAddr[column];
                    column++;
                }

                srcAddr += imgStride;
                dstAddr += imgStride;
            }
        }

        #endregion

        /// <summary>
        /// Calculates features map. First orientations are quantized, the stable ones are selected and then they are spread.
        /// </summary>
        /// <param name="orientationDegImg">Orientation map. Each location represents angle in degrees [0..360].</param>
        /// <param name="spreadNeigborhood">Spreading neighborhood. If 1 no spreading is done.</param>
        /// <param name="minSameOrientations">Minimal number of the same orientations in [3 x 3] neighborhood to proclaim the orientation stable.</param>
        /// <returns>Feature map.</returns>
        [HandleProcessCorruptedStateExceptions] 
        [SecurityCritical]
        public static Image<Gray, byte> Calculate(Image<Gray, int> orientationDegImg, int spreadNeigborhood, int minSameOrientations = 4)
        {
            Image<Gray, Byte> quantizedOrient = FeatureMap.QuantizeOrientations(orientationDegImg);

            Image<Gray, Byte> importantQuantizedOrient = null;
            try
            {
                importantQuantizedOrient = FeatureMap.RetainImportantQuantizedOrientations(quantizedOrient, minSameOrientations);
            }
            catch (Exception)
            {
                return null;
            }

            Image<Gray, Byte> sprededQuantizedOrient = importantQuantizedOrient;
            if(spreadNeigborhood > 1)
                sprededQuantizedOrient = FeatureMap.SpreadOrientations(importantQuantizedOrient, spreadNeigborhood);

            return sprededQuantizedOrient;
        }

    }

}
