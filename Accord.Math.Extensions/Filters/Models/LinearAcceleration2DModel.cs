using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge;
using Accord.Math;

namespace Accord.Statistics.Filters
{
    /// <summary>
    /// Linear acceleration model for 2D case.
    /// Model is constructed as following:
    /// p(i) = p(i-1) + (& Delta t) * v(i-1) + ((& Delta t)^2 / 2) * a(t-1);
    /// v(i) = v(i-1) + (& Delta t) * a(t-1);
    /// a(t) = a(t-1);
    /// <br/>
    /// If p = [x y]^T and v = [(& Delta x) / (& Delta t)  (& Delta y) / (& Delta t)]^T then 
    /// state transition matrix could be written as <see cref="TransitionMatrix"/>
    /// </summary>
    public class LinearAcceleration2DModel
    {
        public static int Dimension = 4;

        public static double[,] GetInitialState(Point initialPosition, Point initalSpeed)
        {
            return new double[] 
            {
                initialPosition.X,
                initialPosition.Y,
                initalSpeed.X,
                initalSpeed.Y
            }
            .Transpose();
        }

        public static double[,] GetTransitionMatrix(double timeInterval)
        {
            return new double[,] 
                { 
                    {1, 0, timeInterval, 0},
                    {0, 1, 0, timeInterval},
                    {0, 0, 1, 0},
                    {0, 0, 0, 1}
                };
        }

        public static double[,] GetControlMatrix(double timeInterval)
        {
            var accelerationPositionFactor = timeInterval * timeInterval / 2;
            var accelerationVelocityFactor = timeInterval;

            return new double[,] 
                {
                    {accelerationPositionFactor},
                    {accelerationPositionFactor},
                    {accelerationVelocityFactor},
                    {accelerationVelocityFactor}
                };
        }

        public static double[,] GetProcessNoiseCovariance(double processNoise, double timeInterval)
        {
            var controlVector = GetControlMatrix(timeInterval).GetColumn(0);
            var influenceMatrix = Matrix.Identity(Dimension).MultiplyByDiagonal(controlVector);
          
            return influenceMatrix.Multiply(processNoise * processNoise);
        }
    }
}
