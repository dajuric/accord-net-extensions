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

using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Extensions.Imaging;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Rectification filter.
    /// </summary>
    public static class RectificationExtensions
    {
        /// <summary>
        /// Rectification filter for projective transformation.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.Rectification"/> for details.</para>
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="homography">The homography matrix used to map a image passed to the filter to the overlay image.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces.</param>
        /// <returns>Rectified image.</returns>
        public static Bgra<byte>[,] Rectification(this Bgra<byte>[,] img, double[,] homography, Bgra<byte> fillColor)
        {
            Rectification r = new Rectification(homography);
            r.FillColor = fillColor.ToColor();

            return img.ApplyBaseTransformationFilter(r);
        }

        /// <summary>
        /// Rectification filter for projective transformation.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.Rectification"/> for details.</para>
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="homography">The homography matrix used to map a image passed to the filter to the overlay image.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces.</param>
        /// <returns>Rectified image.</returns>
        public static Bgr<byte>[,] Rectification(this Bgr<byte>[,] img, double[,] homography, Bgr<byte> fillColor)
        {
            Rectification r = new Rectification(homography);
            r.FillColor = fillColor.ToColor();
         
            return img.ApplyBaseTransformationFilter(r);
        }

        /// <summary>
        /// Rectification filter for projective transformation.
        /// <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.Filters.Rectification"/> for details.</para>
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="homography">The homography matrix used to map a image passed to the filter to the overlay image.</param>
        /// <param name="fillColor">The filling color used to fill blank spaces.</param>
        /// <returns>Rectified image.</returns>
        public static Gray<byte>[,] Rectification(this Gray<byte>[,] img, double[,] homography, Gray<byte> fillColor) 
        {
            Rectification r = new Rectification(homography);
            r.FillColor = fillColor.ToColor();

            return img.ApplyBaseTransformationFilter(r);
        }
    }
}
