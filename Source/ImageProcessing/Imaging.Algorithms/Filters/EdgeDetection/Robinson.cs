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

using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Robinson's Edge Detector.
    /// </summary>
    public static class RobinsonEdgeDetectorExtensions
    {
        /// <summary>
        /// Robinson's Edge Detector.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.RobinsonEdgeDetector"/> for details.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="img">Image.</param>
        /// <returns>Edge image.</returns>
        public static TColor[,] Rectification<TColor>(this TColor[,] img)
            where TColor : struct, IColor
        {
            RobinsonEdgeDetector r = new RobinsonEdgeDetector();
            return img.ApplyFilter(r);
        }
    }
}
