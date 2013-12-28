using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;

namespace LINE2D
{
    unsafe class FeatureMap
    {
        public static byte INVALID_QUANTIZED_ORIENTATION = GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS + 1;

        private static byte[] angleQuantizationTable = null;

        #region Quantization table calculation

        static FeatureMap()
        {
            FeatureMap.angleQuantizationTable = CalculateAngleQuantizationTable();
        }

        private static byte[] CalculateAngleQuantizationTable()
        {
            byte[] angleQuantizationTable = new byte[360 + 1/*0..360*/ + 1/*invalid orientation*/];

            for (int angle = 0; angle <= 360; angle++)  
            {
                int directedAngle;

                directedAngle = (int)Math.Round(2f * GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS / 360 * angle);
                directedAngle &= 7;
         
                angleQuantizationTable[angle] = (byte)directedAngle;
            }

            angleQuantizationTable[GradientOrientation.INVALID_ORIENTATION] = INVALID_QUANTIZED_ORIENTATION;

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
                    qOrinetUnfilteredPtr[i] = angleQuantizationTable[angle]; //[0-360] -> [...] -> [0-7] (for mapping see "CalculateAngleQuantizationTable()")
                }

                orientDegImgPtr += imgWidth; //<Gray, int> is always alligned
                qOrinetUnfilteredPtr += qOrinetUnfilteredStride;
            }

            //quantizedUnfilteredOrient.Mul(36).Save("quantizedUnfilteredImg.bmp");
            return quantizedUnfilteredOrient;
        }

        /// <summary>
        /// Take only those orientations that have MINIMAL_NUM_OF_SAME_ORIENTED_PIXELS in 3x3 negborhood.
        /// Perfroms angle transformation into binary form ([0..7] -> [1, 2, 4, 8, ..., 128]) as well.
        /// </summary>
        private static Image<Gray, Byte> RetainImportantQuantizedOrientations(Image<Gray, Byte> qunatizedOrientionImg)
        {
            //debugImg = new Image<Hsv, byte>(orientDegImg.Width, orientDegImg.Height);
            //debugImg = null;
            int qOrinetStride = qunatizedOrientionImg.Stride;
            int qOrinetAllign = qunatizedOrientionImg.Stride - qunatizedOrientionImg.Width;

            byte* qOrinetUnfilteredPtr = (byte*)qunatizedOrientionImg.ImageData + qOrinetStride + 1;

            Image<Gray, Byte> quantizedFilteredOrient = qunatizedOrientionImg.CopyBlank();
            byte* qOrinetFilteredPtr = (byte*)quantizedFilteredOrient.ImageData + qOrinetStride + 1;

 //Debug.Assert(qunatizedOrientionImg.Stride == quantizedFilteredOrient.Stride);

            const int MINIMAL_NUM_OF_SAME_ORIENTED_PIXELS = 4;

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
                        histogram[qOrinetUnfilteredPtr[-1]]++; histogram[qOrinetUnfilteredPtr[0]]++; histogram[qOrinetUnfilteredPtr[+1]]++;
                        histogram[qOrinetUnfilteredPtr[+qOrinetStride - 1]]++; histogram[qOrinetUnfilteredPtr[+qOrinetStride + 0]]++; histogram[qOrinetUnfilteredPtr[+qOrinetStride + 1]]++;

                        int maxBinVotes = 0; byte quantizedAngle = 0;
                        for (byte histBinIdx = 0; histBinIdx < GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS /*discard invalid orientation*/; histBinIdx++)
                        {
                            if (histogram[histBinIdx] > maxBinVotes)
                            {
                                maxBinVotes = histogram[histBinIdx];
                                quantizedAngle = histBinIdx;
                            }
                        }

                        if (maxBinVotes >= MINIMAL_NUM_OF_SAME_ORIENTED_PIXELS)
                            *qOrinetFilteredPtr = (byte)(1 << quantizedAngle); //[1,2,4,8...128] (8 orientations)

                        //*qOrinetFilteredPtr = (byte)(1 << *qOrinetUnfilteredPtr); //[1,2,4,8...128] (8 orientations)
                        //debugImg[j, i] = new Hsv((*qOrinetFilteredPtr-1) * 35, 100, 100);
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

        public static Image<Gray, byte> Caclulate(Image<Gray, int> orientationDegImg, int spreadNeigborhood)
        {
            Image<Gray, Byte> quantizedOrient = FeatureMap.QuantizeOrientations(orientationDegImg);
            Image<Gray, Byte> importantQuantizedOrient = FeatureMap.RetainImportantQuantizedOrientations(quantizedOrient);

            Image<Gray, Byte> sprededQuantizedOrient = importantQuantizedOrient;
            if(spreadNeigborhood > 1)
                sprededQuantizedOrient = FeatureMap.SpreadOrientations(importantQuantizedOrient, spreadNeigborhood);

            return sprededQuantizedOrient;
        }

    }

}
