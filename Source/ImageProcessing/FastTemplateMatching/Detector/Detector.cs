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

using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// Contains extensions for matching templates against <see cref="LinearizedMapPyramid"/> and <see cref="LinearizedMaps"/>.
    /// <remarks>See <a href="http://cvlabwww.epfl.ch/~lepetit/papers/hinterstoisser_pami11.pdf"/> for details.</remarks>
    /// </summary>
    public static unsafe class LinearizedMemoryDetectorExtensions
    {
        #region TemplatePyrmaid matching (public)

        /// <summary>
        /// Matches the provided pyramids of templates against the linearized memory maps pyramid.  
        /// </summary>
        /// <param name="linPyr">Linearized memory pyramid.</param>
        /// <param name="templPyrs">Pyramids of templates.</param>
        /// <param name="minMatchingPercentage">Minimum matching percentage [0..100].</param>
        /// <param name="inParallel">True to match each template pyramid in parallel, sequentially otherwise.</param>
        /// <returns>List of found matches.</returns>
        public static List<Match> MatchTemplates(this LinearizedMapPyramid linPyr, IEnumerable<ITemplatePyramid> templPyrs, int minMatchingPercentage = 85, bool inParallel = true)
        {
            List<Match> matches = new List<Match>();

            if (inParallel)
            {
                object syncObj = new object();

                Parallel.ForEach(templPyrs, (templPyr) => 
                {
                    List<Match> templateMatches = MatchTemplate(linPyr, templPyr, minMatchingPercentage);
                    lock (syncObj) matches.AddRange(templateMatches);
                });
            }
            else
            {
                foreach (var templPyr in templPyrs)
                {
                    List<Match> templateMatches = MatchTemplate(linPyr, templPyr, minMatchingPercentage);
                    matches.AddRange(templateMatches);
                }
            }

            return matches;
        }

        /// <summary>
        /// Matches the provided template pyramid against the linearized memory maps pyramid.  
        /// </summary>
        /// <param name="linPyr">Linearized memory pyramid.</param>
        /// <param name="templPyr">Template pyramid.</param>
        /// <param name="minMatchingPercentage">Minimum matching percentage [0..100].</param>
        /// <returns>List of found matches.</returns>
        public static List<Match> MatchTemplate(this LinearizedMapPyramid linPyr, ITemplatePyramid templPyr, int minMatchingPercentage = 85)
        {
            if (linPyr.PyramidalMaps.Length != templPyr.Templates.Length)
                throw new Exception("Number of pyramids in linear pyramid must match the number of templates in template pyramid!" + "\n" + 
                                    "Check if the number of neighborhood per level is the same as the number of features per level for template!");

            List<Match>[] pyrMatches = new List<Match>[linPyr.PyramidalMaps.Length];

            //match at the lowest level
            int lowestLevelIdx = linPyr.PyramidalMaps.Length - 1; 
            var searchArea = new Rectangle(new Point(), linPyr.PyramidalMaps[lowestLevelIdx].ImageSize); //search whole image
            pyrMatches[lowestLevelIdx] = matchTemplate(linPyr.PyramidalMaps[lowestLevelIdx], templPyr.Templates[lowestLevelIdx], searchArea, minMatchingPercentage, true);

            //refine matches
            for (int pyrLevel = (lowestLevelIdx - 1); pyrLevel >= 0; pyrLevel--)
            {
                LinearizedMaps maps = linPyr.PyramidalMaps[pyrLevel];
                ITemplate template = templPyr.Templates[pyrLevel];
                Size imageValidSize = maps.ImageValidSize;
                pyrMatches[pyrLevel] = new List<Match>();

                int previousNeigborhood = linPyr.PyramidalMaps[pyrLevel + 1].NeigborhoodSize;

                for (int candidateIdx = 0; candidateIdx < pyrMatches[pyrLevel+1].Count; candidateIdx++) //for every candidate of previous pyramid level...
                {
                    //translate match to lower pyrmaid level
                    Match canidate = pyrMatches[pyrLevel + 1][candidateIdx];
                    canidate.X = canidate.X * 2 + 1;
                    canidate.Y = canidate.Y * 2 + 1;
                    canidate.Template = template;

                    //translate search area to lower pyramid level
                    searchArea = new Rectangle //in originalImageSize coordinate system
                    {
                        X = System.Math.Max(0, canidate.X - previousNeigborhood),
                        Y = System.Math.Max(0, canidate.Y - previousNeigborhood),
                        Width = previousNeigborhood * 2, 
                        Height = previousNeigborhood * 2
                    };
                    searchArea = searchArea.Intersect(imageValidSize);

                    var foundCandidates = matchTemplate(linPyr.PyramidalMaps[pyrLevel], template, searchArea, minMatchingPercentage, pyrLevel != 0 /*filter partial object for all levels except for the original one*/);
                    pyrMatches[pyrLevel].AddRange(foundCandidates);
                }
            }

            return pyrMatches[0]; //matches of the highest pyr level
        }

        #endregion

        #region Template matching (public)

        /// <summary>
        /// Matches the provided templates against the linear memory maps.
        /// </summary>
        /// <param name="linMaps">Linear maps.</param>
        /// <param name="templates">Collections of templates.</param>
        /// <param name="minMatchingPercentage">Minimum matching percentage [0..100].</param>
        /// <param name="inParallel">True to match each template in parallel, sequentially otherwise.</param>
        /// <returns>List of found matches.</returns>
        public static List<Match> MatchTemplates(this LinearizedMaps linMaps, IEnumerable<ITemplate> templates, int minMatchingPercentage = 85, bool inParallel = true)
        {
            var searchArea = new Rectangle(new Point(), linMaps.ImageSize);

            List<Match> matches = new List<Match>();

            if (inParallel)
            {
                object syncObj = new object();

                Parallel.ForEach(templates, (template) =>
                {
                    List<Match> templateMatches = matchTemplate(linMaps, template, searchArea, minMatchingPercentage);
                    lock (syncObj) matches.AddRange(templateMatches);
                });
            }
            else
            {
                foreach (var template in templates)
                {
                    List<Match> templateMatches = matchTemplate(linMaps, template, searchArea, minMatchingPercentage);
                    matches.AddRange(templateMatches);
                }
            }

            return matches;
        }

        /// <summary>
        /// Matches the provided template against the linear memory maps.
        /// </summary>
        /// <param name="linMaps">Linear maps.</param>
        /// <param name="template">Template.</param>
        /// <param name="searchArea">Search area in the image.</param>
        /// <param name="minMatchingPercentage">Minimum matching percentage [0..100].</param>
        /// <returns>List of found matches.</returns>
        public static List<Match> MatchTemplate(this LinearizedMaps linMaps, ITemplate template, Rectangle searchArea, int minMatchingPercentage = 85)
        {
            if (searchArea.IntersectionPercent(new Rectangle(new Point(), linMaps.ImageSize)) < 1)
            {
                throw new Exception("Search area must be within image size!");
            }

            return matchTemplate(linMaps, template, searchArea, minMatchingPercentage);
        }

        /// <summary>
        /// Matches the provided template against the linear memory maps.
        /// </summary>
        /// <param name="linMaps">Linear maps.</param>
        /// <param name="template">Template.</param>
        /// <param name="minMatchingPercentage">Minimum matching percentage [0..100].</param>
        /// <returns>List of found matches.</returns>
        public static List<Match> MatchTemplate(this LinearizedMaps linMaps, ITemplate template, int minMatchingPercentage)
        {
            var searchArea = new Rectangle(new Point(), linMaps.ImageSize);

            return matchTemplate(linMaps, template, searchArea, minMatchingPercentage);
        }

        #endregion

        #region Match template core

        private static List<Match> matchTemplate(this LinearizedMaps linMaps, ITemplate template, Rectangle searchArea, int minMatchingPercentage, bool filterPartialObjects = true)
        {
            //just do matching for templates that can fit into query image
            if (template.Size.Width > linMaps.ImageValidSize.Width ||
                template.Size.Height > linMaps.ImageValidSize.Height)
                return new List<Match>();

            var similarityMap = calculateSimilarityMap(template, linMaps, searchArea);

            float rawScoreScale = 100f / (GlobalParameters.MAX_FEATURE_SIMILARITY * template.Features.Length);
            short minMatchingRawScore = (short)System.Math.Round(minMatchingPercentage * (1 / rawScoreScale));

            List<short> rawScores;
            var foundMatchPoints = searchSimilarityMap(similarityMap, minMatchingRawScore, out rawScores);

            var offset = new Point(searchArea.X, searchArea.Y);
            var foundCandidates = createMatches(template, linMaps.NeigborhoodSize, foundMatchPoints, offset, rawScores, rawScoreScale);

            filterPartialShownObjects(ref foundCandidates, linMaps.ImageSize);

            return foundCandidates;
        }

        private static Gray<short>[,] calculateSimilarityMap(ITemplate template, LinearizedMaps maps, Rectangle searchArea)
        {
            Debug.Assert(searchArea.Right <= maps.ImageSize.Width && 
                         searchArea.Bottom <= maps.ImageSize.Height);
            Debug.Assert(template.Size.Width + searchArea.X < maps.ImageSize.Width &&
                         template.Size.Height + searchArea.Y < maps.ImageSize.Height);

            int width = searchArea.Width / maps.NeigborhoodSize;
            int height = searchArea.Height / maps.NeigborhoodSize;

            Gray<short>[,] similarityMap = new Gray<short>[height, width]; //performance penalty (alloc, dealloc)!!!
            Gray<byte>[,] buffer = new Gray<byte>[height, width];

            using (var uSimilarityMap = similarityMap.Lock())
            using(var uBuffer = buffer.Lock())
            {
                int nAddsInBuffer = 0;
                foreach (var feature in template.Features)
                {
                    var position = new Point(feature.X + searchArea.X, feature.Y + searchArea.Y); //shifted position

                    Point mapPoint;
                    var neighbourMap = maps.GetMapElement(position, feature.AngleIndex, out mapPoint);

                    neighbourMap.AddTo(uBuffer, mapPoint);
                    nAddsInBuffer++;

                    if (nAddsInBuffer / GlobalParameters.MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE != 0)
                    {
                        uBuffer.AddTo(uSimilarityMap);
                        buffer.Clear(); //clear buffer

                        nAddsInBuffer = 0;
                    }
                }

                bool finalAdd = (template.Features.Length % GlobalParameters.MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE != 0) ? true : false;
                if (finalAdd)
                {
                    uBuffer.AddTo(uSimilarityMap);
                }
            }

            return similarityMap;
        }    

        private static List<Point> searchSimilarityMap(Gray<short>[,] similarityMap, int minValue, out List<short> values)
        {
            List<Point> positions = new List<Point>();
            values = new List<short>();

            using (var uSimilarityMap = similarityMap.Lock())
            {
                int width = uSimilarityMap.Width;
                int height = uSimilarityMap.Height;
                int stride = uSimilarityMap.Stride; //stride should be == width * sizeof(short) (see linearized maps)
                short* similarityMapPtr = (short*)uSimilarityMap.ImageData;

                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (similarityMapPtr[col] >= minValue)
                        {
                            positions.Add(new Point(col, row));
                            values.Add(similarityMapPtr[col]);
                        }
                    }

                    similarityMapPtr = (short*)((byte*)similarityMapPtr + stride);
                }
            }

            return positions;
        }

        private static List<Match> createMatches(ITemplate template, int neigborhood, List<Point> mapPositions, Point offset, List<short> rawScores, float rawScoreScale)
        {
            var matches = new List<Match>();
            int allignment = neigborhood / 2;

            for (int i = 0; i < mapPositions.Count; i++)
            {
                var match = new Match 
                {
                    X = mapPositions[i].X * neigborhood + allignment + offset.X, 
                    Y = mapPositions[i].Y * neigborhood + allignment + offset.Y,
                    Score = rawScores[i] * rawScoreScale, 
                    Template = template 
                };

                matches.Add(match);
            }

            return matches;
        }

        private static void filterPartialShownObjects(ref List<Match> matches, Size originalImageSize)
        {
            List<Match> filteredMatches = new List<Match>();

            foreach (Match m in matches)
            {
                Rectangle mRect = new Rectangle(m.X, m.Y, m.Template.Size.Width, m.Template.Size.Height);
                if (!(mRect.Right > originalImageSize.Width))
                    filteredMatches.Add(m);
            }

            matches = filteredMatches;
        }

        #endregion
    }
}
