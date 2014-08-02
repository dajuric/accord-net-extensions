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

using PointF = AForge.Point;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Perspective projection model for 2D case.
    /// Model is constructed as following:
    /// p(i) = (f * D) / (f * D + v(k-1) * d(k-1) * dt) * p(k-1)
    /// d(k) = (f * D) / (f * D + v(k-1) * d(k-1) * dt) * d(k-1)
    /// v(k) - user defined (constant or updated by constant acceleration model)
    /// 
    /// where variables are:
    /// f - focal length
    /// D - object width in real word
    /// d - object width in an image
    /// </summary>
    public class PerspectiveProjection2DModel
    {
        /// <summary>
        /// Gets the dimension of the model.
        /// </summary>
        public const int Dimension = 4;

        /// <summary>
        /// Gets or sets the object position.
        /// </summary>
        public PointF ImagePosition;
        /// <summary>
        /// Gets or sets the image object width.
        /// </summary>
        public double ImageObjectWidth;
        /// <summary>
        /// Gets or sets the real world velocity.
        /// </summary>
        public double Velocity;

        /// <summary>
        /// Evaluates the model by using the provided parameters. The velocity value is copied.
        /// </summary>
        /// <param name="timeInterval">Time interval.</param>
        /// <param name="velocityMultiplierConst">Velocity multiplier which is calculated by using: <see cref="CalculateVelocityMultiplierConstant"/> function.</param>
        /// <returns>New model state.</returns>
        public PerspectiveProjection2DModel Evaluate(double timeInterval, double velocityMultiplierConst)
        {
            var newState = new PerspectiveProjection2DModel();

            var multiplier = 1 / (1 + velocityMultiplierConst * this.Velocity * this.ImageObjectWidth * timeInterval);

            newState.ImagePosition.X = (float)(this.ImagePosition.X * multiplier);
            newState.ImagePosition.Y = (float)(this.ImagePosition.Y * multiplier);
            newState.ImageObjectWidth = this.ImageObjectWidth * multiplier;
            newState.Velocity = this.Velocity;

            return newState;
        }

        /// <summary>
        /// Calculates velocity multiplier constant.
        /// </summary>
        /// <param name="objectWorldWidth">Real world object width.</param>
        /// <param name="focalLength">Focal length of the camera.</param>
        /// <param name="meterToPixelMultiplier">Meter to pixel constant (depends on camera sensor).</param>
        /// <returns>Velocity multiplier constant</returns>
        public static double CalculateVelocityMultiplierConstant(double objectWorldWidth, double focalLength, double meterToPixelMultiplier)
        {
            return meterToPixelMultiplier / (focalLength * objectWorldWidth);
        }

        /// <summary>
        /// Estimates transition matrix using numeric Jacobian calculation.
        /// <para>If using Kalman transition matrix must be updated in each step because the model is not linear.</para>
        /// </summary>
        /// <param name="velocityMultiplierConst">Velocity multiplier constant</param>
        /// <param name="delta">Delta factor for Jacobian estimation.</param>
        /// <returns>Estimated transition matrix.</returns>
        public double[,] EstimateTransitionMatrix(double velocityMultiplierConst, double delta = 1e-3)
        {
            return Math.MathExtensions.CalculateJacobian(x =>
                        {
                            var st = FromArray(x);
                            var output = Evaluate(delta, velocityMultiplierConst);
                            return ToArray(output); 
                        },
                        ToArray(this), delta);
        }

        /// <summary>
        /// Gets the measurement matrix [3 x 4] - [X, Y, velocity].
        /// </summary>
        /// <returns>Measurement matrix.</returns>
        public static double[,] GetMeasurementMatrix()
        {
            return new double[,] 
            {
                //X,   Y,   width,   velocity 
                {1,    0,     0,       0}, //picks x
                {0,    1,     0,       0}, //picks y
                {0,    0,     1,       0}  //picks width
            };
        }

        #region Array conversions

        /// <summary>
        /// Converts the array to the model.
        /// </summary>
        /// <param name="arr">Array to convert from.</param>
        /// <returns>Model.</returns>
        public static PerspectiveProjection2DModel FromArray(double[] arr)
        {
            return new PerspectiveProjection2DModel
            {
                ImagePosition = new PointF
                {
                    X = (float)arr[0],
                    Y = (float)arr[1]
                },
                ImageObjectWidth = arr[2],
                Velocity = arr[3]
            };
        }

        /// <summary>
        /// Converts the model to the array.
        /// </summary>
        /// <param name="modelState">Model to convert.</param>
        /// <returns>Array.</returns>
        public static double[] ToArray(PerspectiveProjection2DModel modelState)
        {
            return new double[] 
            {
                modelState.ImagePosition.X,
                modelState.ImagePosition.Y,
                modelState.ImageObjectWidth,
                modelState.Velocity
            };
        }

        #endregion

        /// <summary>
        /// Clones the model.
        /// </summary>
        /// <returns>The copy of the model.</returns>
        public PerspectiveProjection2DModel Clone()
        {
            return new PerspectiveProjection2DModel 
            {
                ImagePosition = this.ImagePosition,
                ImageObjectWidth = this.ImageObjectWidth,
                Velocity = this.Velocity
            };
        }
    }

    /// <summary>
    /// Contains extensions for perspective 2D model.
    /// </summary>
    public static class PerspectiveProjection2DModelExtensions
    {
        /// <summary>
        /// Translates the position of the model.
        /// </summary>
        /// <param name="state">Current state.</param>
        /// <param name="offset">Position offset.</param>
        /// <returns>New state.</returns>
        public static PerspectiveProjection2DModel Translate(this PerspectiveProjection2DModel state, PointF offset)
        {
            var newState = state.Clone();
            newState.ImagePosition.X += offset.X;
            newState.ImagePosition.Y += offset.Y;

            return newState;
        }
    }
}
