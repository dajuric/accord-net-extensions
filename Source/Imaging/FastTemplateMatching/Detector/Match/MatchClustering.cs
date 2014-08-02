#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using Accord.Extensions.Math.Geometry;
using MoreLinq;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// LINE2D match grouping algorithm.
    /// </summary>
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

        /// <summary>
        /// Creates new LINE2D match clustering object.
        /// </summary>
        /// <param name="minimumNeighbors">Minimum number of objects for a cluster.</param>
        /// <param name="threshold">Min overlap.</param>
        public MatchClustering(int minimumNeighbors = 1, double threshold = 0.2)
           :base(null, areMatchesNear, minimumNeighbors, threshold)
        {
            compareByFunc = COMPARE_BY_SIZE;
            this.AverageFunc = getRepresentative;
        }

        private Match getRepresentative(Match[] matches)
        {
            var bestMatch = matches.MaxBy(compareByFunc);
            return bestMatch;
        }

        private static bool areMatchesNear(Match m1, Match m2, double threshold)
        {
            return RectangleGroupMatching.AreRectanglesNear(m1.BoundingRect, m2.BoundingRect, threshold);
        }

        /// <summary>
        /// Groups near matches into a group.
        /// </summary>
        /// 
        /// <param name="strucutures">The objects to group.</param>
        /// <param name="userCompareBy">User defined comparison function. If null, the default will be used.</param>
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
