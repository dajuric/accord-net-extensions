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

using Accord.Math;
using PointF = AForge.Point;

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Linear acceleration model for 2D case.
    /// <para>Vector is composed as: [X, vX, Y, vY]</para>
    /// Model is constructed as following:
    /// p(i) = p(i-1) + (& Delta t) * v(i-1);
    /// v(i) = v(i-1);
    /// </summary>
    public struct ConstantVelocity2DModel
    {
        public const int Dimension = 4;

        public PointF Position;
        public PointF Velocity;

        public double[] ToArray()
        {
            return ToArray(this);
        }

        public static ConstantVelocity2DModel FromArray(double[] arr)
        {
            return new ConstantVelocity2DModel
            {
                Position = new PointF((float)arr[0], (float)arr[2]),
                Velocity = new PointF((float)arr[1], (float)arr[3]),
            };
        }

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

        public static double[,] GetProcessNoise(double positionError, double velocityError)
        {
            return Matrix.Diagonal(Dimension, new double[] { positionError, positionError, 
                                                             velocityError, velocityError});
        }

        public static ConstantVelocity2DModel Evaluate(ConstantVelocity2DModel state, double[,] transitionMat)
        {
            var stateVector = state.ToArray().Multiply(transitionMat);
            return ConstantVelocity2DModel.FromArray(stateVector);
        }
    }
}
