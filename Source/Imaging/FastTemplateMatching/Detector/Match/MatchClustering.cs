using Accord.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MoreLinq;

namespace LINE2D
{
    public class MatchClustering : GroupMatching<Match>
    {
        public MatchClustering(int minimumNeighbors = 1, double threshold = 0.2)
            : base(AverageMatches, AreMatchesNear, minimumNeighbors, threshold)
        { }

        public static Match AverageMatches(Match[] matches)
        {
            var bestMatch = matches.MaxBy(delegate(Match a) { return a.BoundingRect.Size.Width * a.BoundingRect.Size.Height; });
            //var bestMatch = matches.MaxBy(m => m.Score);
            return bestMatch;
        }

        public static bool AreMatchesNear(Match m1, Match m2, double threshold)
        {
            return RectangleGroupMatching.AreRectanglesNear(m1.BoundingRect, m2.BoundingRect, threshold);
        }
    }
}
