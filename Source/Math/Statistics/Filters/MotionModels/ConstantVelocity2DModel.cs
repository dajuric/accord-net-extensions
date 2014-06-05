using Accord.Math;
using Accord.Extensions.Math;
using System;
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
        public const int Dimension = 4;

        public PointF Position;
        public PointF Velocity;

        public ConstantVelocity2DModel()
        {
            this.Position = default(PointF);
            this.Velocity = default(PointF);
        }

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

        public static double[,] GetPositionMeasurementMatrix()
        {
            return new double[,] //just pick point coordinates for an observation [2 x 6] (look at used state model)
                { 
                   //X,  vX, Y,  vY   (look at ConstantAcceleration2DModel)
                    {1,  0,  0,  0}, //picks X
                    {0,  0,  1,  0}  //picks Y
                };
        }

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

        public static ConstantVelocity2DModel Evaluate(ConstantVelocity2DModel state, double[,] transitionMat, double[,] procesNoiseMat = null)
        {
            var stateVector = transitionMat.Multiply(state.ToArray()); 

            if (procesNoiseMat != null)
                stateVector = stateVector.Multiply(procesNoiseMat); //TODO - critical: verify if this is correct

            return ConstantVelocity2DModel.FromArray(stateVector);
        }

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
