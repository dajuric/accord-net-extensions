using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Accord.Extensions.Statistics.Filters;
using PointF = AForge.Point;
using Accord.Statistics.Distributions.Univariate;

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

        public ConstantVelocity2DModel GetNoisyState(double positionNoise, double velocityNoise)
        {
            return new ConstantVelocity2DModel
            {
                Position = new PointF 
                {
                    X = currentState.Position.X + (float)normalDistribution.Generate() * (float)positionNoise,
                    Y = currentState.Position.Y + (float)normalDistribution.Generate() * (float)positionNoise
                },

                Velocity = new PointF
                {
                    X = currentState.Velocity.X + (float)normalDistribution.Generate() * (float)velocityNoise,
                    Y = currentState.Velocity.Y + (float)normalDistribution.Generate() * (float)velocityNoise
                }
            };
        }

        public PointF TryGetNoisyMeasurement(double measurementNoise, out bool isSuccess)
        {
            isSuccess = rand.NextDouble() > 0.2;
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
