using Accord.Imaging;
using LINE2D.TemplateMatching;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace LINE2D.QueryImage
{
    public unsafe class Maps
    {
        public static byte[,] SimilarityAngleTable = null; //[NUM_OF_ORIENTATIONS, 256 (all possible angle combinations for an source orientation)]

        static Maps()
        {
            SimilarityAngleTable = ComputeSimilarityAngleTable();
        }

        private static byte[,] ComputeSimilarityAngleTable()
        {
            byte[,] similarityTable = new byte[GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS, 256];

            for (int angleIdx = 0; angleIdx < GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS; angleIdx++)
            {
                for (int destAngles = 0; destAngles < 256; destAngles++)
                {
                    similarityTable[angleIdx, destAngles] = CalculateSimilarity((byte)destAngles, (byte)(1 << angleIdx));
                }
            }

            return similarityTable;
        }

        private static byte CalculateSimilarity(byte destAngles, byte sourceAngle)
        {
            //the closest 1 on the left...
            byte numOfLeftShifts = 0;

            while ( ((destAngles << numOfLeftShifts) & sourceAngle) == 0 && numOfLeftShifts < GlobalParameters.MAX_FEATURE_SIMILARITY)
            {
                numOfLeftShifts++;
            }

            //the closest 1 on the right...
            byte numOfRightShifts = 0;

            while (((destAngles >> numOfRightShifts) & sourceAngle) == 0 && numOfRightShifts < GlobalParameters.MAX_FEATURE_SIMILARITY)
            {
                numOfRightShifts++;
            }

            //the less shifts, the bigger similarity
            byte similarity = (byte)(GlobalParameters.MAX_FEATURE_SIMILARITY - Math.Min(numOfLeftShifts, numOfRightShifts));

            return similarity;
        }


        public Image<Gray, Byte> ImportantQuantizedOrientImage;

        public Image<Gray, Byte>[] ResponseMaps { get; private set; }
        public Size ResponseMapSize; //ResponseMap.Size != OriginalImage.Size !!!
        public Size OriginalImageSize;

        public Image<Gray, Byte>[] LinearMaps { get; private set; }
        public Size LinearMapSize;
        public Size LinearMapLineVirtualSize; //the actual size is ResponseMapsSize.Width, which could be represented like this...

        public int NeigborhoodSize;

        private Maps(Size originalImageSize, Image<Gray, Byte>[] responseMaps, Image<Gray, Byte>[] linearMaps, int neigborhood, Image<Gray, Byte> importantQuantizedOrientImage, Image<Gray, int> magnitude)
        {
            this.ResponseMaps = responseMaps;
            this.LinearMaps = linearMaps;
            this.NeigborhoodSize = neigborhood;

            this.ResponseMapSize = ResponseMaps[0].Size;
            this.OriginalImageSize = originalImageSize;

            this.LinearMapSize = LinearMaps[0].Size;
            this.LinearMapLineVirtualSize = new Size(OriginalImageSize.Width / NeigborhoodSize, OriginalImageSize.Height / NeigborhoodSize);

            this.ImportantQuantizedOrientImage = importantQuantizedOrientImage;
            this.magnitude = magnitude;
        }

        public Image<Gray, int> magnitude;

        public static Maps Calculate(Image<Bgr, Byte> sourceImage, int neigborhood)
        {
            Image<Gray, int>  magnitude, orientation;
            ColorGradient.ComputeColor(sourceImage, out magnitude, out orientation);

            Image<Gray, Byte> quantizedOrient = ColorGradient.QuantizeOrientations(orientation);
            Image<Gray, Byte> importantQuantizedOrient = ColorGradient.RetainImportantQuantizedOrientations(quantizedOrient, magnitude, GlobalParameters.MIN_GRADIENT_THRESHOLD);
            Image<Gray, Byte> sprededQuantizedOrient = ColorGradient.SpreadOrientations(importantQuantizedOrient, neigborhood);

            Image<Gray, Byte>[] responseMaps = ComputeResponseMaps(sprededQuantizedOrient, neigborhood);
            Image<Gray, Byte>[] linearMaps = LinearizeResponseMaps(responseMaps, neigborhood);

            return new Maps(sourceImage.Size, responseMaps, linearMaps, neigborhood, importantQuantizedOrient, magnitude);
        }

        private static Image<Gray, Byte>[] ComputeResponseMaps(Image<Gray, Byte> sprededQuantizedImage, int neigborhood)
        {
            Image<Gray, Byte>[] responseMaps = new Image<Gray, byte>[GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS];

            int imgWidth = (sprededQuantizedImage.Width / neigborhood) * neigborhood;
            int imgHeight = (sprededQuantizedImage.Height / neigborhood) * neigborhood;
            //int imgWidth = sprededQuantizedImage.Width;
            //int imgHeight = sprededQuantizedImage.Height;

            for (int i = 0; i < GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS; i++)
            {
                responseMaps[i] = new Image<Gray,byte>(imgWidth, imgHeight);
            }

            int responseMapImgStride = responseMaps[0].Stride;
            int sprededQuantizedImgStride = sprededQuantizedImage.Stride;

            for (int orient = 0; orient < GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS; orient++)
            {
                byte* srcPtr = (byte*)sprededQuantizedImage.ImageData;
                byte* destPtr = (byte*)responseMaps[orient].ImageData;

                //za sliku
                fixed (byte* angleTablePtr = &SimilarityAngleTable[0, 0])
                {
                    byte* orientPtr = angleTablePtr + orient * SimilarityAngleTable.GetLength(1);

                    for (int row = 0; row < imgHeight; row++)
                    {
                        for (int col = 0; col < imgWidth; col++)
                        {
                            //destPtr[col] = SimilarityAngleTable[orient, srcPtr[col]];
                            destPtr[col] = orientPtr[srcPtr[col]]; //faster way
                        }

                        srcPtr += sprededQuantizedImgStride;
                        destPtr += responseMapImgStride;
                    }
                }          
            }

            return responseMaps;
        }

        public static Image<Gray, Byte> LinearizeResponseMap(Image<Gray, Byte> responseMap, int neigborhood)
        {
            byte* responseMapPtr = (byte*)responseMap.ImageData;
            int responseMapValidWidth = (responseMap.Width / neigborhood) * neigborhood;
            int responseMapValidHeight = (responseMap.Height / neigborhood) * neigborhood; 

            int memoryWidth = (responseMap.Width / neigborhood) * (responseMap.Height / neigborhood);
            int memoryHeight = neigborhood * neigborhood;

            Image<Gray, Byte> linearizedMap = new Image<Gray, byte>(memoryWidth, memoryHeight);
            byte* linMapPtr = (byte*)linearizedMap.ImageData;

            int responseMapStride = responseMap.Stride;
            int linMememoryStride = linearizedMap.Stride;

            //Outer two for loops iterate over top-left T^2 starting pixels
            for (int rowNeigbor = 0; rowNeigbor < neigborhood; rowNeigbor++)
            {
                for (int colNeigbor = 0; colNeigbor < neigborhood; colNeigbor++)
                {
                    //Inner two loops copy every T-th pixel into the linear memory
                    byte* responseMapRowPtr = responseMapPtr; //drugi pokazivač zbog kopiranja podataka 
                    int linMapIdx = 0;

                    for (int rowStart = rowNeigbor; rowStart < responseMapValidHeight; rowStart += neigborhood)
                    {
                        for (int colStart = colNeigbor; colStart < responseMapValidWidth; colStart += neigborhood)
                        {
                            linMapPtr[linMapIdx] = responseMapRowPtr[colStart];
                            linMapIdx++;
                        }
                        responseMapRowPtr += responseMapStride * neigborhood; //preskoči NEGBORHOOD redova
                    }

                    linMapPtr += linMememoryStride; //za svaki element susjedstva novi red
                }

                responseMapPtr += responseMapStride; //prati redove susjedstva
            }

            return linearizedMap;
        }

        public static Image<Gray, Byte>[] LinearizeResponseMaps(Image<Gray, Byte>[] responseMaps, int neigboorhod)
        {
            Image<Gray, Byte>[] linearizedMaps = new Image<Gray, byte>[responseMaps.Length];

            for (int i = 0; i < linearizedMaps.Length; i++)
            {
                linearizedMaps[i] = LinearizeResponseMap(responseMaps[i], neigboorhod);
            }

            return linearizedMaps;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* GetLinearMapElement(Template.Feature feature, out int numOfElemsUntilEndOfLine)
        {
            //pronađi odgovrajući redak
            int gridX = feature.X % this.NeigborhoodSize;
            int gridY = feature.Y % this.NeigborhoodSize;

            int lineIndex = gridY * this.NeigborhoodSize + gridX;

            //pronađi element u redu
            int elemX = feature.X / this.NeigborhoodSize;
            int elemY = feature.Y / this.NeigborhoodSize;

            int elemIndex = elemY * (ResponseMapSize.Width / this.NeigborhoodSize) + elemX;

    
            Image<Gray, Byte> linearMemory = this.LinearMaps[feature.AngleLabel];
            byte* linePtr = (byte*)linearMemory.ImageData + lineIndex * linearMemory.Stride;
            byte* elemPtr = linePtr + elemIndex;

            numOfElemsUntilEndOfLine = this.LinearMapSize.Width - elemIndex;

            return elemPtr;
        }

        public int MaxValidTemplateShiftsInLinearMemoryLine(Size templateSize)
        {
            int imgW = this.ResponseMapSize.Width / this.NeigborhoodSize; //LinearMemoryLineVirtualSize
            int imgH = this.ResponseMapSize.Height / this.NeigborhoodSize;

            int tmplW = (templateSize.Width - 1) / this.NeigborhoodSize + 1;
            int tmplH = (templateSize.Height - 1) / this.NeigborhoodSize + 1;

            int spanX = imgW - tmplW;
            int spanY = imgH - tmplH;

            int numOfValidTmplPos = spanY * imgW + spanX + 1;

            return numOfValidTmplPos;
        }

        public static class LinearMemoryArithmetic
        {
            [SuppressUnmanagedCodeSecurity]
            [DllImport("SIMDArrayInstructions.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void AddByteToByteVector(byte* srcAddr, byte* dstAddr, int numOfElemsToAdd);

            [SuppressUnmanagedCodeSecurity]
            [DllImport("SIMDArrayInstructions.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void AddByteToShortVector(byte* srcAddr, short* dstAddr, int numOfElemsToAdd);

            public static int MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE = Byte.MaxValue / GlobalParameters.MAX_FEATURE_SIMILARITY;
        }
    }
}
