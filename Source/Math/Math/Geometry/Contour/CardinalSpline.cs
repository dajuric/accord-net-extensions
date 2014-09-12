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
using Accord.Math;
using PointF = AForge.Point;
using RangeF = AForge.Range;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Represents cardinal cubic spline which is a type of C(1) interpolating spline made up of cubic polynomial segments.
    /// <remarks>
    /// See: 
    /// <para><a href="http://research.cs.wisc.edu/graphics/Courses/559-f2004/docs/cs559-splines.pdf"/></para> and
    /// <para><a href="http://www.intelligence.tuc.gr/~petrakis/courses/computervision/splines.pdf"/></para>.
    /// </remarks>
    /// </summary>
    public class CardinalSpline: ICloneable
    {
        /// <summary>
        /// Minimal point index of the cardinal spline.
        /// </summary>
        public const int MIN_INDEX = 1;
        /// <summary>
        /// Offset from the maximal index.
        /// </summary>
        public const int MAX_INDEX_OFFSET = 1;
        /// <summary>
        /// Number of points which are used for interpolation.
        /// </summary>
        public const int NUM_DERIVATIVE_POINTS = 2;

        List<PointF> controlPoints;

        #region Constructors

        /// <summary>
        /// Creates cardinal spline.
        /// </summary>
        /// <param name="controlPoints">Control points for the curve.</param>
        /// <param name="tension">User specified tension.</param>
        /// <param name="addTensionPoints">Adds first and last control point so that current border point can be also interpreted as part of a contour.</param>
        public CardinalSpline(IEnumerable<PointF> controlPoints, float tension = 0.5f, bool addTensionPoints = false)
        { 
            initialize(tension);
            this.controlPoints.AddRange(controlPoints);

            if (addTensionPoints)
                AddTensionPoints(this.controlPoints);
        }

        private void initialize(float tension)
        {
            controlPoints = new List<PointF>();
            this.Tension = tension;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the total number of points.
        /// </summary>
        public IList<PointF> ControlPoints { get { return this.controlPoints; } }

        /// <summary>
        /// Tension. 
        /// <para>
        /// Value 1 gives linear interpolation.
        /// Values smaller than 1 gives greater contour "exterior" tension. 
        /// Values greater than 1 give greater contour "interior" tension.
        /// </para>
        /// </summary>
        public float Tension { get; set; }

        /// <summary>
        /// Gets the total number of points.
        /// <para>The total number of points that can be used for interpolation is totalNumber - <see cref="NUM_DERIVATIVE_POINTS"/></para>
        /// </summary>
        public int Count { get { return controlPoints.Count; } }

        #endregion

        #region Methods

        /// <summary>
        /// Interpolates four control points.
        /// </summary>
        /// <param name="index">Index between two control points.</param>
        /// <returns>Interpolated point.</returns>
        public PointF InterpolateAt(float index)
        {
            return InterpolateAt(this.controlPoints, Tension, index);
        }

        /// <summary>
        /// Gets derivative at specified index.
        /// </summary>
        /// <param name="index">Index between two control points.</param>
        /// <returns>Derivative at interpolated point.</returns>
        public PointF DerivativeAt(float index)
        {
            return DerivativeAt(this.controlPoints, Tension, index);
        }

        /// <summary>
        /// Gets normal direction at specified point.
        /// </summary>
        /// <param name="index">Index between two control points.</param>
        /// <returns>Normal direction at interpolated point.</returns>
        public PointF NormalAt(float index)
        {
            return NormalAt(this.controlPoints, Tension, index);
        }

        /// <summary>
        /// Clones this curvature. Curvature points are not shared.
        /// </summary>
        /// <returns>New cloned curvature.</returns>
        public object Clone()
        {
            return new CardinalSpline(this.ControlPoints, this.Tension, false);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Interpolates four control points.
        /// </summary>
        /// <param name="controlPoints">Control points of the spline.</param>
        /// <param name="tension">Tension of the spline.</param>
        /// <param name="index">Index between two control points.</param>
        /// <returns>Interpolated point.</returns>
        public static PointF InterpolateAt(IList<PointF> controlPoints, float tension, float index)
        {
            if (ValidIndicesRange(controlPoints.Count).IsInside(index) == false)
                throw new NotSupportedException("Index is not in the valid indices range!");

            float s = (1 - tension) / 2;

            int idx = (int)index;
            float u = index - (int)index;

            var U = new float[] { u * u * u, u * u, u, 1 };
            var MC = new float[,] 
            {
                {-s,  2-s, s-2,      s},
                {2*s, s-3, 3-(2*s), -s},
                {-s,  0,   s,        0},
                {0,   1,   0,        0}
            };

            var GHx = new float[] { controlPoints[idx - 1].X, controlPoints[idx].X, controlPoints[idx + 1].X, controlPoints[idx + 2].X };
            var GHy = new float[] { controlPoints[idx - 1].Y, controlPoints[idx].Y, controlPoints[idx + 1].Y, controlPoints[idx + 2].Y };

            var mulFactor = U.Multiply(MC);
            var pointX = mulFactor.Multiply(GHx.Transpose())[0];
            var pointY = mulFactor.Multiply(GHy.Transpose())[0];

            return new PointF(pointX, pointY);
        }

        /// <summary>
        /// Interpolates points and defined indices.
        /// </summary>
        /// <param name="controlPoints">Control points of the spline.</param>
        /// <param name="tension">Tension of the spline.</param>
        /// <param name="indices">Indices where to interpolate values.</param>
        /// <returns>Interpolated points.</returns>
        public static IEnumerable<PointF> InterpolateAt(IList<PointF> controlPoints, float tension, IEnumerable<float> indices)
        {
            return indices.Select(x => CardinalSpline.InterpolateAt(controlPoints, tension, x));
        }

        /// <summary>
        /// Interpolates at indices which are obtained using <paramref name="samplingStep"/>.
        /// <para>Distances between points do not have to be equal because control points may not be equally distributed.</para>
        /// <para>For equally distributed points please use: <seealso cref="GetEqualyDistributedPointIndices"/>.</para>
        /// </summary>
        /// <param name="controlPoints">Control points of the spline.</param>
        /// <param name="tension">Tension of the spline.</param>
        /// <param name="samplingStep">Index increase factor.</param>
        /// <returns>Interpolated points.</returns>
        public static IEnumerable<PointF> Interpolate(IList<PointF> controlPoints, float tension, float samplingStep = 0.3f)
        {
            //interpolate points
            for (float i = MIN_INDEX; i < (controlPoints.Count - 1 - MAX_INDEX_OFFSET); i += samplingStep)
            {
                var pt = CardinalSpline.InterpolateAt(controlPoints, tension, i);
                yield return pt;
            }
        }

        /// <summary>
        /// Gets derivative at specified index.
        /// </summary>
        /// <param name="controlPoints">Control points of the spline.</param>
        /// <param name="tension">Tension of the spline.</param>
        /// <param name="index">Index between two control points.</param>
        /// <param name="approxAtControlPoint">Derivation in control point is zero. 
        /// If true a small offset will be used to avoid zero-point result if a point is control point.</param>
        /// <returns>Derivative at interpolated point.</returns>
        public static PointF DerivativeAt(IList<PointF> controlPoints, float tension, float index, bool approxAtControlPoint = true)
        {
            if (ValidIndicesRange(controlPoints.Count).IsInside(index) == false)
                throw new NotSupportedException("Index is not in the valid indices range!");

            float s = (1 - tension) / 2;

            int idx = (int)index;
            float u = index - (int)index;
            if (approxAtControlPoint && index == (int)index) //if control point is choosen then u == 0 so:
                u += 1E-1f;

            var dU = new float[] { 3 * u * u, 2 * u, 1, 0 };
            var MC = new float[,] 
            {
                {-s,  2-s, s-2,      s},
                {2*s, s-3, 3-(2*s), -s},
                {-s,  0,   s,        0},
                {0,   1,   0,        0}
            };

            var GHx = new float[] { controlPoints[idx - 1].X, controlPoints[idx].X, controlPoints[idx + 1].X, controlPoints[idx + 2].X };
            var GHy = new float[] { controlPoints[idx - 1].Y, controlPoints[idx].Y, controlPoints[idx + 1].Y, controlPoints[idx + 2].Y };

            var mulFactor = dU.Multiply(MC);
            var pointX = mulFactor.Multiply(GHx.Transpose())[0];
            var pointY = mulFactor.Multiply(GHy.Transpose())[0];

            return (PointF)(new Vector2D(pointX, pointY).Normalize());
        }

        /// <summary>
        /// Gets normal direction at specified point.
        /// </summary>
        /// <param name="controlPoints">Control points of the spline.</param>
        /// <param name="tension">Tension of the spline.</param>
        /// <param name="index">Index between two control points.</param>
        /// <param name="approxAtControlPoint">If true a small offset will be used to avoid zero-point result if a point is control point.</param>
        /// <returns>Normal direction at interpolated point.</returns>
        public static PointF NormalAt(IList<PointF> controlPoints, float tension, float index, bool approxAtControlPoint = true)
        {
            var derivPt = DerivativeAt(controlPoints, tension, index, approxAtControlPoint);

            return new PointF //rotate 90 degrees (normal is perpendicular)
            {
                X = -derivPt.Y,
                Y = derivPt.X
            };
        }

        /// <summary>
        /// Gets indices for which points are evenly distributed along contour.
        /// <para>A rough estimation is made at resolution: <paramref name="samplingStep"/> (index step).</para>
        /// </summary>
        /// <param name="controlPoints">Control points of the spline.</param>
        /// <param name="tension">Tension of the spline.</param>
        /// <param name="numPoints">Number of requested points.</param>
        /// <param name="samplingStep">Sampling resolution for calculating contour length. 
        /// <para>Distance between two points will be more accurate if the provided value is lower (sampling resolution is higher). The provided value should be fine for most splines.</para>
        /// <para>If the spline is "very curvy" set it to a lower value. To increase performance set it to a higher one (e.g. 1).</para>
        /// <para>Interval [0..1].</para>
        /// </param>
        /// <returns>Indices for which points are evenly distributed.</returns>
        public static IList<float> GetEqualyDistributedPointIndices(IList<PointF> controlPoints, float tension, int numPoints, float samplingStep = 0.3f) 
        {
            if (numPoints < 2)
                throw new NotSupportedException("The minimal number of points is 2");

            //interpolate points
            var interpolatedPts = Interpolate(controlPoints, tension, samplingStep).ToList();
        
            //calculate cumulative distance
            var cumulativeDistance = interpolatedPts.CumulativeEuclideanDistance(treatAsClosed: false); cumulativeDistance.Insert(0, 0);
            var requestedTwoPointsDist = cumulativeDistance.Last() / (numPoints - 0.5f);

            //calculate spline indices
            var indices = new List<float>();
            float cumDist = cumulativeDistance.First();

            //interpolate indices
            int t = 0;
            int nFitTimesInSegment = 0;
            while (true)
            {
                while (cumDist > cumulativeDistance[t + 1]) //if some segments are too narrow
                {
                    t++;

                    if (t == cumulativeDistance.Count - 1)
                        goto EXIT;
                }

                float interpolatedIdx = (cumDist - cumulativeDistance[t]) / (cumulativeDistance[t+1] - cumulativeDistance[t] + Single.Epsilon); //[0..1] - interpolate into segment
                var idx = MIN_INDEX + (t + interpolatedIdx) * samplingStep;
                indices.Add(idx);

                cumDist += requestedTwoPointsDist;
                nFitTimesInSegment++;
            }

EXIT:
            return indices;
        }

        /// <summary>
        /// Gets valid indices range for interpolation.
        /// <para>Valid range is: [MIN_INDEX.. (count-1+MAX_INDEX_OFFSET - epsilon)]</para>
        /// </summary>
        /// <param name="controlPointsCount">Control points count.</param>
        /// <returns>Valid indices range for interpolation.</returns>
        public static RangeF ValidIndicesRange(int controlPointsCount)
        {
            const float EPSILON = 1E-3f;

            return new RangeF 
            {
                 Min = MIN_INDEX,
                 Max = (controlPointsCount - 1) + MAX_INDEX_OFFSET - EPSILON
            };
        }

        /// <summary>
        /// Adds first and last control point so that current border point can be also interpreted as part of a contour.
        /// <para>Those new points will be added as reflected penultimate points over the last (border) control points.</para>
        /// </summary>
        /// <param name="controlPoints">Control points.</param>
        public static void AddTensionPoints(List<PointF> controlPoints)
        {
            var first = reflectPoint(controlPoints[1], controlPoints[0]);
            var last = reflectPoint(controlPoints[controlPoints.Count - 1 - 1], controlPoints[controlPoints.Count - 1]);

            controlPoints.Insert(0, first);
            controlPoints.Add(last);
        }

        /// <summary>
        /// Reflect point over reflector point.
        /// </summary>
        private static PointF reflectPoint(PointF pt, PointF reflectorPt)
        {
            return new PointF 
            {
                X = reflectorPt.X + (reflectorPt.X - pt.X),
                Y = reflectorPt.Y + (reflectorPt.Y - pt.Y)
            };
        }

        #endregion
    }
}
