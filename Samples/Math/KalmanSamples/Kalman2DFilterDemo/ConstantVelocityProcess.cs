#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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
using System.Drawing;
using Accord.Extensions.Statistics.Filters;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using PointF = AForge.Point;

namespace Kalman2DFilterDemo
{
    public class ConstantVelocityProcess
    {
        public static Size WorkingArea = new Size(100, 100);
        public static float TimeInterval = 1;

        NormalDistribution normalDistribution = new NormalDistribution(0, 0.2);
        Random rand = new Random();

        ConstantVelocity2DModel initialState;
        ConstantVelocity2DModel currentState;

        public ConstantVelocityProcess()
        {
            currentState = new ConstantVelocity2DModel
            {
                Position = new PointF(50, 1),
                Velocity = new PointF(0.3f, 0.3f)
            };

            initialState = currentState;
        }

        public void GoToNextState(out bool doneFullCycle)
        {
            Func<PointF, bool> isBorder = (point) =>
            {
                return point.X <= 0 || point.X >= WorkingArea.Width ||
                       point.Y <= 0 || point.Y >= WorkingArea.Height;
            };

            doneFullCycle = false;
            var prevPos = currentState.Position;
            var speed = currentState.Velocity;

            if (isBorder(currentState.Position))
            {
                var temp = speed.X;
                speed.X = -speed.Y;
                speed.Y = temp;

                if (speed.Equals(initialState.Velocity)) doneFullCycle = true;
            }

            var nextState = new ConstantVelocity2DModel
            {
                Position = new PointF
                {
                    X = prevPos.X + speed.X * TimeInterval,
                    Y = prevPos.Y + speed.Y * TimeInterval
                },

                Velocity = speed
            };

            currentState = nextState;
        }

        public ConstantVelocity2DModel GetNoisyState(double accelerationNoise)
        {
            var processNoiseMat = ConstantVelocity2DModel.GetProcessNoise(accelerationNoise);
            var noise = normalDistribution.Generate(ConstantVelocity2DModel.Dimension).Multiply(processNoiseMat);
            
            return new ConstantVelocity2DModel
            {
                Position = new PointF 
                {
                    X = currentState.Position.X + (float)noise[0],
                    Y = currentState.Position.Y + (float)noise[2]
                },

                Velocity = new PointF
                {
                    X = currentState.Velocity.X + (float)noise[1],
                    Y = currentState.Velocity.Y + (float)noise[3]
                }
            };
        }

        public PointF TryGetNoisyMeasurement(double measurementNoise, out bool isSuccess, double missingMeasurementProbability = 0.2)
        {
            isSuccess = rand.NextDouble() > missingMeasurementProbability;
            if (!isSuccess)
                return new PointF();

            return new PointF 
            {
                X = currentState.Position.X + (float)normalDistribution.Generate() * (float)measurementNoise,
                Y = currentState.Position.Y + (float)normalDistribution.Generate() * (float)measurementNoise
            };
        }

       
    }
}
