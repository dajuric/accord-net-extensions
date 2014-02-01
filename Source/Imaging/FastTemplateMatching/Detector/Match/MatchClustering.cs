using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MoreLinq;

namespace LINE2D
{
    public class MatchClustering : GroupMatching<Match>
    {
        /// <summary>
        /// Default comparer. Compare matches by size. Usually representative match is better by using this criteria.
        /// </summary>
        public static Func<Match, double> COMPARE_BY_SIZE = (m) => m.BoundingRect.Size.Width * m.BoundingRect.Size.Height;
        /// <summary>
        /// Compare matches by score.
        /// </summary>
        public static Func<Match, double> COMPARE_BY_SCORE = (m) => m.Score;

        Func<Match, double> compareByFunc = null;

        public MatchClustering(int minimumNeighbors = 1, double threshold = 0.2)
           :base(null, AreMatchesNear, minimumNeighbors, threshold)
        {
            compareByFunc = COMPARE_BY_SIZE;
            this.AverageFunc = AverageMatches;
        }

        private Match AverageMatches(Match[] matches)
        {
            var bestMatch = matches.MaxBy(compareByFunc);
            return bestMatch;
        }

        private static bool AreMatchesNear(Match m1, Match m2, double threshold)
        {
            return RectangleGroupMatching.AreRectanglesNear(m1.BoundingRect, m2.BoundingRect, threshold);
        }

        /// <summary>
        /// Groups near matches into a group.
        /// </summary>
        /// 
        /// <param name="strucutures">The structures to group.</param>
        /// <param name="userCompareBy">User defined comparasion function. If null, the default will be used.</param>
        /// 
        public GroupMatch<Match>[] Group(Match[] strucutures, Func<Match, double> userCompareBy = null)
        {
            this.compareByFunc = userCompareBy;
            if (this.compareByFunc == null)
                this.compareByFunc = COMPARE_BY_SIZE;

            return base.Group(strucutures);
        }
    }
}
