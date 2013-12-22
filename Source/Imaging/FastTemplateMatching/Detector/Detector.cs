using Accord.Imaging;
using Accord.Math.Geometry;
using LINE2D.QueryImage;
using LINE2D.TemplateMatching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace LINE2D
{
    public static class ImageMatchExtensions
    {
        public static void Draw(this Image<Bgr, Byte> image, Match match, Bgr color, int featureRadius, int thickness)
        {
            int numOfFeatures = match.Template.Features.Length;
            var featureArr = match.Template.Features;

            for (int i = 0; i < numOfFeatures; i++)
            {
                var center = new PointF(featureArr[i].X + match.X, featureArr[i].Y + match.Y);
                image.Draw(new CircleF(center, featureRadius), color, thickness);
            }
        }
    }

    public unsafe class Detector
    {
        public static List<Match> MatchTemplates(LinearMemoryPyramid linPyr, List<TemplatePyramid> templPyrs, int minMatchingPercentage)
        {
            List<Match> matches = new List<Match>();

            /*int numOfTemplates = templPyrs.Count;
            for (int i = 0; i < numOfTemplates; i++)
            {
                List<Match> templateMatches = MatchTemplate(linPyr, templPyrs[i], minMatchingPercentage);
                matches.AddRange(templateMatches);
            }*/

            object syncObj = new object();

            Parallel.ForEach(templPyrs, delegate(TemplatePyramid templPyr)
            {
                List<Match> templateMatches = MatchTemplate(linPyr, templPyr, minMatchingPercentage);
                lock (syncObj)
                {
                    matches.AddRange(templateMatches);
                }
            });

            return matches;
        }

        public static List<Match> MatchTemplate(LinearMemoryPyramid linPyr, TemplatePyramid templPyr, int minMatchingPercentage)
        {
            int numOfPyramids = GlobalParameters.NEGBORHOOD_PER_LEVEL.Length;

            List<Match>[] pyrMatches = new List<Match>[numOfPyramids];
          
            int lowestPyramidIdx = numOfPyramids - 1; //lowestPyramidIdx = 0;
            Maps mapsLowPyr = linPyr.ResponseMaps[lowestPyramidIdx];
            Template templateLowPyr = templPyr.Templates[lowestPyramidIdx];
            
            if (templateLowPyr.Size.Width > mapsLowPyr.ResponseMapSize.Width || //just do matching for templates that can fit into query image
                templateLowPyr.Size.Height > mapsLowPyr.ResponseMapSize.Height)
                return new List<Match>();

            short[] similarityLinMem = CalculateSimilarityMap(templateLowPyr, mapsLowPyr);
            pyrMatches[lowestPyramidIdx] = SearchSimilarityMap(similarityLinMem, new Rectangle(System.Drawing.Point.Empty, mapsLowPyr.OriginalImageSize),
                                                               mapsLowPyr.NeigborhoodSize, templateLowPyr, minMatchingPercentage);

            FilterPartialShownObjects(ref pyrMatches[lowestPyramidIdx], mapsLowPyr.OriginalImageSize);

            for (int pyrLevel = lowestPyramidIdx - 1; pyrLevel >= 0; pyrLevel--)
            {
                Maps maps = linPyr.ResponseMaps[pyrLevel];
                Template template = templPyr.Templates[pyrLevel];
                Size responseMapSize = linPyr.ResponseMaps[pyrLevel].ResponseMapSize;
                pyrMatches[pyrLevel] = new List<Match>();

                int previousNeigborhood = linPyr.ResponseMaps[pyrLevel + 1].NeigborhoodSize;

                for (int candidateIdx = 0; candidateIdx < pyrMatches[pyrLevel+1].Count; candidateIdx++) //for every candidate of previous pyramid level...
                {
                    Match canidate = pyrMatches[pyrLevel + 1][candidateIdx];
                    canidate.X = canidate.X * 2 + 1;
                    canidate.Y = canidate.Y * 2 + 1;
                    canidate.Template = template;

                    Rectangle searchArea = new Rectangle //in originalImageSize coordinate system
                    {
                        X = Math.Max(0, canidate.X - previousNeigborhood),
                        Y = Math.Max(0, canidate.Y - previousNeigborhood),
                        Width = previousNeigborhood * 2, 
                        Height = previousNeigborhood * 2
                    };

                    if (searchArea.Right >= responseMapSize.Width)
                        searchArea.Width = responseMapSize.Width - searchArea.X;

                    if (searchArea.Bottom >= responseMapSize.Height)
                        searchArea.Height = responseMapSize.Height - searchArea.Y;

                    if (template.Size.Width + searchArea.X >= responseMapSize.Width || //search only full visible templates
                         template.Size.Height + searchArea.Y >= responseMapSize.Height)
                        continue;

                    short[] similarityLinMemLocal = CalculateSimilarityMapSimple(template, maps, searchArea);
                    List<Match> foundCandidates = SearchSimilarityMap(similarityLinMemLocal, searchArea, maps.NeigborhoodSize, template, minMatchingPercentage);
                    pyrMatches[pyrLevel].AddRange(foundCandidates);
                }
            }

            return pyrMatches[0]; //matches of the highest pyr level
        }

        private static short[] CalculateSimilarityMap(Template template, Maps maps)
        {
            int maxValidTemplateShifts = maps.MaxValidTemplateShiftsInLinearMemoryLine(template.Size);
            Debug.Assert(maxValidTemplateShifts >= 0);

            int numOfFeatures = template.Features.Length;
            Template.Feature[] featureArray = template.Features;

            short[] destinationLinMemLine = new short[maps.LinearMapSize.Width]; //one line in linearMemoryMap
            byte[] buffer = new byte[maps.LinearMapSize.Width];

            fixed(short* destLinMemPtr = destinationLinMemLine)
            fixed(byte* bufferPtr = buffer)
            {
                int numOfBufferAddingInLoop = 0;

                for (int featureIdx = 0; featureIdx < numOfFeatures; featureIdx++)
                {
                    int numOfElemnentsUntilEndOfLine;
                    byte* linMemPtr = maps.GetLinearMapElement(featureArray[featureIdx], out numOfElemnentsUntilEndOfLine);
                    Debug.Assert(featureArray[featureIdx].X >= 0 && featureArray[featureIdx].Y >= 0);

                    Debug.Assert(maxValidTemplateShifts <= numOfElemnentsUntilEndOfLine && numOfElemnentsUntilEndOfLine >= 0);

                    Maps.LinearMemoryArithmetic.AddByteToByteVector(linMemPtr, bufferPtr, maxValidTemplateShifts);
                    numOfBufferAddingInLoop++;

                    if (numOfBufferAddingInLoop / Maps.LinearMemoryArithmetic.MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE != 0)
                    {
                        numOfBufferAddingInLoop = 0;
                        Maps.LinearMemoryArithmetic.AddByteToShortVector(bufferPtr, destLinMemPtr, maxValidTemplateShifts);
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }

                bool finalAdd = (numOfFeatures % Maps.LinearMemoryArithmetic.MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE != 0) ? true : false;
                if (finalAdd)
                {
                    Maps.LinearMemoryArithmetic.AddByteToShortVector(bufferPtr, destLinMemPtr, maxValidTemplateShifts);
                }
            }

            return destinationLinMemLine;
        }

        private static short[] CalculateSimilarityMap(Template template, Maps maps, Rectangle rect)
        {
            Debug.Assert(rect.Right <= maps.OriginalImageSize.Width && 
                         rect.Bottom <= maps.OriginalImageSize.Height);
            Debug.Assert(template.Size.Width + rect.X < maps.OriginalImageSize.Width &&
                         template.Size.Height + rect.Y < maps.OriginalImageSize.Height);

            int destLinMemWidth = rect.Width / maps.NeigborhoodSize;
            int destLinMemHeight = rect.Height / maps.NeigborhoodSize;
            int destLinMemSize = destLinMemWidth * destLinMemHeight;

            int linMemLineVirtualWidth = maps.LinearMapLineVirtualSize.Width;

            int numOfFeatures = template.Features.Length;
            Template.Feature[] featureArray = template.Features;


            short[] destinationLinMemLine = new short[destLinMemSize];  
            byte[] buffer = new byte[destLinMemSize];

             fixed(short* destLinMemSourcePtr = destinationLinMemLine)
             fixed (byte* bufferSourcePtr = buffer)
             {
                 short* destLinMemPtr = destLinMemSourcePtr;
                 byte* bufferPtr = bufferSourcePtr;

                 int numOfBufferAddingInLoop = 0;

                 for (int featureIdx = 0; featureIdx < numOfFeatures; featureIdx++)
                 {
                     Template.Feature shiftedFeature = featureArray[featureIdx].Clone();
                     shiftedFeature.X += rect.X;
                     shiftedFeature.Y += rect.Y;

                     //if (shiftedFeature.X >= maps.OriginalImageSize.Width || shiftedFeature.Y >= maps.OriginalImageSize.Height)
                     //    continue;

                     int numOfElemnentsUntilEndOfLine;
                     byte* linMemPtr = maps.GetLinearMapElement(shiftedFeature, out numOfElemnentsUntilEndOfLine);

                     for (int row = 0; row < destLinMemHeight; row++)
                     {
                         Maps.LinearMemoryArithmetic.AddByteToByteVector(linMemPtr, bufferPtr, Math.Min(destLinMemWidth, numOfElemnentsUntilEndOfLine));

                         bufferPtr += destLinMemWidth;
                         linMemPtr += linMemLineVirtualWidth;

                         numOfElemnentsUntilEndOfLine -= linMemLineVirtualWidth;
                         if (numOfElemnentsUntilEndOfLine < 0)
                             break;
                     }


                     numOfBufferAddingInLoop++;
                     bufferPtr = (byte*)bufferSourcePtr; //reinit buffer start addr

                     if (numOfBufferAddingInLoop / Maps.LinearMemoryArithmetic.MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE != 0)
                     {
                         numOfBufferAddingInLoop = 0;
                         Maps.LinearMemoryArithmetic.AddByteToShortVector(bufferPtr, destLinMemPtr, destLinMemSize);
                         Array.Clear(buffer, 0, buffer.Length);
                     }
                 }

                 bool finalAdd = (numOfFeatures % Maps.LinearMemoryArithmetic.MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE != 0) ? true : false;
                 if (finalAdd)
                 {
                     Maps.LinearMemoryArithmetic.AddByteToShortVector(bufferPtr, destLinMemPtr, destLinMemSize);
                 }
             }
        
            return destinationLinMemLine;
        }    


        private static List<Match> SearchSimilarityMap(short[] similarityMap, Rectangle similarityMapRect, int mapsNegborhood, Template template, int minMatchingPercentage)
        {
            int numOfFeatures = template.Features.Length;

            List<Match> matches = new List<Match>();

            int destLinMemWidth = similarityMapRect.Width / mapsNegborhood;
            int destLinMemHeight = similarityMapRect.Height / mapsNegborhood;

            float scaleFactor = 100f / (GlobalParameters.MAX_FEATURE_SIMILARITY * numOfFeatures);
            short minMatchingRawScore = (short)Math.Round(minMatchingPercentage * (1 / scaleFactor));

            fixed (short* similarityMapPtr = similarityMap)
            {
                short* destLinMemLinePtr = similarityMapPtr;

                for (int row = 0; row < destLinMemHeight; row++)
                {
                    for (int col = 0; col < destLinMemWidth; col++)
                    {
                        if (destLinMemLinePtr[col] >= minMatchingRawScore)
                        {
                            int offset = mapsNegborhood / 2;

                            int x = col * mapsNegborhood + similarityMapRect.X + offset;
                            int y = row * mapsNegborhood + similarityMapRect.Y + offset;

                            float score = destLinMemLinePtr[col] * scaleFactor;
                            var candidate = new Match { X = x, Y = y, Score = score, Template = template };
                            matches.Add(candidate);
                        }
                    }

                    destLinMemLinePtr += destLinMemWidth;
                }
            }

            return matches;
        }

        private static short[] CalculateSimilarityMapSimple(Template template, Maps maps, Rectangle rect)
        {
            Debug.Assert(rect.Right <= maps.OriginalImageSize.Width &&
                         rect.Bottom <= maps.OriginalImageSize.Height);
            Debug.Assert(template.Size.Width + rect.X < maps.OriginalImageSize.Width &&
                         template.Size.Height + rect.Y < maps.OriginalImageSize.Height);

            int destLinMemWidth = rect.Width / maps.NeigborhoodSize;
            int destLinMemHeight = rect.Height / maps.NeigborhoodSize;
            int destLinMemSize = destLinMemWidth * destLinMemHeight;

            int linMemLineVirtualWidth = maps.LinearMapLineVirtualSize.Width;
         
            int numOfFeatures = template.Features.Length;
            Template.Feature[] featureArray = template.Features;


            short[] destinationLinMemLine = new short[destLinMemSize];

            fixed(short* destLinMemSourcePtr = destinationLinMemLine)
            {
                short* destLinMemPtr = destLinMemSourcePtr;

                for (int featureIdx = 0; featureIdx < numOfFeatures; featureIdx++)
                {
                    Template.Feature shiftedFeature = featureArray[featureIdx].Clone();
                    shiftedFeature.X += rect.X;
                    shiftedFeature.Y += rect.Y;

                    //if (shiftedFeature.X >= maps.OriginalImageSize.Width || shiftedFeature.Y >= maps.OriginalImageSize.Height)
                    //    continue;

                    destLinMemPtr = (short*)destLinMemSourcePtr;

                    int numOfElemnentsUntilEndOfLine;
                    byte* linMemPtr = maps.GetLinearMapElement(shiftedFeature, out numOfElemnentsUntilEndOfLine);

                    for (int row = 0; row < destLinMemHeight; row++)
                    {
                        AddMemoryByteShort_Managed(linMemPtr, destLinMemPtr, Math.Min(destLinMemWidth, numOfElemnentsUntilEndOfLine));

                        destLinMemPtr += destLinMemWidth;
                        linMemPtr += linMemLineVirtualWidth;

                        numOfElemnentsUntilEndOfLine -= linMemLineVirtualWidth;
                        if (numOfElemnentsUntilEndOfLine < 0)
                            break;
                    }
                }
            }

            return destinationLinMemLine;
        }

        private static void AddMemoryByteShort_Managed(byte* srcAddr, short* dstAddr, int numOfElemsToAdd)
        {
            int i = 0;
            while (i < numOfElemsToAdd)
            {
                dstAddr[i] += srcAddr[i];
                i++;
            }
        }

        private static void FilterPartialShownObjects(ref List<Match> matches, Size originalImageSize)
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
