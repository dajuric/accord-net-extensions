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
using Accord.Extensions.Math;
using Accord.Math;
using PointF = AForge.Point;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Linear acceleration model for 2D case.
    /// <para>Vector is composed as: [X, vX, Y, vY]</para>
    /// Model is constructed as following:
    /// p(i) = p(i-1) + (&#x0394;t) * v(i-1);
    /// v(i) = v(i-1);
    /// </summary>
    public class ConstantVelocity2DModel: ICloneable
    {
        /// <summary>
        /// Gets the dimension of the model.
        /// </summary>
        public const int Dimension = 4;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public PointF Position;
        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        public PointF Velocity;

        /// <summary>
        /// Constructs an empty model.
        /// </summary>
        public ConstantVelocity2DModel()
        {
            this.Position = default(PointF);
            this.Velocity = default(PointF);
        }

        /// <summary>
        /// Evaluates the model by using the provided transition matrix.
        /// </summary>
        /// <param name="transitionMat">Transition matrix.</param>
        /// <returns>New model state.</returns>
        public ConstantVelocity2DModel Evaluate(double[,] transitionMat)
        {
            var stateVector = transitionMat.Multiply(ToArray(this));
            return ConstantVelocity2DModel.FromArray(stateVector);
        }

        /// <summary>
        /// Gets the state transition matrix [4 x 4].
        /// </summary>
        /// <param name="timeInterval">Time interval.</param>
        /// <returns>State transition matrix.</returns>
        public static double[,] GetTransitionMatrix(double timeInterval = 1)
        {
            var t = timeInterval;

            return new double[,] 
                { 
                    {1, t, 0, 0},
                    {0, 1, 0, 0},
                    {0, 0, 1, t},
                    {0, 0, 0, 1}
                };
        }

        /// <summary>
        /// Gets the position measurement matrix [2 x 4] used in Kalman filtering.
        /// </summary>
        /// <returns>Position measurement matrix.</returns>
        public static double[,] GetPositionMeasurementMatrix()
        {
            return new double[,] //just pick point coordinates for an observation [2 x 6] (look at used state model)
                { 
                   //X,  vX, Y,  vY   (look at ConstantAcceleration2DModel)
                    {1,  0,  0,  0}, //picks X
                    {0,  0,  1,  0}  //picks Y
                };
        }

        /// <summary>
        /// Gets process-noise matrix [4 x 2] where the location is affected by (dt * dt) / 2 and velocity with the factor of dt - integrals of dt. 
        /// Factor 'dt' represents time interval.
        /// </summary>
        /// <param name="accelerationNoise">Acceleration noise.</param>
        /// <param name="timeInterval">Time interval.</param>
        /// <returns>Process noise matrix.</returns>
        public static double[,] GetProcessNoise(double accelerationNoise, double timeInterval = 1)
        {
            var dt = timeInterval;
            var G = new double[,] 
            { 
                {(dt*dt) / 2, 0},
                {dt, 0},
                {0, (dt*dt) / 2},
                {0, dt}
            };

            var Q = Matrix.Diagonal<double>(G.ColumnCount(), accelerationNoise); //TODO - check: noise * noise ?
            var processNoise = G.Multiply(Q).Multiply(G.Transpose());
            return processNoise;
        }

        #region Array conversion

        /// <summary>
        /// Converts the array to the model.
        /// </summary>
        /// <param name="arr">Array to convert from.</param>
        /// <returns>Model.</returns>
        public static ConstantVelocity2DModel FromArray(double[] arr)
        {
            return new ConstantVelocity2DModel
            {
                Position = new PointF((float)arr[0], (float)arr[2]),
                Velocity = new PointF((float)arr[1], (float)arr[3]),
            };
        }

        /// <summary>
        /// Converts the model to the array.
        /// </summary>
        /// <param name="modelState">Model to convert.</param>
        /// <returns>Array.</returns>
        public static double[] ToArray(ConstantVelocity2DModel modelState)
        {
            return new double[] 
                {
                    modelState.Position.X,
                    modelState.Velocity.X,

                    modelState.Position.Y,
                    modelState.Velocity.Y,
                };
        }

        #endregion

        /// <summary>
        /// Clones the model.
        /// </summary>
        /// <returns>The copy of the model.</returns>
        public object Clone()
        {
            return new ConstantVelocity2DModel
            {
                Position = this.Position,
                Velocity = this.Velocity
            };
        }
    }
}
