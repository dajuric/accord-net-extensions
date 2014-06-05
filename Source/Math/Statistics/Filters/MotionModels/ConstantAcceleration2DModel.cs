using Accord.Math;
using Accord.Extensions.Math;
using System;
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
        public const int Dimension = 6;

        public PointF Position;
        public PointF Velocity;
        public PointF Acceleration;

        public ConstantAcceleration2DModel()
        {
            this.Position = default(PointF);
            this.Velocity = default(PointF);
            this.Acceleration = default(PointF);
        }

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

        public static double[,] GetPositionMeasurementMatrix()
        {
            return new double[,] //just pick point coordinates for an observation [2 x 6] (look at used state model)
                { 
                   //X,  vX, aX, Y,  vY  aY  (look at ConstantAcceleration2DModel)
                    {1,  0,  0,  0,  0,  0}, //picks X
                    {0,  0,  0,  1,  0,  0}  //picks Y
                };
        }

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

        public static ConstantAcceleration2DModel Evaluate(ConstantAcceleration2DModel state, double[,] transitionMat, double[,] procesNoiseMat = null)
        {
            var stateVector = transitionMat.Multiply(state.ToArray());

            if (procesNoiseMat != null)
                stateVector = stateVector.Multiply(procesNoiseMat); //TODO - critical: verify if this is correct

            return ConstantAcceleration2DModel.FromArray(stateVector);
        }

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
