#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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
using System.Linq;
using MoreLinq;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// LINE2D match grouping algorithm.
    /// </summary>
    public class MatchClustering : Clustering<Match>
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
        /// <param name="minGroupWeight">
        /// Minimum group weight threshold. Group with less than <paramref name="minGroupWeight"/> will be discarded.
        /// <para>If the all weights are equal to one, the <paramref name="minGroupWeight"/> represents the minimum number of neighbors.</para>
        /// </param>
        /// <param name="minMatchAreaOverlap">Minimum bounding rectangle overlap area represented as percentage [0..1].</param>
        public MatchClustering(float minGroupWeight = 1f,float minMatchAreaOverlap = 0.3f)
           :base()
        {
            this.MinMatchAreaOverlap = minMatchAreaOverlap;
            compareByFunc = COMPARE_BY_SIZE;
        }

        /// <summary>
        /// Minimum bounding rectangle overlap area represented as percentage [0..1].
        /// </summary>
        public float MinMatchAreaOverlap { get; set; }

        /// <summary>
        /// Gets whether the two matches are adjacent.
        /// </summary>
        /// <param name="m1">First match.</param>
        /// <param name="m2">Second match.</param>
        /// <returns>True if two matches are adjacent, false otherwise.</returns>
        protected override bool AreDetectionsAdjacent(Match m1, Match m2)
        {
            return m1.BoundingRect.IntersectionPercent(m2.BoundingRect) >= MinMatchAreaOverlap;
        }

        /// <summary>
        /// Gets cluster representative.
        /// </summary>
        /// <param name="matches">Matches.</param>
        /// <param name="weights">Match weights - ignored because weights are incorporated into match itself.</param>
        /// <returns>Representative match.</returns>
        protected override Match GetRepresentative(IList<Match> matches, IList<float> weights)
        {
            var bestMatch = matches.MaxBy(compareByFunc);
            return bestMatch;
        }

        /// <summary>
        /// Groups near matches into a group.
        /// </summary>
        /// <param name="matches">The objects to group.</param>
        /// <param name="userCompareBy">User defined comparison function. If null, the default will be used.</param>
        /// <returns>Clusters.</returns>
        public IList<Cluster<Match>> Group(IList<Match> matches, Func<Match, double> userCompareBy = null)
        {
            this.compareByFunc = userCompareBy;
            if (this.compareByFunc == null)
                this.compareByFunc = COMPARE_BY_SIZE;

            return base.Group(matches, matches.Select(x=> x.Score).ToArray());
        }
    }
}
