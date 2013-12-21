using PointF = AForge.Point;
using Point = AForge.IntPoint;

namespace Accord.Math.Geometry
{
    public struct CircleF
    {
        public float X;
        public float Y;
        public float Radius;

        public CircleF(PointF position, float radius)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Radius = radius;
        }
    }

    public struct Circle
    {
        public int X;
        public int Y;
        public int Radius;
    }
}
