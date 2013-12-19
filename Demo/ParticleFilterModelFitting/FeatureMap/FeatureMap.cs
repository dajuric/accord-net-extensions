using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using System;
using System.Runtime.CompilerServices;

namespace ParticleFilterModelFitting
{
    public unsafe static class FeatureMap
    {
        const int NUM_OF_QUNATIZED_ORIENTATIONS = 8;
        const int MIN_GRADIENT_THRESHOLD = 45;
        const int SPREAD_SIZE = 5;

        static byte[] angleQuantizationTable = null;

        #region Initialization

        static FeatureMap()
        {
            FeatureMap.angleQuantizationTable = CalculateAngleQuantizationTable();
        }

        private static byte[] CalculateAngleQuantizationTable()
        {
            byte[] angleQuantizationTable = new byte[360 + 1];

            for (int angle = 0; angle <= 360; angle++)  
            {
                int directedAngle;

                directedAngle = (int)Math.Round(2f * NUM_OF_QUNATIZED_ORIENTATIONS / 360 * angle);
                directedAngle &= 7;
         
                angleQuantizationTable[angle] = (byte)directedAngle;
            }

            return angleQuantizationTable;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte QuantizeOrientation(int angleDeg) //angleDeg [0..360]
        {
            var idx = angleQuantizationTable[angleDeg];
            return (byte)(1 << idx); //[1,2,4,8...128] (8 orientations)
        }

        #endregion

        #region Orientation Calculation

        public static Image<Gray, Byte> QuantizeOrientations(Image<Gray, int> orientDegImg)
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
        /// Take only those orientations that have MINIMAL_NUM_OF_SAME_ORIENTED_PIXELS in 3x3 negborhood
        /// </summary>
        public static Image<Gray, Byte> RetainImportantQuantizedOrientations(Image<Gray, Byte> qunatizedOrientionImg,  Image<Gray, int> magnitudeImg, int minValidMagnitude)
        {
            int* magPtr = (int*)magnitudeImg.ImageData + magnitudeImg.Width + 1;


            //debugImg = new Image<Hsv, byte>(orientDegImg.Width, orientDegImg.Height);
            //debugImg = null;
            int qOrinetStride = qunatizedOrientionImg.Stride;
            int qOrinetAllign = qunatizedOrientionImg.Stride - qunatizedOrientionImg.Width;

            byte* qOrinetUnfilteredPtr = (byte*)qunatizedOrientionImg.ImageData + qOrinetStride + 1;

            Image<Gray, Byte> quantizedFilteredOrient = qunatizedOrientionImg.CopyBlank();
            byte* qOrinetFilteredPtr = (byte*)quantizedFilteredOrient.ImageData + qOrinetStride + 1;

 //Debug.Assert(qunatizedOrientionImg.MIplImage.widthStep == quantizedFilteredOrient.MIplImage.widthStep);

            const int MINIMAL_NUM_OF_SAME_ORIENTED_PIXELS = 5;

            int imgWidth = qunatizedOrientionImg.Width;
            int imgHeight = qunatizedOrientionImg.Height;

            for (int j = 1; j < imgHeight - 1; j++)
            {
                for (int i = 1; i < imgWidth - 1; i++)
                {
                    if (*magPtr > 0)
                    {
                        byte[] histogram = new byte[NUM_OF_QUNATIZED_ORIENTATIONS]; //gleda se susjedstvo 3x3

                        histogram[qOrinetUnfilteredPtr[-qOrinetStride - 1]]++; histogram[qOrinetUnfilteredPtr[-qOrinetStride + 0]]++; histogram[qOrinetUnfilteredPtr[-qOrinetStride + 1]]++;
                        histogram[qOrinetUnfilteredPtr[-1]]++; histogram[qOrinetUnfilteredPtr[0]]++; histogram[qOrinetUnfilteredPtr[+1]]++;
                        histogram[qOrinetUnfilteredPtr[+qOrinetStride - 1]]++; histogram[qOrinetUnfilteredPtr[+qOrinetStride + 0]]++; histogram[qOrinetUnfilteredPtr[+qOrinetStride + 1]]++;

                        int maxBinVotes = 0; byte quantizedAngle = 0;
                        for (byte histBinIdx = 0; histBinIdx < NUM_OF_QUNATIZED_ORIENTATIONS; histBinIdx++)
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

                    magPtr++;
                    qOrinetUnfilteredPtr++;
                    qOrinetFilteredPtr++;
                }

                magPtr += 1 + 1;
                qOrinetUnfilteredPtr += 1 + qOrinetAllign + 1;
                qOrinetFilteredPtr += 1 + qOrinetAllign + 1; //preskoči zadnji piksel, poravnanje, i početni piksel
            }

            //magnitudeImg.Save("magnitude.bmp");
            //quantizedFilteredOrient.Save("quantizedImg.bmp");
            return quantizedFilteredOrient;
        }

        public static void ComputeGradient(Image<Gray, short> frame, out Image<Gray, int> magImg, out Image<Gray, int> orientImg)
        {
            Image<Gray, short> dx = frame.Sobel(1, 0, 3);
            Image<Gray, short> dy = frame.Sobel(0, 1, 3);

            short* dxPtr = (short*)dx.ImageData;
            short* dyPtr = (short*)dy.ImageData;
            int dxyImgAllignStride = dx.Stride / sizeof(short) - dx.Width;

            magImg = new Image<Gray, int>(frame.Size);
            int* magImgPtr = (int*)magImg.ImageData;

            orientImg = new Image<Gray, int>(frame.Size);
            int* orientImgPtr = (int*)orientImg.ImageData;

            int imgWidth = frame.Width;
            int imgHeight = frame.Height;

            for (int j = 0; j < imgHeight; j++)
            {
                for (int i = 0; i < imgWidth; i++)
                {
                    int mag = dxPtr[0] * dxPtr[0] + dyPtr[0] * dyPtr[0];
                    *magImgPtr = Math.Min(255, (int)Math.Sqrt(mag));

                    //*orientImgPtr = (int)(Math.Atan2(*dyPtr, *dxPtr) * 180 / Math.PI);
                    *orientImgPtr = MathExtensions.Atan2Aprox(*dyPtr, *dxPtr);
                    /*if (*orientImgPtr < 0)
                        *orientImgPtr += 360;*/ //ne treba ?

                    dxPtr += 1; dyPtr += 1;
                    magImgPtr += 1; orientImgPtr += 1;
                }

                dxPtr += dxyImgAllignStride;
                dyPtr += dxyImgAllignStride;
            }
        }

        #endregion

        #region Orientation spreading

        public static Image<Gray, Byte> SpreadOrientations(Image<Gray, Byte> quantizedOrientationImage, int neghborhood)
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

        public static Image<Gray, byte> Compute(Image<Bgr, byte> image)
        {
            Image<Gray, int> magnitude, orientation;
            ComputeGradient(image.Convert<Gray, short>(), out magnitude, out orientation);

            Image<Gray, Byte> quantizedOrient = QuantizeOrientations(orientation);
            Image<Gray, Byte> importantQuantizedOrient = RetainImportantQuantizedOrientations(quantizedOrient, magnitude, MIN_GRADIENT_THRESHOLD);
            Image<Gray, Byte> sprededQuantizedOrient = SpreadOrientations(importantQuantizedOrient, SPREAD_SIZE);
            return sprededQuantizedOrient;
        }
    }
}
