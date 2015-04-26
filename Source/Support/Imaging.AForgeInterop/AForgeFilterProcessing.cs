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
using AForge.Imaging.Filters;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for AForge imaging filters execution.
    /// </summary>
    public static class AForgeFilterProcessing
    {
        /// <summary>
        /// Executes specified filter on an image (without using parallel processor).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge filter.</param>
        /// <param name="inPlace">Execute in place or not. Please use this switch correctly as some filters may not be processed correctly.</param>
        /// <returns>Processed image. In case <paramref name="inPlace"/> is set to true, result is processed source image (can be discarded).</returns>
        private static TColor[,] ApplyFilter<TColor, TFilter>(this TColor[,] img, TFilter filter, bool inPlace = false)
            where TColor: struct, IColor
            where TFilter: IFilter
        {
            TColor[,] dest = null;
            if (inPlace)
                dest = img;
            else
                dest = img.CopyBlank();

            using (var uImg = img.Lock())
            using (var uDest = dest.Lock())
            {
                filter.Apply(uImg.AsAForgeImage(), uDest.AsAForgeImage());
            }

            return dest;
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). As destination image size may be different from source in-place filtering is not allowed.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseFilter"/>.</param>
        public static TColor[,] ApplyFilter<TColor>(this TColor[,] img, BaseFilter filter)
            where TColor : struct, IColor
        {
            return ApplyFilter<TColor, BaseFilter>(img, filter, false);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). 
        /// <see cref="BaseUsingCopyPartialFilter"/> must copy an image if in place operation is requested, so it was decided that in-place filtering is not allowed.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseUsingCopyPartialFilter"/>.</param>
        public static TColor[,] ApplyFilter<TColor>(this TColor[,] img, BaseUsingCopyPartialFilter filter)
            where TColor : struct, IColor
        {
            return ApplyFilter<TColor, BaseUsingCopyPartialFilter>(img, filter, false);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). 
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseInPlaceFilter"/>.</param>
        /// <param name="inPlace">Execute in place or not. Please use this switch correctly as some filters may not be processed correctly.</param>
        public static TColor[,] ApplyFilter<TColor>(this TColor[,] img, BaseInPlaceFilter filter, bool inPlace = false)
            where TColor : struct, IColor
        {
            return ApplyFilter<TColor, BaseInPlaceFilter>(img, filter, inPlace);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseInPlacePartialFilter"/>.</param>
        /// <param name="inPlace">Execute in place or not. Please use this switch correctly as some filters may not be processed correctly.</param>
        public static TColor[,] ApplyFilter<TColor>(this TColor[,] img, BaseInPlacePartialFilter filter, bool inPlace = false)
            where TColor : struct, IColor
        {
            return ApplyFilter<TColor, BaseInPlacePartialFilter>(img, filter, inPlace);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). As destination image size may be different from source; in-place filtering is not allowed.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseTransformationFilter"/>.</param>
        public static TColor[,] ApplyBaseTransformationFilter<TColor>(this TColor[,] img, BaseTransformationFilter filter)
            where TColor : struct, IColor
        {
            return ApplyFilter<TColor, BaseTransformationFilter>(img, filter);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). As destination image size may be different from source; in-place filtering is not allowed.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseTransformationFilter"/>.</param>
        public static TDstColor[,] ApplyBaseTransformationFilter<TSrcColor, TDstColor>(this TSrcColor[,] img, BaseTransformationFilter filter)
            where TSrcColor : struct, IColor
            where TDstColor : struct, IColor
        {
            TDstColor[,] dest = new TDstColor[img.Height(), img.Width()];

            using (var uImg = img.Lock())
            using (var uDest = dest.Lock())
            {
                filter.Apply(uImg.AsAForgeImage(), uDest.AsAForgeImage());
            }

            return dest;
        }
    }
}
