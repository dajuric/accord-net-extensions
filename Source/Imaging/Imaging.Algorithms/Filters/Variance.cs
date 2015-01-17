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
    static class VarianceExtensionsBase
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
        internal static Image<TColor, TDepth> Variance<TColor, TDepth>(this Image<TColor, TDepth> img, int radius = 2)
            where TColor : IColor
            where TDepth : struct
        {
            Variance v = new Variance(radius);
            return img.ApplyFilter(v);
        }
    }

    /// <summary>
    /// Contains methods for variance calculation.
    /// </summary>
    public static class VarianceExtensionsGray
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
        internal static Image<Gray, byte> Variance(this Image<Gray, byte> img, int radius = 2)
        {
            return VarianceExtensionsBase.Variance(img, radius);
        }
    }

    /// <summary>
    /// Contains methods for variance calculation.
    /// </summary>
    public static class VarianceExtensionsColor3
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
        internal static Image<TColor, byte> Variance<TColor>(this Image<TColor, byte> img, int radius = 2)
            where TColor : IColor3
        {
            return VarianceExtensionsBase.Variance(img, radius);
        }
    }

    /// <summary>
    /// Contains methods for variance calculation.
    /// </summary>
    public static class VarianceExtensionsBgra
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
        internal static Image<Bgra, byte> Variance(this Image<Bgra, byte> img, int radius = 2)
        {
            return VarianceExtensionsBase.Variance(img, radius);
        }
    }

}
