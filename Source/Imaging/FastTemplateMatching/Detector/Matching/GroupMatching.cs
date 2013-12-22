using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINE2D
{
    public class GroupMatch<T>
    {
        public int Neighbours { get { return this.Detections.Length; } }
        public T[] Detections { get; set; }
        public T AverageDetection { get; set; }
    }

    /// <summary>
    ///   Group matching algorithm for detection region averging.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be seen as a post-processing filter. Its goal is to
    ///   group near or contained regions together in order to produce more
    ///   robust and smooth estimates of the detected regions.
    /// </remarks>
    /// 
    public class GroupMatching<T>
    {
        private double threshold;
        private int classCount;

        private int minNeighbors;

        private int[] labels;
        private int[] equals;

        /// <summary>
        ///   Creates a new <see cref="GroupMatching"/> object.
        /// </summary>
        /// <param name="averageFunc">
        ///   The function for the averaging ner strucutres.</param>
        ///<param name="nearFunc">
        ///   The function which compares structures' distance.</param>
        /// <param name="minimumNeighbors">
        ///   The minimum number of neighbors needed to keep a detection. If a rectangle
        ///   has less than this minimum number, it will be discarded as a false positive.</param>
        /// <param name="threshold">
        ///   The minimum distance threshold to consider two structures as neighbors.
        ///   Default is 0.2.</param>
        /// 
        public GroupMatching(Average averageFunc, Near nearFunc, int minimumNeighbors = 1, double threshold = 0.2)
        {
            this.AverageFunc = averageFunc;
            this.NearFunc = nearFunc;

            this.minNeighbors = minimumNeighbors;
            this.threshold = threshold / 2.0;
        }

        /// <summary>
        ///   Gets or sets the minimum number of neighbors necessary to keep a detection.
        ///   If a rectangle has less neighbors than this number, it will be discarded as
        ///   a false positive.
        /// </summary>
        /// 
        public int MinimumNeighbors
        {
            get { return minNeighbors; }
            set
            {
                if (minNeighbors < 0)
                    throw new ArgumentOutOfRangeException("value", "Value must be equal to or higher than zero.");
                minNeighbors = value;
            }
        }

        /// <summary>
        ///   Groups possibly near rectangles into a smaller
        ///   set of distinct and averaged rectangles.
        /// </summary>
        /// 
        /// <param name="strucutures">The structures to group.</param>
        /// 
        public virtual GroupMatch<T>[] Group(T[] strucutures)
        {
            // Start by classifying rectangles according to distance
            classify(strucutures); // assign label for near rectangles

            int[] neighborCount;

            // Average the rectangles contained in each labelled group
            GroupMatch<T>[] output = average(strucutures);

            // Check supression
            if (minNeighbors > 0)
            {
                // Discard weak rectangles which don't have enough neighbors
                var filter = output.Where(x => x.Neighbours >= minNeighbors);
                return filter.ToArray();
            }

            return output;
        }

        public delegate T Average(T[] structures);
        public Average AverageFunc { get; set; }

        /// <summary>
        ///   Averages rectangles which belongs to the
        ///   same class (have the same class label)
        /// </summary>
        private GroupMatch<T>[] average(T[] structures)
        {
            GroupMatch<T>[] groupMatches = new GroupMatch<T>[classCount];

            var zip = structures.Zip(labels, (s, l) => new { S = s, L = l });
            var groups = from z in zip
                         group z by z.L into g
                         orderby g.Key
                         select new { Label = g.Key, Group = g.Select(x => x.S).ToArray(), Count = g.Count() };

            foreach (var g in groups)
            {
                T centroid = AverageFunc(g.Group);

                groupMatches[g.Label] = new GroupMatch<T> { AverageDetection = centroid, Detections = g.Group };
            }

            return groupMatches;
        }

        /// <summary>
        ///   Detects rectangles which are near and 
        ///   assigns similar class labels accordingly.
        /// </summary>
        /// 
        private void classify(T[] structures)
        {
            equals = new int[structures.Length];
            for (int i = 0; i < equals.Length; i++)
                equals[i] = -1;

            labels = new int[structures.Length];
            for (int i = 0; i < labels.Length; i++)
                labels[i] = i;

            classCount = 0;

            // If two strucutres are near, or contained in
            // each other, merge then in a single rectangle
            for (int i = 0; i < structures.Length - 1; i++)
            {
                for (int j = i + 1; j < structures.Length; j++)
                {
                    if (NearFunc(structures[i], structures[j], threshold))
                        merge(labels[i], labels[j]);
                }
            }

            // Count the number of classes and centroids
            int[] centroids = new int[structures.Length];
            for (int i = 0; i < centroids.Length; i++)
                if (equals[i] == -1) centroids[i] = classCount++;

            // Classify all rectangles with their labels
            for (int i = 0; i < structures.Length; i++)
            {
                int root = labels[i];
                while (equals[root] != -1)
                    root = equals[root];

                labels[i] = centroids[root];
            }
        }

        /// <summary>
        ///   Merges two labels.
        /// </summary>
        /// 
        private void merge(int label1, int label2)
        {
            int root1 = label1;
            int root2 = label2;

            // Get the roots associated with the two labels
            while (equals[root1] != -1) root1 = equals[root1];
            while (equals[root2] != -1) root2 = equals[root2];

            if (root1 == root2) // labels are already connected
                return;

            int minRoot, maxRoot;
            int labelWithMinRoot, labelWithMaxRoot;

            if (root1 > root2)
            {
                maxRoot = root1;
                minRoot = root2;

                labelWithMaxRoot = label1;
                labelWithMinRoot = label2;
            }
            else
            {
                maxRoot = root2;
                minRoot = root1;

                labelWithMaxRoot = label2;
                labelWithMinRoot = label1;
            }

            equals[maxRoot] = minRoot;

            for (int root = maxRoot + 1; root <= labelWithMaxRoot; root++)
            {
                if (equals[root] == maxRoot)
                    equals[root] = minRoot;
            }
        }

        /// <summary>
        ///   Checks if two rectangles are near.
        /// </summary>
        /// 
        public delegate bool Near(T t1, T t2, double threshold);
        public Near NearFunc { get; set; }
    }

    public class RectangleGroupMatching : GroupMatching<Rectangle>
    {
        public RectangleGroupMatching(int minimumNeighbors = 1, double threshold = 0.2)
            : base(averageRectangles, areRectanglesNear, minimumNeighbors, threshold)
        { }

        private static Rectangle averageRectangles(Rectangle[] rects)
        {
            Rectangle centroid = Rectangle.Empty;

            for (int i = 0; i < rects.Length; i++)
            {
                centroid.X += rects[i].X;
                centroid.Y += rects[i].Y;
                centroid.Width += rects[i].Width;
                centroid.Height += rects[i].Height;
            }

            centroid.X = (int)Math.Ceiling((float)centroid.X / rects.Length);
            centroid.Y = (int)Math.Ceiling((float)centroid.Y / rects.Length);
            centroid.Width = (int)Math.Ceiling((float)centroid.Width / rects.Length);
            centroid.Height = (int)Math.Ceiling((float)centroid.Height / rects.Length);

            return centroid;
        }

        private static bool areRectanglesNear(Rectangle r1, Rectangle r2, double threshold)
        {
            if (r1.Contains(r2) || r2.Contains(r1))
                return true;

            int minHeight = Math.Min(r1.Height, r2.Height);
            int minWidth = Math.Min(r1.Width, r2.Width);
            double delta = threshold * (minHeight + minWidth);

            return Math.Abs(r1.X - r2.X) <= delta
                && Math.Abs(r1.Y - r2.Y) <= delta
                && Math.Abs(r1.Right - r2.Right) <= delta
                && Math.Abs(r1.Bottom - r2.Bottom) <= delta;
        }
    }

}
