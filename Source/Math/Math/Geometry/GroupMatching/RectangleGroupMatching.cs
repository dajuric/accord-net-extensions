
namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Represents the group matcher for rectangles.
    /// </summary>
    public class RectangleGroupMatching : GroupMatching<Rectangle>
    {
        /// <summary>
        /// Constructs new rectangle clustering.
        /// </summary>
        /// <param name="minimumNeighbors">Minimum number of neighbours per cluster.</param>
        /// <param name="threshold">Minmimal overlap between rectangles in cluster.</param>
        public RectangleGroupMatching(int minimumNeighbors = 1, double threshold = 0.2)
            : base(AverageRectangles, AreRectanglesNear, minimumNeighbors, threshold)
        { }

        /// <summary>
        /// Makes a representative rectangle.
        /// </summary>
        /// <param name="rects">Rectangles within cluster.</param>
        /// <returns>Cluster representative.</returns>
        public static Rectangle AverageRectangles(Rectangle[] rects)
        {
            Rectangle centroid = Rectangle.Empty;

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

        /// <summary>
        /// Determines whether rectangles should belong to one cluster or not.
        /// </summary>
        /// <param name="r1">First rectangle.</param>
        /// <param name="r2">Second rectangle.</param>
        /// <param name="threshold">Minmimal overlap between rectangles in cluster.</param>
        /// <returns>True if the rectangles are near, false otherwise.</returns>
        public static bool AreRectanglesNear(Rectangle r1, Rectangle r2, double threshold)
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
