using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Range = AForge.IntRange;
using RangeF = AForge.Range;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Math.Geometry
{
    /// <summary>
    /// Contour extensions
    /// </summary>
    public static class ContourExtensions
    {
        /// <summary>
        /// Gets path length between specified points.
        /// </summary>
        /// <param name="pA">Start point.</param>
        /// <param name="pB">End point.</param>
        /// <param name="cumulativePathLength">Contour cumulative path length.</param>
        /// <returns>Distance from start to end point (by following contour points).</returns>
        public static double GetPathLength(this List<Point> pts, int pA, int pB, List<double> cumulativePathLength)
        {
            if (pB - pA > 0)
            {
                return cumulativePathLength[pB] - cumulativePathLength[pA];
            }
            else //if does cross the contour boundary
            {
                var cumulative_pA_pCircularFirst = cumulativePathLength[cumulativePathLength.Count - 1] - cumulativePathLength[pA];
                var cumulative_pFirst_pB = -(cumulativePathLength[0] - cumulativePathLength[pB]);

                return cumulative_pA_pCircularFirst + cumulative_pFirst_pB;
            }
        }

        /// <summary>
        /// Gets cumulative distance for a contour.
        /// </summary>
        /// <param name="pts">Contour.</param>
        /// <returns>Cumulative distance.</returns>
        public static List<double> CumulativeEuclideanDistance(this List<Point> pts)
        {
            var cumulativeDistances = new List<double>();

            double cumulativeDistance = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                var idxA = i;
                var idxB = (i + 1) % pts.Count;

                cumulativeDistance += ((PointF)pts[idxA]).DistanceTo(pts[idxB]);
                cumulativeDistances.Add(cumulativeDistance);
            }

            return cumulativeDistances;
        }

        /// <summary>
        /// Gets closest point to the <see cref="ptIdx"/> starting from a <see cref="startIdx"/> moving in the <see cref="direction"/>.
        /// Scale <see cref="scale"/> is used to avoid local minima if contour is noisy.
        /// </summary>
        /// <param name="contour">Contour.</param>
        /// <param name="ptIdx">Point index for which to find the closest point.</param>
        /// <param name="startIdx">Start point from which the search begins.</param>
        /// <param name="direction">Search direction. If &gt 0 then search continoues in the positive direction, if &lt 0 then search is continoued toward negative indeces.</param>
        /// <param name="scale">A good value is ~15. A specified region will be searched every time to avoid local minima.</param>
        /// <returns>Closest point index regarding <see cref="ptIdx"/>,</returns>
        public static int GetClosestPoint(this List<Point> contour, int ptIdx, int startIdx, int direction, int scale)
        {
            double distance;
            return GetClosestPoint(contour, ptIdx, startIdx, direction, scale, out distance);
        }

        /// <summary>
        /// Gets closest point to the <see cref="ptIdx"/> starting from a <see cref="startIdx"/> moving in the <see cref="direction"/>.
        /// Scale <see cref="scale"/> is used to avoid local minima if contour is noisy.
        /// </summary>
        /// <param name="contour">Contour.</param>
        /// <param name="ptIdx">Point index for which to find the closest point.</param>
        /// <param name="startIdx">Start point from which the search begins.</param>
        /// <param name="direction">Search direction. If &gt 0 then search continoues in the positive direction, if &lt 0 then search is continoued toward negative indeces.</param>
        /// <param name="scale">A good value is ~15. A specified region will be searched every time to avoid local minima.</param>
        /// <param name="distance">Distance from <see cref="ptIdx"/> to returned point index.</param>
        /// <returns>Closest point index regarding <see cref="ptIdx"/>,</returns>
        public static int GetClosestPoint(this List<Point> contour, int ptIdx, int startIdx, int direction, int scale, out double distance)
        {
            double minDist = Double.MaxValue;
            int closestPt = -1;

            Point pt = contour[ptIdx];
            int idx = startIdx;

            while (true)
            {
                var idxB = (idx + direction * scale) % contour.Count;
                if (idxB < 0) idxB += contour.Count;

                double foundDist;
                var searchRange = (direction > 0) ? new Range(idx, idxB) : new Range(idxB, idx);
                int foundPt = GetClosestPoint(contour, ptIdx, searchRange, out foundDist);

                idx = idxB;

                if (foundDist < minDist)
                {
                    minDist = foundDist;
                    closestPt = foundPt;
                }
                else
                    break;
            }

            distance = minDist;
            return closestPt;
        }

        /// <summary>
        /// Gets closest point to the <see cref="ptIdx"/>.
        /// </summary>
        /// <param name="contour">Cotour.</param>
        /// <param name="ptIdx">Point index for which to find the closest point.</param>
        /// <param name="searchSegment">Contour segment to search.</param>
        /// <param name="distance">Distance from <see cref="ptIdx"/> to returned point index.</param>
        /// <returns>Closest point index regarding <see cref="ptIdx"/>,</returns>
        public static int GetClosestPoint(this List<Point> contour, int ptIdx, Range searchSegment, out double distance)
        {
            double minDist = Double.MaxValue;
            int closestPt = -1;

            Point pt = contour[ptIdx];
            int idx = (int)searchSegment.Min;

            while (idx != (int)searchSegment.Max)
            {
                var dist = ((PointF)pt).DistanceTo(contour[idx]);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestPt = idx;
                }

                idx = (idx + 1) % contour.Count;
            }

            distance = minDist;
            return closestPt;
        }

        /// <summary>
        /// Finds vaelys and peaks.
        /// </summary>
        /// <param name="contourPts">Contour.</param>
        /// <param name="selector">Filter function where peak or valey is choosen. Angle range can be determined also.
        /// </param>
        /// <param name="scale">A good value is ~15. A specified amount will be skipped every time to avoid local minima.</param>
        /// <param name="peaks">Found peaks.</param>
        /// <param name="valeys">Found valeys.</param>
        /// <remarks>
        /// Sample usage:
        /// <code>
        ///     contourPts.FindExtremaIndices((angle, isPeak)=>
        ///     {
        ///         if ((isPeak && angle &gt 0 && angle &lt 90) || //peak filtering
        ///             (!isPeak && angle &gt 0 && angle &lt 90))  //valey filtering
        ///             return true;
        ///
        ///         return false;
        ///     }, 
        ///     scale, out peakIndeces, out valeyIndeces);
        /// </code>
        /// </remarks>
        public static void FindExtremaIndices(this List<Point> contourPts, Func<float, bool, bool> selector, int scale,
                                              out List<int> peaks, out List<int> valeys)
        {
            peaks = new List<int>(); valeys = new List<int>();
           
            for (int i = 0; i < contourPts.Count; i++)
            {
                var idxL = (i - scale) % contourPts.Count; if (idxL < 0) idxL = contourPts.Count + idxL;
                var idxC = i;
                var idxR = (i + scale) % contourPts.Count;

                Vector2D v1 = new Vector2D(contourPts[idxC], contourPts[idxL]);
                Vector2D v2 = new Vector2D(contourPts[idxC], contourPts[idxR]);

                var angle = System.Math.Acos(Vector2D.DotProduct(v1, v2)) * 180 / System.Math.PI;

                var z = Vector2D.CrossProduct(v1, v2);
                bool isPeak = z >= 0;

                if (selector((float)angle, isPeak))
                {
                    if (isPeak)
                    {
                        peaks.Add(i);
                    }
                    else
                    {
                        valeys.Add(i);
                    }
                }

            }
        }

        /// <summary>
        /// Finds humps in contour. Hump scale is determined by <see cref="scale"/>. 
        /// <para>For each peak a closest valey is found. Next for that valey a closet point is found. Those three point make hump.</para>
        /// <para>Hump searching will be successful even when only one peak and one valey are found; it can be successful where peak and valey search against convex hull does not give good results.</para>
        /// <para></para>Peaks and valeys can be obtained by using <see cref="FindExtremaIndices"/>.
        /// </summary>
        /// <param name="contour">Contour.</param>
        /// <param name="peaks">Peaks.</param>
        /// <param name="valeys">Valeys.</param>
        /// <param name="scale">Used for <see cref="GetClosestPoint"/>. A good value is ~20. A specified region will be searched every time to avoid local minima.</param>
        /// <param name="humpPeaks">Found hump peaks.</param>
        /// <returns>Humps contour indeces.</returns>
        public static List<Range> GetHumps(this List<Point> contour, List<int> peaks, List<int> valeys, int scale, out List<int> humpPeaks)
        {
            List<Range> humps = new List<Range>();
            humpPeaks = new List<int>();

            if (valeys.Count == 0) return humps;

            foreach (var peak in peaks)
            { 
                var closestValey = valeys.MinBy(valey => System.Math.Abs(valey - peak));

                var searchDirection = ((peak - closestValey) > 0) ? 1 : -1;
                int closestPtToValey = contour.GetClosestPoint(closestValey, peak, searchDirection, scale);

                if (closestPtToValey == closestValey) //skip "humps" with zero elements
                    continue;

                Range hump;
                
                if (searchDirection < 0)
                    hump = new Range(closestPtToValey, closestValey);
                else
                    hump = new Range(closestValey, closestPtToValey);

                //check if a hump contain some other peaks and valey; classify it as a bad hump
                if (hump.IsInside(peaks).Count(x => x == true) > 1 ||/*discard the current peak*/
                    hump.IsInside(valeys).Count(x => x == true) > 1)
                { }
                else
                {
                    humps.Add(hump);
                    humpPeaks.Add(peak);
                }
            }

            return humps;
        }

        /// <summary>
        /// Clusters points. Maximum successive distance between two points is specified by <see cref="clusterRange"/>.
        /// <para>Points must be ordered!. </para>
        /// Can be useful to group data after <see cref="FindExtremaIndices"/> is used.
        /// </summary>
        /// <param name="contour">Contour.</param>
        /// <param name="ptIndeces">Point indeces.</param>
        /// <param name="clusterRange">Maximum successive point disatnce.</param>
        /// <param name="cumulativeDistance">Cumulative contour distance. If not specified it will be automatically calculated.</param>
        /// <returns>Point clusters.</returns>
        public static List<List<int>> ClusterPoints(this List<Point> contour, List<int> ptIndeces, double clusterRange, List<double> cumulativeDistance = null)
        {
            cumulativeDistance = cumulativeDistance ?? contour.CumulativeEuclideanDistance();

            List<List<int>> clusters = new List<List<int>>();
            int lastPtIdx = -1;

            foreach (var peakIdx in ptIndeces)
            {
                if (lastPtIdx == -1 || contour.GetPathLength(lastPtIdx, peakIdx, cumulativeDistance) > clusterRange)
                {
                    clusters.Add(new List<int>());
                }

                clusters.Last().Add(peakIdx);
                lastPtIdx = peakIdx;
            }

            //circular indexing (check whether the first cluster can be merged with the last one)
            if (clusters.Count != 0 && contour.GetPathLength(clusters.Last().Last(), clusters.First().First(), cumulativeDistance) <= clusterRange)
            {
                for (int i = 0; i < clusters.Last().Count; i++)
                {
                    clusters.Last()[i] -= contour.Count;
                }

                clusters.Last().AddRange(clusters.First());
                clusters.RemoveAt(0);
            }

            return clusters;
        }

        /// <summary>
        /// This will order the points clockwise starting from the 12 o'clock. 
        /// </summary>
        /// <param name="points">Points to sort clockwise</param>
        /// <returns>Sorted point indexes.</returns>
        public static IEnumerable<int> SortPointsClockwise(this IEnumerable<Point> points)
        {
            PointF center = new PointF
            {
                X = (float)points.Select(x => x.X).Average(),
                Y = (float)points.Select(x => x.Y).Average()
            };

            var sortedIndeces = points
                                .Select((x, i) => new
                                {
                                    Index = i,
                                    Value = (System.Math.Atan2(x.Y - center.Y, x.X - center.X) * 180 / System.Math.PI - 90 + 360) % 360
                                })
                                .OrderBy(x => x.Value)
                                .Select(x => x.Index)
                                .ToList();

            return sortedIndeces;
            //return Enumerable.Range(0, points.Count());
        }

        /*private static int less(PointF center, PointF a, PointF b)
        {
            //  Variables to Store the atans
            double aTanA, aTanB;

            //  Fetch the atans
            aTanA = Math.Atan2(a.Y - center.Y, a.X - center.X);
            aTanB = Math.Atan2(b.Y - center.Y, b.X - center.X);

            //  Determine next point in Clockwise rotation
            if (aTanA < aTanB) return -1;
            else if (aTanB < aTanA) return 1;
            return 0;
        }*/

    }
}
