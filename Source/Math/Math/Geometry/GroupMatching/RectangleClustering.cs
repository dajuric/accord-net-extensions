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

using System.Collections.Generic;
namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Represents the group matcher for rectangles.
    /// </summary>
    public class RectangleClustering : Clustering<Rectangle>
    {
        /// <summary>
        /// Constructs new rectangle clustering.
        /// </summary>
        /// <param name="minGroupWeight">
        /// Minimum group weight threshold. Group with less than <paramref name="minGroupWeight"/> will be discarded.
        /// <para>If the all weights are equal to one, the <paramref name="minGroupWeight"/> represents the minimum number of neighbors.</para>
        /// <param name="minRectangleAreaOverlap">Minimum bounding rectangle overlap area represented as percentage [0..1].</param>
        /// </param>
        public RectangleClustering(float minGroupWeight = 1f, float minRectangleAreaOverlap = 0.3f)
        {
            this.MinGroupWeight = minGroupWeight;
            this.MinRectangleAreaOverlap = minRectangleAreaOverlap;
        }

        /// <summary>
        /// Minimum bounding rectangle overlap area represented as percentage [0..1].
        /// </summary>
        public float MinRectangleAreaOverlap { get; set; }

        /// <summary>
        /// Calculates representative as weighted average.
        /// </summary>
        /// <param name="rects">Rectangles within cluster.</param>
        /// <param name="weights">Rectangles' importance.</param>
        /// <returns>Cluster representative.</returns>
        protected override Rectangle GetRepresentative(IList<Rectangle> rects, IList<float> weights)
        {
            RectangleF centroid = RectangleF.Empty;
            var weightsSum = 0f;

            for (int i = 0; i < rects.Count; i++)
            {
                centroid.X += rects[i].X * weights[i];
                centroid.Y += rects[i].Y * weights[i];
                centroid.Width += rects[i].Width * weights[i];
                centroid.Height += rects[i].Height * weights[i];

                weightsSum += weights[i];
            }

            centroid.X = centroid.X / weightsSum;
            centroid.Y = centroid.Y / weightsSum;
            centroid.Width = centroid.Width / weightsSum;
            centroid.Height = centroid.Height / weightsSum;

            return Rectangle.Round(centroid);
        }

        /// <summary>
        /// Determines whether rectangles should belong to one cluster or not.
        /// </summary>
        /// <param name="r1">First rectangle.</param>
        /// <param name="r2">Second rectangle.</param>
        /// <returns>True if the rectangles are near, false otherwise.</returns>
        protected override bool AreDetectionsAdjacent(Rectangle r1, Rectangle r2)
        {
            return r1.IntersectionPercent(r2) >= MinRectangleAreaOverlap;
        }
    }

}
