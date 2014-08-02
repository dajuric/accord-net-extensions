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
    /// <para>Vector is composed as: [X, vX, aX, Y, vY, aY]</para>
    /// Model is constructed as following:
    /// p(i) = p(i-1) + (&#x0394;t) * v(i-1) + ((&#x0394;t)^2 / 2) * a(t-1);
    /// v(i) = v(i-1) + (&#x0394;t) * a(t-1);
    /// a(t) = a(t-1);
    /// 
    /// <para>Look at: http://hyperphysics.phy-astr.gsu.edu/hbase/acons.html </para>
    /// </summary>
    public class ConstantAcceleration2DModel: ICloneable //TODO: test it! (last time - wrong results)
    {
        /// <summary>
        /// Gets the dimension of the model.
        /// </summary>
        public const int Dimension = 6;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public PointF Position;
        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        public PointF Velocity;
        /// <summary>
        /// Gets or sets the acceleration.
        /// </summary>
        public PointF Acceleration;

        /// <summary>
        /// Constructs an empty model.
        /// </summary>
        public ConstantAcceleration2DModel()
        {
            this.Position = default(PointF);
            this.Velocity = default(PointF);
            this.Acceleration = default(PointF);
        }

        /// <summary>
        /// Evaluates the model by using the provided transition matrix.
        /// </summary>
        /// <param name="transitionMat">Transition matrix.</param>
        /// <returns>New model state.</returns>
        public ConstantAcceleration2DModel Evaluate(double[,] transitionMat)
        {
            var stateVector = transitionMat.Multiply(ToArray(this));
            return ConstantAcceleration2DModel.FromArray(stateVector);
        }

        /// <summary>
        /// Gets the state transition matrix [6 x 6].
        /// </summary>
        /// <param name="timeInterval">Time interval.</param>
        /// <returns>State transition matrix.</returns>
        public static double[,] GetTransitionMatrix(double timeInterval = 1)
        {
            var t = timeInterval;
            var a = 1/2f * t * t;

            return new double[,] 
                { 
                    {1, t, a, 0, 0, 0}, 
                    {0, 1, t, 0, 0, 0}, 
                    {0, 0, 1, 0, 0, 0},
                    {0, 0, 0, 1, t, a},
                    {0, 0, 0, 0, 1, t},
                    {0, 0, 0, 0, 0, 1}
                };
        }

        /// <summary>
        /// Gets the position measurement matrix [2 x 6] used in Kalman filtering.
        /// </summary>
        /// <returns>Position measurement matrix.</returns>
        public static double[,] GetPositionMeasurementMatrix()
        {
            return new double[,] //just pick point coordinates for an observation [2 x 6] (look at used state model)
                { 
                   //X,  vX, aX, Y,  vY  aY  (look at ConstantAcceleration2DModel)
                    {1,  0,  0,  0,  0,  0}, //picks X
                    {0,  0,  0,  1,  0,  0}  //picks Y
                };
        }

        /// <summary>
        /// Gets process-noise matrix [6 x 2] where the location is affected by (dt * dt * dt) / 6, velocity with the factor of (dt * dt) / 2 and the acceleration with the factor dt - integrals of dt. 
        /// Factor 'dt' represents time interval.
        /// </summary>
        /// <param name="noise">Acceleration noise.</param>
        /// <param name="timeInterval">Time interval.</param>
        /// <returns>Process noise matrix.</returns>
        public static double[,] GetProcessNoise(double noise, double timeInterval = 1)
        {
            var dt = timeInterval;
            var G = new double[,] 
            { 
                {(dt * dt * dt) / 6, 0},
                {(dt * dt) / 2, 0},
                {dt, 0},

                {0, (dt * dt * dt) / 6},
                {0, (dt * dt) / 2},
                {dt, 0}
            };

            var Q = Matrix.Diagonal<double>(G.ColumnCount(), noise); //TODO - check: noise * noise ?
            var processNoise = G.Multiply(Q).Multiply(G.Transpose());
            return processNoise;
        }


        #region Array conversion

        /// <summary>
        /// Converts the array to the model.
        /// </summary>
        /// <param name="arr">Array to convert from.</param>
        /// <returns>Model.</returns>
        public static ConstantAcceleration2DModel FromArray(double[] arr)
        {
            return new ConstantAcceleration2DModel
            {
                Position = new PointF((float)arr[0], (float)arr[3]),
                Velocity = new PointF((float)arr[1], (float)arr[4]),
                Acceleration = new PointF((float)arr[2], (float)arr[5])
            };
        }

        /// <summary>
        /// Converts the model to the array.
        /// </summary>
        /// <param name="modelState">Model to convert.</param>
        /// <returns>Array.</returns>
        public static double[] ToArray(ConstantAcceleration2DModel modelState)
        {
            return new double[] //TODO - critical: check if the matrix is valid!
                {
                    modelState.Position.X,
                    modelState.Velocity.X,
                    modelState.Acceleration.X,

                    modelState.Position.Y,
                    modelState.Velocity.Y,
                    modelState.Acceleration.Y
                };
        }

        #endregion

        /// <summary>
        /// Clones the model.
        /// </summary>
        /// <returns>The copy of the model.</returns>
        public object Clone()
        {
            return new ConstantAcceleration2DModel
            {
                Position = this.Position,
                Velocity = this.Velocity,
                Acceleration = this.Acceleration
            };
        }
    }
}
