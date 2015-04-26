#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// Linearized memory maps calculation.
    /// <para>
    /// See <a href="http://cvlabwww.epfl.ch/~lepetit/papers/hinterstoisser_pami11.pdf" /> Section 2.5. for details.
    /// </para>
    /// </summary>
    public unsafe class LinearizedMaps
    {
        /// <summary>
        /// Linear map stride alignment.
        /// <para>It's value is 0 because linear map should be represented as continuous vector (without stride alignment).</para>
        /// </summary>
        public const int MAP_STRIDE_ALLIGNMENT = 0; 

        /// <summary>
        /// Pre-calculated similarities between feature angle (binary representations) and all angle combinations. 
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
            byte similarity = (byte)(GlobalParameters.MAX_FEATURE_SIMILARITY - System.Math.Min(numOfLeftShifts, numOfRightShifts));

            return similarity;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets linear maps. The map is selected according to an angle and then to an neighborhood.
        /// </summary>
        public Image<Gray<byte>>[][,] LinearMaps { get; private set; }

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
        /// <param name="neigborhood">Spread neighborhood size.</param>
        public LinearizedMaps(Gray<int>[,] orientationDegImg, int neigborhood)
        {
            this.NeigborhoodSize = neigborhood;
            this.ImageSize = orientationDegImg.Size();
            
            this.LinearMapSize = new Size(orientationDegImg.Width() / neigborhood, orientationDegImg.Height() / neigborhood);
            this.ImageValidSize = new Size(this.LinearMapSize.Width * neigborhood, this.LinearMapSize.Height * neigborhood);

            this.LinearMaps = calculate(orientationDegImg);
        }

        private Image<Gray<byte>>[][,] calculate(Gray<int>[,] orientationDegImg)
        {
            var linearMaps = new Image<Gray<byte>>[GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS][,];

            Gray<byte>[,] sprededQuantizedOrient = FeatureMap.Calculate(orientationDegImg, this.NeigborhoodSize);
            for (int orient = 0; orient < GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS; orient++)
            {
                var responseMap = computeResponseMap(sprededQuantizedOrient, orient);
                linearMaps[orient] = linearizeResponseMap(responseMap);
                //responseMap.Save("C:/RM_" + orient + ".bmp");
            }

            return linearMaps;
        }

        private Gray<byte>[,] computeResponseMap(Gray<byte>[,] sprededQuantizedImage, int orientationIndex)
        {
            int width = this.ImageValidSize.Width;
            int height = this.ImageValidSize.Height;
            var responseMap = new Gray<byte>[height, width];

            using (var uSprededQuantizedImage = sprededQuantizedImage.Lock())
            {
                int srcStride = uSprededQuantizedImage.Stride;
                byte* srcPtr = (byte*)uSprededQuantizedImage.ImageData;
           
                using (var uResponseMap = responseMap.Lock())
                {
                    int dstStride = uResponseMap.Stride;
                    byte* dstPtr = (byte*)uResponseMap.ImageData;

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
                }
            }

            return responseMap;
        }

        #region Response map linearization

        private Image<Gray<byte>>[,] linearizeResponseMap(Gray<byte>[,] responseMap)
        {
            var linearizedMaps = new Image<Gray<byte>>[NeigborhoodSize, NeigborhoodSize];

            //Outer two for loops iterate over top-left T^2 starting pixels
            for (int rowNeigbor = 0; rowNeigbor < this.NeigborhoodSize; rowNeigbor++)
            {
                for (int colNeigbor = 0; colNeigbor < this.NeigborhoodSize; colNeigbor++)
                {
                    var linearMap = new Gray<byte>[this.LinearMapSize.Height, this.LinearMapSize.Width];
                    calculateLinearMapForNeighbour(responseMap, rowNeigbor, colNeigbor, linearMap);
                    linearizedMaps[rowNeigbor, colNeigbor] = linearMap.Lock();
                }
            }

            return linearizedMaps;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void calculateLinearMapForNeighbour(Gray<byte>[,] responseMap, int neigbourRow, int neighbourCol,
                                                           Gray<byte>[,] linearMap)
        {
            using (var uResponseMap = responseMap.Lock())
            using (var uLinearMap = linearMap.Lock())
            {
                int neigborhood = this.NeigborhoodSize;

                byte* linMapPtr = (byte*)uLinearMap.ImageData;
                int linMapStride = uLinearMap.Stride;

                int width = uResponseMap.Width;
                int height = uResponseMap.Height;
                int stride = uResponseMap.Stride;
                byte* responseMapPtr = (byte*)uResponseMap.GetData(neigbourRow);

                //Two loops copy every T-th pixel into the linear memory
                for (int r = neigbourRow; r < height; r += neigborhood)
                {
                    int linMapIdx = 0;
                    for (int c = neighbourCol; c < width; c += neigborhood)
                    {
                        linMapPtr[linMapIdx] = responseMapPtr[c];
                        linMapIdx++;
                    }

                    responseMapPtr += stride * neigborhood; //skip neighborhood rows
                    linMapPtr += linMapStride;
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the linearized map by suing provided image position and angle index.
        /// </summary>
        /// <param name="position">Image position.</param>
        /// <param name="angleIndex">Quantized angle index.</param>
        /// <param name="mapPoint">Corresponding position in the linearized map.</param>
        /// <returns>Linearized map.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Image<Gray<byte>> GetMapElement(Point position, int angleIndex, out Point mapPoint)
        {
            //find corresponding linearized map for neighbor
            int gridX = position.X % this.NeigborhoodSize;
            int gridY = position.Y % this.NeigborhoodSize;

            //find corresponding position
            int elemX = position.X / this.NeigborhoodSize;
            int elemY = position.Y / this.NeigborhoodSize;

            //get corresponding map and (row, col)
            var map = this.LinearMaps[angleIndex][gridY, gridX]; 

            //corresponding point in returned map 
            mapPoint = new Point(elemX, elemY);

            return map;
        }
    }

}
