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
