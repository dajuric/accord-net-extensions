using System;
using System.Linq;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Represents collections of elements in one group.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    public class GroupMatch<T>
    {
        /// <summary>
        /// Gets the number of neighbors within a group.
        /// </summary>
        public int Neighbours { get { return this.Detections.Length; } }
        /// <summary>
        /// Gets the collection of elements within the group.
        /// </summary>
        public T[] Detections { get; set; }
        /// <summary>
        /// Gets the group representative chosen by average function in group matching algorithm.
        /// </summary>
        public T Representative { get; set; }
    }

    /// <summary>
    ///   Group matching algorithm for detection region averaging.
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
        ///   Creates a new group matcher.
        /// </summary>
        /// <param name="averageFunc">
        ///   The function for the averaging near structures.</param>
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
        ///   Groups possibly near structures into clusters.
        /// </summary>
        /// 
        /// <param name="strucutures">The structures to group.</param>
        /// 
        public virtual GroupMatch<T>[] Group(T[] strucutures)
        {
            // Start by classifying rectangles according to distance
            classify(strucutures); // assign label for near rectangles

            // Average the rectangles contained in each labeled group
            GroupMatch<T>[] output = average(strucutures);

            // Check suppression
            if (minNeighbors > 0)
            {
                // Discard weak rectangles which don't have enough neighbors
                var filter = output.Where(x => x.Neighbours >= minNeighbors);
                return filter.ToArray();
            }

            return output;
        }

        /// <summary>
        /// Delegate for averaging structures (finding the representative).
        /// </summary>
        /// <param name="structures">Structures.</param>
        /// <returns>Representative (average structure).</returns>
        public delegate T Average(T[] structures);

        /// <summary>
        /// Function for finding a group representative.
        /// </summary>
        public Average AverageFunc { get; set; }

        /// <summary>
        ///   Averages elements which belongs to the
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

                groupMatches[g.Label] = new GroupMatch<T> { Representative = centroid, Detections = g.Group };
            }

            return groupMatches;
        }

        /// <summary>
        ///   Detects elements which are near and 
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

            // If two structures are near, or contained in
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
        ///   Delegates for checking whether tow elements are near.
        /// </summary>
        /// <param name="t1">First element</param>
        /// <param name="t2">Second element</param>
        /// <param name="threshold">The minimum distance threshold to consider two structures as neighbors.</param>
        public delegate bool Near(T t1, T t2, double threshold);

        /// <summary>
        /// Gets or sets <see cref="Near"/> function.
        /// </summary>
        public Near NearFunc { get; set; }
    }
}
