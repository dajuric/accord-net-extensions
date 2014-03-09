
namespace Accord.Extensions.Math.Geometry
{
    public class RectangleGroupMatching : GroupMatching<Int32Rect>
    {
        public RectangleGroupMatching(int minimumNeighbors = 1, double threshold = 0.2)
            : base(AverageRectangles, AreRectanglesNear, minimumNeighbors, threshold)
        { }

        public static Int32Rect AverageRectangles(Int32Rect[] rects)
        {
            Int32Rect centroid = Int32Rect.Empty;

            for (int i = 0; i < rects.Length; i++)
            {
                centroid.X += rects[i].X;
                centroid.Y += rects[i].Y;
                centroid.Width += rects[i].Width;
                centroid.Height += rects[i].Height;
            }

            centroid.X = (int)System.Math.Ceiling((float)centroid.X / rects.Length);
            centroid.Y = (int)System.Math.Ceiling((float)centroid.Y / rects.Length);
            centroid.Width = (int)System.Math.Ceiling((float)centroid.Width / rects.Length);
            centroid.Height = (int)System.Math.Ceiling((float)centroid.Height / rects.Length);

            return centroid;
        }

        public static bool AreRectanglesNear(Int32Rect r1, Int32Rect r2, double threshold)
        {
            if (r1.Contains(r2) || r2.Contains(r1))
                return true;

            int minHeight = System.Math.Min(r1.Height, r2.Height);
            int minWidth = System.Math.Min(r1.Width, r2.Width);
            double delta = threshold * (minHeight + minWidth);

            return System.Math.Abs(r1.X - r2.X) <= delta
                && System.Math.Abs(r1.Y - r2.Y) <= delta
                && System.Math.Abs(r1.Right - r2.Right) <= delta
                && System.Math.Abs(r1.Bottom - r2.Bottom) <= delta;
        }
    }

}
