using PointF = AForge.Point;

namespace Accord.Extensions.Statistics.Filters
{
    public class PerspectiveProjectionCoordinateTransform
    {
        public double X;
        public double Y;
        public double ObjectWidth;
        public double Velocity;

        public static double CalculateVelocityMultiplierConstant(double objectWorldWidth, double focalLength, double meterToPixelMultiplier)
        {
            return meterToPixelMultiplier / (focalLength * objectWorldWidth);
        }

        public static PerspectiveProjectionCoordinateTransform Evaluate(double previousX, double previousY, double previousObjectWidth, 
                                                                        double velocity, double timeInterval, 
                                                                        double velocityMultiplierConst)
        {
            var newState = new PerspectiveProjectionCoordinateTransform();

            var multiplier = 1 / (1 + velocityMultiplierConst * velocity * previousObjectWidth * timeInterval);

            newState.X = previousX * multiplier;
            newState.Y = previousY * multiplier;
            newState.ObjectWidth = previousObjectWidth * multiplier;
            newState.Velocity = velocity;

            return newState;
        }

        public double[] ToArray()
        {
            return ToArray(this);
        }

        public static double[] ToArray(PerspectiveProjectionCoordinateTransform model)
        {
            return new double[] 
            {
                model.X,
                model.Y,
                model.ObjectWidth,
                model.Velocity
            };
        }

        public static PerspectiveProjectionCoordinateTransform FromArray(double[] arr)
        {
            return new PerspectiveProjectionCoordinateTransform
            {
               X = arr[0], Y = arr[1], ObjectWidth = arr[2], Velocity = arr[3]
            };
        }

        public static double[,] GetMeasurementMatrix()
        {
            return new double[,] 
            {
                //X,   Y,   width,   velocity 
                {1,    0,     0,       0}, //picks x
                {0,    1,     0,       0}, //picks y
                {0,    0,     1,       0}  //picks width
            };
        }

        public PerspectiveProjectionCoordinateTransform Clone()
        {
            return new PerspectiveProjectionCoordinateTransform 
            {
                X = this.X,
                Y = this.Y,
                ObjectWidth = this.ObjectWidth,
                Velocity = this.Velocity
            };
        }

        public double[,] EstimateTransitionMatrix(double velocityMultiplierConst, double delta = 1e-3)
        {
            return Math.MathExtensions.CalculateJacobian(x => 
            {
                var st = FromArray(x);
                var output = Evaluate(st.X, st.Y, st.ObjectWidth, st.Velocity, delta, velocityMultiplierConst);
                return output.ToArray();
            }, 
            this.ToArray(), delta);
        }
    }

    public static class PerspectiveProjectionCoordinateTransformExtensions
    {
        public static PointF GetCoordinate(this PerspectiveProjectionCoordinateTransform state)
        {
            return new PointF 
            {
                X = (float)state.X,
                Y = (float)state.Y
            };
        }

        public static PerspectiveProjectionCoordinateTransform Translate(this PerspectiveProjectionCoordinateTransform state, PointF offset)
        {
            var newState = state.Clone();
            newState.X += offset.X;
            newState.Y += offset.Y;

            return newState;
        }
    }
}
