using System;
using System.Collections.Generic;
using System.Linq;
using PointF = AForge.Point;

namespace Accord.Math.Geometry
{
    /// <summary>
    /// Represents cardinal cubic spline which is a type of C(1) interpolating spline made up of cubic polynomial segments.
    /// <remarks>
    /// See: 
    /// <para><see cref="http://research.cs.wisc.edu/graphics/Courses/559-f2004/docs/cs559-splines.pdf"/></para> and
    /// <para><see cref="http://www.intelligence.tuc.gr/~petrakis/courses/computervision/splines.pdf"/></para>.
    /// </remarks>
    /// </summary>
    public class CardinalSpline: ICloneable
    {
        public const int NUM_DERIVATIVE_POINTS = 2;
        public const int MIN_INDEX = 0;
        public const int MAX_INDEX_OFFSET = -NUM_DERIVATIVE_POINTS;

        List<PointF> controlPoints;

        #region Constructors

        /// <summary>
        /// Creates cardinal spline.
        /// </summary>
        /// <param name="tension">User specified tension.</param>
        public CardinalSpline(float tension = 0.5f)
        { 
            initialize(tension);
        }

        /// <summary>
        /// Creates cardinal spline.
        /// </summary>
        /// <param name="controlPoints">Control points for the curve.</param>
        /// <param name="tension">User specified tension.</param>
        public CardinalSpline(IEnumerable<PointF> controlPoints, float tension = 0.5f)
        { 
            initialize(tension);
            this.controlPoints.AddRange(controlPoints); //nedostaje jedan krak
            /*foreach (var cp in controlPoints)
            {
                this.Add(cp);
            }*/
        }

        private void initialize(float tension)
        {
            controlPoints = new List<PointF>();
            this.Tension = tension;
        }

        #endregion

        #region Properties

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
        /// Gets the number of control points.
        /// </summary>
        public int Count { get { return controlPoints.Count - NUM_DERIVATIVE_POINTS/*2 are derivative points*/; } } //TODO: what to do with this ?

        #endregion

        #region Methods

        /// <summary>
        /// Adds control point to the end of the collection.
        /// </summary>
        /// <param name="controlPoint">Control point to add.</param>
        public void Add(PointF controlPoint)
        {
            PointF p0, p1, p2, p3;

            switch (controlPoints.Count)
            { 
                case 0:
                    controlPoints.Add(controlPoint);
                    break;
                case 1: //make additional approximation points (two)

                    p1 = controlPoints.First();
                    p2 = controlPoint;

                    p0 = new PointF //[0] - deriv at [1]
                    {
                        X = 2 * p1.X - p2.X,
                        Y = 2 * p1.Y - p2.Y
                    };

                    p3 = new PointF //[3] - deriv at [2]
                    {
                        X = 2 * p2.X - p1.X,
                        Y = 2 * p2.Y - p1.Y
                    };

                    controlPoints[0] = p0;
                    controlPoints.Add(p1);
                    controlPoints.Add(p2);
                    controlPoints.Add(p3);
                    break;
                default: //make additional approximation point (one)
                    p2 = controlPoint;

                    p1 = controlPoints[(controlPoints.Count - 1) - 1];

                    p3 = new PointF //[3] - deriv at [2]
                    {
                        X = 2 * p2.X - p1.X,
                        Y = 2 * p2.Y - p1.Y
                    };

                    controlPoints[(controlPoints.Count - 1)] = p2;
                    controlPoints.Add(p3);
                    break;
            }
            
        }

        /// <summary>
        /// Interpolates four control points.
        /// </summary>
        /// <param name="index">Index between two control points.</param>
        /// <returns>Interpolated point.</returns>
        public PointF Interpolate(float index)
        {
            return Interpolate(this.controlPoints, Tension, index);
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
        public PointF NormalDirection(float index)
        {
            return NormalDirection(this.controlPoints, Tension, index);
        }

        /// <summary>
        /// Clones this curvature. Curvature points are not shared.
        /// </summary>
        /// <returns>New cloned curvature.</returns>
        public object Clone()
        {
            var newCardinal = new CardinalSpline(this.Tension);
            newCardinal.controlPoints.AddRange(this.controlPoints);

            return newCardinal;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Interpolates four control points.
        /// </summary>
        /// <param name="index">Index between two control points.</param>
        /// <returns>Interpolated point.</returns>
        public static PointF Interpolate(IList<PointF> controlPoints, float tension, float index)
        {
            if ((index) < 0 || (index + 2) >= controlPoints.Count)
                throw new NotSupportedException("One control point behind and two control points in advance must be available!");

            float s = (1 - tension) / 2;

            int idx = (int)index + 1;
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
        /// Gets derivative at specified index.
        /// </summary>
        /// <param name="index">Index between two control points.</param>
        /// <returns>Derivative at interpolated point.</returns>
        public static PointF DerivativeAt(IList<PointF> controlPoints, float tension, float index)
        {
            if ((index) < 0 || (index + 2) >= controlPoints.Count)
                throw new NotSupportedException("One control point behind and two control points in advance must be available!");

            float s = (1 - tension) / 2;

            int idx = (int)index + 1;
            float u = index - (int)index;

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

            return new PointF(pointX, pointY).Normalize();
        }

        /// <summary>
        /// Gets normal direction at specified point.
        /// </summary>
        /// <param name="index">Index between two control points.</param>
        /// <returns>Normal direction at interpolated point.</returns>
        public static PointF NormalDirection(IList<PointF> controlPoints, float tension, float index)
        {
            var derivPt = DerivativeAt(controlPoints, tension, index);

            return new PointF //rotate 90 degrees (normal is perpendicular)
            {
                X = -derivPt.Y,
                Y = derivPt.X
            };
        }

        #endregion
    }
}
