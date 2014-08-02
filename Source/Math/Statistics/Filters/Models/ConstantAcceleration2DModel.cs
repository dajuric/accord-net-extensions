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
    /// <para>Vector is composed as: [X, vX, aX, Y, vY, aY]</para>
    /// Model is constructed as following:
    /// p(i) = p(i-1) + (& Delta t) * v(i-1) + ((& Delta t)^2 / 2) * a(t-1);
    /// v(i) = v(i-1) + (& Delta t) * a(t-1);
    /// a(t) = a(t-1);
    /// 
    /// <para>Look at: http://hyperphysics.phy-astr.gsu.edu/hbase/acons.html </para>
    /// </summary>
    public struct ConstantAcceleration2DModel //TODO: test it! (last time - wrong results)
    {
        public const int Dimension = 6;

        public PointF Position;
        public PointF Velocity;
        public PointF Acceleration;

        public double[] ToArray()
        {
            return ToArray(this);
        }

        public static ConstantAcceleration2DModel FromArray(double[] arr)
        {
            return new ConstantAcceleration2DModel
            {
                Position = new PointF((float)arr[0], (float)arr[3]),
                Velocity = new PointF((float)arr[1], (float)arr[4]),
                Acceleration = new PointF((float)arr[2], (float)arr[5])
            };
        }

        public static double[] ToArray(ConstantAcceleration2DModel modelState)
        {
            return new double[] 
                {
                    modelState.Position.X,
                    modelState.Velocity.X,
                    modelState.Acceleration.X,

                    modelState.Position.Y,
                    modelState.Velocity.Y,
                    modelState.Acceleration.Y
                };
        }

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

        public static double[,] GetProcessNoise(double positionError, double velocityError, double accelerationError)
        {
            return Matrix.Diagonal(Dimension, new double[] { positionError, positionError, 
                                                             velocityError, velocityError,
                                                             accelerationError, accelerationError });
        }

        public static ConstantAcceleration2DModel Evaluate(ConstantAcceleration2DModel state, double[,] transitionMat)
        {
            var stateVector = state.ToArray().Multiply(transitionMat);
            return ConstantAcceleration2DModel.FromArray(stateVector);
        }
    }
}
