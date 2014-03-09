using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace LINE2D
{
    /// <summary>
    /// Linearized memory maps calcuation.
    /// </summary>
    public unsafe class LinearizedMaps: IDisposable
    {
        /// <summary>
        /// Linear map stride allignment.
        /// <para>It's value is 0 because linear map should be represented as continous vector (without stride allignment).</para>
        /// </summary>
        public const int MAP_STRIDE_ALLIGNMENT = 0; 

        /// <summary>
        /// Pre-calculated simmilarites between feature angle (binary representations) and all angle combinations. 
        /// <para>[NUM_OF_ORIENTATIONS, 256 (all possible angle combinations for an source orientation)]</para>
        /// </summary>
        public static readonly byte[,] SimilarityAngleTable = null; 

        #region Similarity angle table pre-calculation

        static LinearizedMaps()
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

        #endregion

        #region Properties

        /// <summary>
        /// Gets linear maps.
        /// </summary>
        public Image<Gray, Byte>[][,] LinearMaps { get; private set; }

        /// <summary>
        /// Gets neigborhood size which is used to caclulate spreaded orientation image (FeatureMap)
        /// </summary>
        public int NeigborhoodSize { get; private set; }

        /// <summary>
        /// Gets original image size from which the response map is caclulated.
        /// </summary>
        public Size ImageSize { get; private set; }
        /// <summary>
        /// Getsvalid image size.
        /// <para>Valid size is: (imgWidth / neigborhood * neigborhood,  imgWidth / neigborhood * neigborhood) - integer divisions are used.</para>
        /// </summary>
        public Size ImageValidSize { get; private set; }

        /// <summary>
        /// Gets linear map size.
        /// <para>Linear size is: (imgWidth / neigborhood, imgHeight / neigborhood) - integer divisions are used.</para>
        /// </summary>
        public Size LinearMapSize { get; private set; } 

        #endregion

        /// <summary>
        /// Creates linear response maps.
        /// </summary>
        /// <param name="orientationDegImg">Orientation image (in degrees).</param>
        /// <param name="neigborhood">Spread neigborhood size.</param>
        public LinearizedMaps(Image<Gray, int> orientationDegImg, int neigborhood)
        {
            this.NeigborhoodSize = neigborhood;
            this.ImageSize = orientationDegImg.Size;
            
            this.LinearMapSize = new Size(orientationDegImg.Width / neigborhood, orientationDegImg.Height / neigborhood);
            this.ImageValidSize = new Size(this.LinearMapSize.Width * neigborhood, this.LinearMapSize.Height * neigborhood);

            this.LinearMaps = calculate(orientationDegImg);
        }

        private Image<Gray, Byte>[][,] calculate(Image<Gray, int> orientationDegImg)
        {
            Image<Gray, Byte>[][,] linearMaps = new Image<Gray, byte>[GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS][,];

            using (Image<Gray, Byte> sprededQuantizedOrient = FeatureMap.Caclulate(orientationDegImg, this.NeigborhoodSize))
            {
                for (int orient = 0; orient < GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS; orient++)
                {
                    using (var responseMap = computeResponseMap(sprededQuantizedOrient, this.NeigborhoodSize, orient))
                    {
                        linearMaps[orient] = linearizeResponseMap(responseMap);
                        //responseMap.Save("C:/RM_" + orient + ".bmp");
                    }
                }
            }

            return linearMaps;
        }

        private Image<Gray, byte> computeResponseMap(Image<Gray, Byte> sprededQuantizedImage, int neigborhood, int orientationIndex)
        {
            int width = this.ImageValidSize.Width;
            int height = this.ImageValidSize.Height;
            
            int srcStride = sprededQuantizedImage.Stride;
            byte* srcPtr = (byte*)sprededQuantizedImage.ImageData;

            var responseMap = new Image<Gray, byte>(width, height);
            int dstStride = responseMap.Stride;
            byte* dstPtr = (byte*)responseMap.ImageData;

            //za sliku
            fixed (byte* angleTablePtr = &SimilarityAngleTable[0, 0])
            {
                byte* orientPtr = angleTablePtr + orientationIndex * SimilarityAngleTable.GetLength(1);

                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        //destPtr[col] = SimilarityAngleTable[orient, srcPtr[col]];
                        dstPtr[col] = orientPtr[srcPtr[col]]; //faster way
                    }

                    srcPtr += srcStride;
                    dstPtr += dstStride;
                }
            }

            return responseMap;
        }

        #region Response map linearization

        private Image<Gray, Byte>[,] linearizeResponseMap(Image<Gray, Byte> responseMap)
        {
            Image<Gray, Byte>[,] linearizedMaps = new Image<Gray, byte>[NeigborhoodSize, NeigborhoodSize];

            //Outer two for loops iterate over top-left T^2 starting pixels
            for (int rowNeigbor = 0; rowNeigbor < this.NeigborhoodSize; rowNeigbor++)
            {
                for (int colNeigbor = 0; colNeigbor < this.NeigborhoodSize; colNeigbor++)
                {
                    linearizedMaps[rowNeigbor, colNeigbor] = new Image<Gray, byte>(this.LinearMapSize, MAP_STRIDE_ALLIGNMENT);
                    calculateLinearMapForNeighbour(responseMap, rowNeigbor, colNeigbor, linearizedMaps[rowNeigbor, colNeigbor]);
                }
            }

            return linearizedMaps;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void calculateLinearMapForNeighbour(Image<Gray, byte> responseMap, int neigbourRow, int neighbourCol,
                                                           Image<Gray, byte> linearMap)
        {
            Debug.Assert(linearMap.Width == linearMap.Stride); //because linear map should be represented as continous vector

            int neigborhood = this.NeigborhoodSize;

            byte* linMapPtr = (byte*)linearMap.ImageData;
            int linMapStride = linearMap.Stride;

            int width = responseMap.Width;
            int height = responseMap.Height;
            int stride = responseMap.Stride;
            byte* responseMapPtr = (byte*)responseMap.GetData(neigbourRow);

            //Two loops copy every T-th pixel into the linear memory
            for (int r = neigbourRow; r < height; r += neigborhood)
            {
                int linMapIdx = 0;
                for (int c = neighbourCol; c < width; c += neigborhood)
                {
                    linMapPtr[linMapIdx] = responseMapPtr[c];
                    linMapIdx++;
                }

                responseMapPtr += stride * neigborhood; //skip neigborhood rows
                linMapPtr += linMapStride;
            }
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Image<Gray, byte> GetMapElement(Point position, int angleIndex, out Point mapPoint)
        {
            //find corresponding linearized map for neighbour
            int gridX = position.X % this.NeigborhoodSize;
            int gridY = position.Y % this.NeigborhoodSize;

            //find corresponsing position
            int elemX = position.X / this.NeigborhoodSize;
            int elemY = position.Y / this.NeigborhoodSize;

            //get corresponding map and (row, col)
            var map = this.LinearMaps[angleIndex][gridY, gridX]; 

            //corresponding point in returned map 
            mapPoint = new Point(elemX, elemY);

            return map;
        }

        public void Dispose()
        {
            for (int orientationIdx = 0; orientationIdx < LinearMaps.Length; orientationIdx++)
            {
                for (int r = 0; r < this.NeigborhoodSize; r++)
                {
                    for (int c = 0; c < this.NeigborhoodSize; c++)
                    {
                        LinearMaps[orientationIdx][r, c].Dispose();
                    }
                }
            }
        }
    }

}
