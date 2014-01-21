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
            :this(position.X, position.Y, radius)
        {}

        public CircleF(float x, float y, float radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
        }
    }

    public struct Circle
    {
        public int X;
        public int Y;
        public int Radius;

        public Circle(Point position, int radius)
            :this(position.X, position.Y, radius)
        {}

         public Circle(int x, int y, int radius)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
        }
    }
}
