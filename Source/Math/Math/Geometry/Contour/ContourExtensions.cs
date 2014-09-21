#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
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
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using Range = AForge.IntRange;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Contour extensions
    /// </summary>
    public static class ContourExtensions_Point32i
    {
        /// <summary>
        /// Gets path length between specified points.
        /// </summary>
        /// <param name="pts">Point collection.</param>
        /// <param name="pA">Start point.</param>
        /// <param name="pB">End point.</param>
        /// <param name="cumulativePathLength">Contour cumulative path length.</param>
        /// <returns>Distance from start to end point (by following contour points).</returns>
        public static double GetPathLength(this IList<Point> pts, int pA, int pB, List<float> cumulativePathLength)
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
        /// Gets cumulative distance for a contour (threated as closed contour).
        /// </summary>
        /// <param name="pts">Contour.</param>
        /// <param name="treatAsClosed">Treat as closed contour (distance from the last to the first point is added).</param>
        /// <returns>Cumulative distance.</returns>
        public static List<float> CumulativeEuclideanDistance(this IList<Point> pts, bool treatAsClosed = true)
        {
            var cumulativeDistances = new List<float>();
            var maxOffset = treatAsClosed ? 0 : -1;

            float cumulativeDistance = 0;
            for (int i = 0; i < pts.Count + maxOffset; i++)
            {
                var idxA = i;
                var idxB = (i + 1) % pts.Count;

                cumulativeDistance += ((PointF)pts[idxA]).DistanceTo(pts[idxB]);
                cumulativeDistances.Add(cumulativeDistance);
            }

            return cumulativeDistances;
        }

        /// <summary>
        /// Gets closest point to the <paramref name="ptIdx"/> starting from a <paramref name="startIdx"/> moving in the <paramref name="direction"/>.
        /// Scale <paramref name="scale"/> is used to avoid local minima if contour is noisy.
        /// </summary>
        /// <param name="contour">Contour.</param>
        /// <param name="ptIdx">Point index for which to find the closest point.</param>
        /// <param name="startIdx">Start point from which the search begins.</param>
        /// <param name="direction">Search direction. If &gt; 0 then search continues in the positive direction, if &lt; 0 then search is continued toward negative indexes.</param>
        /// <param name="scale">A good value is ~15. A specified region will be searched every time to avoid local minima.</param>
        /// <returns>Closest point index regarding <paramref name="ptIdx"/>.</returns>
        public static int GetClosestPoint(this IList<Point> contour, int ptIdx, int startIdx, int direction, int scale)
        {
            double distance;
            return GetClosestPoint(contour, ptIdx, startIdx, direction, scale, out distance);
        }

        /// <summary>
        /// Gets closest point to the <paramref name="ptIdx"/> starting from a <paramref name="startIdx"/> moving in the <paramref name="direction"/>.
        /// Scale <paramref name="scale"/> is used to avoid local minima if contour is noisy.
        /// </summary>
        /// <param name="contour">Contour.</param>
        /// <param name="ptIdx">Point index for which to find the closest point.</param>
        /// <param name="startIdx">Start point from which the search begins.</param>
        /// <param name="direction">Search direction. If &gt; 0 then search continues in the positive direction, if &lt; 0 then search is continued toward negative indexes.</param>
        /// <param name="scale">A good value is ~15. A specified region will be searched every time to avoid local minima.</param>
        /// <param name="distance">Distance from <paramref name="ptIdx"/> to returned point index.</param>
        /// <returns>Closest point index regarding <paramref name="ptIdx"/>.</returns>
        public static int GetClosestPoint(this IList<Point> contour, int ptIdx, int startIdx, int direction, int scale, out double distance)
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
        /// Gets closest point to the <paramref name="ptIdx"/>.
        /// </summary>
        /// <param name="contour">Contour.</param>
        /// <param name="ptIdx">Point index for which to find the closest point.</param>
        /// <param name="searchSegment">Contour segment to search.</param>
        /// <param name="distance">Distance from <paramref name="ptIdx"/> to returned point index.</param>
        /// <returns>Closest point index regarding <paramref name="ptIdx"/>.</returns>
        public static int GetClosestPoint(this IList<Point> contour, int ptIdx, Range searchSegment, out double distance)
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
        /// Finds valeys and peaks.
        /// </summary>
        /// <param name="contourPts">Contour.</param>
        /// <param name="selector">Filter function where peak or valey is choosen. Angle range can be determined also. </param>
        /// <param name="scale">A good value is ~15. A specified amount will be skipped every time to avoid local minima.</param>
        /// <param name="peaks">Found peaks.</param>
        /// <param name="valeys">Found valeys.</param>
        /// <remarks>
        /// Sample usage:
        /// <code>
        ///     contourPts.FindExtremaIndices((angle, isPeak)=>
        ///     {
        ///         if ((isPeak AND angle ;gt 0 AND angle ;lt 90) || //peak filtering
        ///             (!isPeak AND angle ;gt 0 AND angle ;lt 90))  //valey filtering
        ///             return true;
        ///
        ///         return false;
        ///     }, 
        ///     scale, out peakIndeces, out valeyIndeces);
        /// </code>
        /// </remarks>
        public static void FindExtremaIndices(this IList<Point> contourPts, Func<float, bool, bool> selector, int scale,
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
        /// Finds humps in contour. Hump scale is determined by <paramref name="scale"/>. 
        /// <para>For each peak a closest valley is found. Next for that valley a closet point is found. Those three point make hump.</para>
        /// <para>Hump searching will be successful even when only one peak and one valley are found; it can be successful where peak and valley search against convex hull does not give good results.</para>
        /// <para></para>Peaks and valleys can be obtained by using <see cref="FindExtremaIndices"/>.
        /// </summary>
        /// <param name="contour">Contour.</param>
        /// <param name="peaks">Peaks.</param>
        /// <param name="valeys">Valleys.</param>
        /// <param name="scale">Used for <see cref="GetClosestPoint"/>. A good value is ~20. A specified region will be searched every time to avoid local minimum.</param>
        /// <param name="humpPeaks">Found hump peaks.</param>
        /// <returns>Humps contour indexes.</returns>
        public static List<Range> GetHumps(this IList<Point> contour, List<int> peaks, List<int> valeys, int scale, out List<int> humpPeaks)
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
        /// Clusters points. Maximum successive distance between two points is specified by <paramref name="clusterRange"/>.
        /// <para>Points must be ordered!. </para>
        /// Can be useful to group data after <see cref="FindExtremaIndices"/> is used.
        /// </summary>
        /// <param name="contour">Contour.</param>
        /// <param name="ptIndeces">Point indexes.</param>
        /// <param name="clusterRange">Maximum successive point distance.</param>
        /// <param name="cumulativeDistance">Cumulative contour distance. If not specified it will be automatically calculated.</param>
        /// <returns>Point clusters.</returns>
        public static List<List<int>> ClusterPoints(this IList<Point> contour, List<int> ptIndeces, double clusterRange, List<float> cumulativeDistance = null)
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
        /// Order the points clockwise starting from the 12 o'clock. 
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
        }

        /// <summary>
        /// Gets the minimum bounding rectangle around the points.
        /// </summary>
        /// <param name="points">Contour points.</param>
        /// <returns>Bounding rectangle.</returns>
        public static Rectangle BoundingRect(this IEnumerable<Point> points)
        {
            if (points.Any() == false) return Rectangle.Empty;

            int minX = Int32.MaxValue, maxX = Int32.MinValue,
                minY = Int32.MaxValue, maxY = Int32.MinValue;

            foreach (var pt in points)
            {
                if (pt.X < minX)
                    minX = pt.X;
                if (pt.X > maxX)
                    maxX = pt.X;

                if (pt.Y < minY)
                    minY = pt.Y;
                if (pt.Y > maxY)
                    maxY = pt.Y;
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Gets the center of the mass of the contour.
        /// </summary>
        /// <param name="points">Contour points.</param>
        /// <returns>The center of the mass of the contour.</returns>
        public static PointF Center(this IEnumerable<Point> points)
        {
            PointF average = new PointF();
            int nSamples = 0;

            foreach (var pt in points)
            {
                average.X += pt.X;
                average.Y += pt.Y;
                nSamples++;
            }

            average.X /= nSamples;
            average.Y /= nSamples;

            return average;
        }

        /// <summary>
        /// Determines whether the polygon forms rectangle.
        /// </summary>
        /// <param name="points">Polygon.</param>
        /// <returns>True if the polygon forms rectangle, false otherwise.</returns>
        public static bool IsRectangle(this IEnumerable<Point> points)
        {
            if (points.Count() != 4)
                return false;

            var rect = points.BoundingRect();

            bool hasTopLeft = false, hasTopRight = false, hasBottomLeft = false, hasBottomRight = false;

            foreach (var pt in points)
            {
                if (rect.Top == pt.Y)
                {
                    if (rect.X == pt.X)
                        hasTopLeft = true;

                    if (rect.Right == pt.X)
                        hasTopRight = true;
                }

                if (rect.Bottom == pt.Y)
                {
                    if (rect.X == pt.X)
                        hasBottomLeft = true;

                    if (rect.Right == pt.X)
                        hasBottomRight = true;
                }
            }

            return hasTopLeft && hasTopRight && hasBottomLeft && hasBottomRight;
        }
    }

    /// <summary>
    /// Contour extensions
    /// </summary>
    public static class ContourExtensions_Point32f
    {
        /// <summary>
        /// Order the points clockwise starting from the 12 o'clock. 
        /// </summary>
        /// <param name="points">Points to sort clockwise</param>
        /// <returns>Sorted point indexes.</returns>
        public static IEnumerable<int> SortPointsClockwise(this IEnumerable<PointF> points)
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
        }

        /// <summary>
        /// Gets cumulative distance for a contour (threated as closed contour).
        /// </summary>
        /// <param name="pts">Contour.</param>
        /// <param name="treatAsClosed">Treat as closed contour (distance from the last to the first point is added).</param>
        /// <returns>Cumulative distance.</returns>
        public static List<float> CumulativeEuclideanDistance(this IList<PointF> pts, bool treatAsClosed = true)
        {
            var cumulativeDistances = new List<float>();
            var maxOffset = treatAsClosed ? 0 : -1;

            float cumulativeDistance = 0;
            for (int i = 0; i < pts.Count + maxOffset; i++)
            {
                var idxA = i;
                var idxB = (i + 1) % pts.Count;

                cumulativeDistance += ((PointF)pts[idxA]).DistanceTo(pts[idxB]);
                cumulativeDistances.Add(cumulativeDistance);
            }

            return cumulativeDistances;
        }

        /// <summary>
        /// Gets the minimum bounding rectangle around the points.
        /// </summary>
        /// <param name="points">Contour points.</param>
        /// <returns>Bounding rectangle.</returns>
        public static RectangleF BoundingRect(this IEnumerable<PointF> points)
        {
            if (points.Any() == false) return RectangleF.Empty;

            float minX = Single.MaxValue, maxX = Single.MinValue, 
                  minY = Single.MaxValue, maxY = Single.MinValue;

            foreach (var pt in points)
            {
                if (pt.X < minX)
                    minX = pt.X;
                if (pt.X > maxX)
                    maxX = pt.X;

                if (pt.Y < minY)
                    minY = pt.Y;
                if (pt.Y > maxY)
                    maxY = pt.Y;
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Gets the center of the mass of the contour.
        /// </summary>
        /// <param name="points">Contour points.</param>
        /// <returns>The center of the mass of the contour.</returns>
        public static PointF Center(this IEnumerable<PointF> points)
        {
            PointF average = new PointF();
            int nSamples = 0;

            foreach (var pt in points)
            {
                average.X += pt.X;
                average.Y += pt.Y;
                nSamples++;
            }

            average.X /= nSamples;
            average.Y /= nSamples;

            return average;
        }

        /// <summary>
        /// Gets equaly distributed points allong a contour.
        /// </summary>
        /// <param name="points">Contout points.</param>
        /// <param name="numberOfPoints">Number of points to take.</param>
        /// <param name="treatAsClosed">Treat contour as closed meaning that the distance between the last and the first point will be also calculated.</param>
        /// <param name="takeFirstPoint">Force to include the first point. Otherwise it may not be included.</param>
        /// <returns>Equaly distibuted points.</returns>
        public static IEnumerable<PointF> GetEqualyDistributedPoints(this IList<PointF> points, int numberOfPoints, bool treatAsClosed = true, bool takeFirstPoint = false)
        {
            var cumulativeDistance = CumulativeEuclideanDistance(points, treatAsClosed);
            var distanceBetween = cumulativeDistance.Last() / (numberOfPoints + (takeFirstPoint ? 0: 0.5));

            var previousDist = takeFirstPoint ? (cumulativeDistance.First() - distanceBetween): 0;
            for (int i = 0; i < cumulativeDistance.Count; i++)
            {
                var dist = cumulativeDistance[i] - previousDist;
                if (dist > distanceBetween)
                {
                    var error = System.Math.Abs(distanceBetween - dist);
                    previousDist = cumulativeDistance[i] - error; //correct feature distance between points
                    yield return points[i];
                }
            }
        }

        /// <summary>
        /// Determines whether the polygon forms rectangle.
        /// </summary>
        /// <param name="points">Polygon.</param>
        /// <returns>True if the polygon forms rectangle, false otherwise.</returns>
        public static bool IsRectangle(this IEnumerable<PointF> points)
        {
            if (points.Count() != 4)
                return false;

            var rect = points.BoundingRect();

            bool hasTopLeft = false, hasTopRight = false, hasBottomLeft = false, hasBottomRight = false;

            foreach (var pt in points)
            {
                if (rect.Top == pt.Y)
                {
                    if (rect.X == pt.X)
                        hasTopLeft = true;

                    if (rect.Right == pt.X)
                        hasTopRight = true;
                }

                if (rect.Bottom == pt.Y)
                {
                    if (rect.X == pt.X)
                        hasBottomLeft = true;

                    if (rect.Right == pt.X)
                        hasBottomRight = true;
                }
            }

            return hasTopLeft && hasTopRight && hasBottomLeft && hasBottomRight;
        }
    }
}
