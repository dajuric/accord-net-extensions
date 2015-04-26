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
    /// Contains methods for variance calculation.
    /// </summary>
    public static class VarianceExtensions
    {
        /// <summary>
        /// <para>(Accord .NET internal call)</para>
        /// The Variance filter replaces each pixel in an image by its
        /// neighborhood variance. The end result can be regarded as an
        /// border enhancement, making the Variance filter suitable to
        /// be used as an edge detection mechanism.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="radius">The radius neighborhood used to compute a pixel's local variance.</param>
        /// <returns>Processed image.</returns>
        public static Gray<byte>[,] Variance(this Gray<byte>[,] img, int radius = 2)
        {
            Variance v = new Variance(radius);
            return img.ApplyFilter(v);
        }

        /// <summary>
        /// <para>(Accord .NET internal call)</para>
        /// The Variance filter replaces each pixel in an image by its
        /// neighborhood variance. The end result can be regarded as an
        /// border enhancement, making the Variance filter suitable to
        /// be used as an edge detection mechanism.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="radius">The radius neighborhood used to compute a pixel's local variance.</param>
        /// <returns>Processed image.</returns>
        public static TColor[,] Variance<TColor>(this TColor[,] img, int radius = 2)
            where TColor: struct, IColor3<byte>
        {
            Variance v = new Variance(radius);
            return img.ApplyFilter(v);
        }

        /// <summary>
        /// <para>(Accord .NET internal call)</para>
        /// The Variance filter replaces each pixel in an image by its
        /// neighborhood variance. The end result can be regarded as an
        /// border enhancement, making the Variance filter suitable to
        /// be used as an edge detection mechanism.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="radius">The radius neighborhood used to compute a pixel's local variance.</param>
        /// <returns>Processed image.</returns>
        public static Bgra<byte>[,] Variance(this Bgra<byte>[,] img, int radius = 2)
        {
            Variance v = new Variance(radius);
            return img.ApplyFilter(v);
        }
    }
}
