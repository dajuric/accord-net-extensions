using Accord.Imaging;
using Accord.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using Accord.Core;

namespace LINE2D
{
    public static class ImageMatchExtensions
    {
        public static void Draw(this Image<Bgr, Byte> image, Match match, Bgr color, int featureRadius, int thickness)
        {
            var circles = match.Template.Features.Select(x => new CircleF
                                                                {
                                                                    X = x.X + match.X,
                                                                    Y = x.Y + match.Y,
                                                                    Radius = featureRadius
                                                                });

            image.Draw(circles, color, thickness);
        }
    }

    public unsafe class Detector
    {
        public static List<Match> MatchTemplates(LinearizedMapPyramid linPyr, IEnumerable<ITemplatePyramid> templPyrs, int minMatchingPercentage, bool inParallel = true)
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

        public static List<Match> MatchTemplate(LinearizedMapPyramid linPyr, ITemplatePyramid templPyr, int minMatchingPercentage)
        {
            List<Match>[] pyrMatches = new List<Match>[GlobalParameters.NEGBORHOOD_PER_LEVEL.Length];

            //match at the lowest level
            int lowestPyramidIdx = GlobalParameters.NEGBORHOOD_PER_LEVEL.Length - 1; //lowestPyramidIdx = 0;
            var searchArea = new Rectangle(System.Drawing.Point.Empty, linPyr.PyramidalMaps[lowestPyramidIdx].ImageSize); //search whole image
            pyrMatches[lowestPyramidIdx] = matchTemplate(linPyr.PyramidalMaps[lowestPyramidIdx], templPyr.Templates[lowestPyramidIdx], searchArea, minMatchingPercentage);

            //refine matches
            for (int pyrLevel = lowestPyramidIdx - 1; pyrLevel >= 0; pyrLevel--)
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
                        X = Math.Max(0, canidate.X - previousNeigborhood),
                        Y = Math.Max(0, canidate.Y - previousNeigborhood),
                        Width = previousNeigborhood * 2, 
                        Height = previousNeigborhood * 2
                    };
                    searchArea = searchArea.Intersect(imageValidSize);

                    var foundCandidates = matchTemplate(linPyr.PyramidalMaps[pyrLevel], template, searchArea, minMatchingPercentage);
                    pyrMatches[pyrLevel].AddRange(foundCandidates);
                }
            }

            return pyrMatches[0]; //matches of the highest pyr level
        }

        #region Match template core

        private static List<Match> matchTemplate(LinearizedMaps linMaps, ITemplate template, Rectangle searchArea, int minMatchingPercentage)
        {
            //just do matching for templates that can fit into query image
            if (template.Size.Width > linMaps.ImageValidSize.Width ||
                template.Size.Height > linMaps.ImageValidSize.Height)
                return new List<Match>();

            var similarityMap = calculateSimilarityMap(template, linMaps, searchArea);

            float rawScoreScale = 100f / (GlobalParameters.MAX_FEATURE_SIMILARITY * template.Features.Length);
            short minMatchingRawScore = (short)Math.Round(minMatchingPercentage * (1 / rawScoreScale));

            List<short> rawScores;
            var foundMatchPoints = searchSimilarityMap(similarityMap, minMatchingRawScore, out rawScores);

            var offset = new Point(searchArea.X, searchArea.Y);
            var foundCandidates = createMatches(template, linMaps.NeigborhoodSize, foundMatchPoints, offset, rawScores, rawScoreScale);

            if (linMaps.NeigborhoodSize == GlobalParameters.NEGBORHOOD_PER_LEVEL.Last() /*only the lowest level*/)
                filterPartialShownObjects(ref foundCandidates, linMaps.ImageSize);

            return foundCandidates;
        }

        private static Image<Gray, short> calculateSimilarityMap(ITemplate template, LinearizedMaps maps, Rectangle searchArea)
        {
            Debug.Assert(searchArea.Right <= maps.ImageSize.Width && 
                         searchArea.Bottom <= maps.ImageSize.Height);
            Debug.Assert(template.Size.Width + searchArea.X < maps.ImageSize.Width &&
                         template.Size.Height + searchArea.Y < maps.ImageSize.Height);

            int width = searchArea.Width / maps.NeigborhoodSize;
            int height = searchArea.Height / maps.NeigborhoodSize;
            int size = width * height; //stride == width

            Image<Gray, short> similarityMap = new Image<Gray, short>(width, height, LinearizedMaps.MAP_STRIDE_ALLIGNMENT); //performance penalty (alloc, dealloc)!!!

            using (var buffer = new Image<Gray, byte>(width, height, LinearizedMaps.MAP_STRIDE_ALLIGNMENT)) //performance penalty (alloc, dealloc)!!!
            {
                int nBufferAddings = 0;

                foreach (var feature in template.Features)
                {
                    var position = new Point(feature.X + searchArea.X, feature.Y + searchArea.Y); //shifted position

                    Point mapPoint;
                    var neighbourMap = maps.GetMapElement(position, feature.AngleIndex, out mapPoint);

                    neighbourMap.AddTo(buffer, mapPoint);
                    nBufferAddings++;

                    if (nBufferAddings / SIMDArithemtics.MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE != 0)
                    {
                        buffer.AddTo(similarityMap);
                        buffer.Clear();

                        nBufferAddings = 0;
                    }
                }

                bool finalAdd = (template.Features.Length % SIMDArithemtics.MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE != 0) ? true : false;
                if (finalAdd)
                {
                    buffer.AddTo(similarityMap);
                }
            }

            return similarityMap;
        }    

        private static List<Point> searchSimilarityMap(Image<Gray, short> similarityMap, int minValue, out List<short> values)
        {
            List<Point> positions = new List<Point>();
            values = new List<short>();

            int width = similarityMap.Width;
            int height = similarityMap.Height;
            int stride = similarityMap.Stride; //stride should be == width * sizeof(short) (see linearized maps)
            short* similarityMapPtr = (short*)similarityMap.ImageData;

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

        #endregion

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

    }
}
