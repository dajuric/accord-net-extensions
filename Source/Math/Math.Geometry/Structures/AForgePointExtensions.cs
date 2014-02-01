using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Math.Geometry
{
    public static class DoublePointExtensions
    {
        public static DoublePoint UpScale(this DoublePoint p, int levels, double factor = 2)
        {
            double upscaleFactor = System.Math.Pow(factor, levels);

            return new DoublePoint
            {
                X = p.X * upscaleFactor,
                Y = p.Y * upscaleFactor
            };
        }

        public static DoublePoint DownScale(this DoublePoint p, int levels, double factor = 2)
        {
            double downscaleFactor = 1 / System.Math.Pow(factor, levels);

            return new DoublePoint
            {
                X = p.X * downscaleFactor,
                Y = p.Y * downscaleFactor
            };
        }


        public static Point UpScale(this Point p, int levels = 1, double factor = 2)
        {
            var upscaleFactor = (float)System.Math.Pow(factor, levels);

            return new Point
            {
                X = p.X * upscaleFactor,
                Y = p.Y * upscaleFactor
            };
        }

        public static Point DownScale(this Point p, int levels = 1, double factor = 2)
        {
            var downscaleFactor = (float)(1 / System.Math.Pow(factor, levels));

            return new Point
            {
                X = p.X * downscaleFactor,
                Y = p.Y * downscaleFactor
            };
        }


        public static IntPoint Floor(this Point p)
        {
            return new IntPoint
            {
                X = (int)p.X,
                Y = (int)p.Y
            };
        }

        public static float DistanceTo(this Point a, Point b)
        {
            return (float)System.Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public static Point Normalize(this Point p)
        {
            var norm = p.EuclideanNorm();
            var pt = new Point 
            {
                X = p.X / norm,
                Y = p.Y / norm
            };

            return pt;
        }

        public static Point Swap(this Point p)
        {
            return new Point 
            { 
                X = p.Y,
                Y = p.X
            };
        }
    }
}
