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
using AForge.Imaging.Filters;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for AForge imaging filters execution.
    /// </summary>
    public static class AForgeFilterProcessing
    {
        static AForgeFilterProcessing()
        { }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge filter.</param>
        /// <param name="inPlace">Execute in place or not. Please use this switch correctly as some filters may not be processed correctly.</param>
        /// <returns>Processed image. In case <paramref name="inPlace"/> is set to true, result is processed source image (can be discarded).</returns>
        private static Image<TColor, TDepth> ApplyFilter<TColor, TDepth, TFilter>(this Image<TColor, TDepth> img, TFilter filter, bool inPlace = false)
            where TColor: IColor
            where TDepth: struct
            where TFilter: IFilter
        {
            bool castOnly = img.CanCastToAForgeImage(); //we do not want to convert just cast
            if (castOnly == false)
                throw new Exception("AForge filters can not be applied to this image format!");

            Image<TColor, TDepth> dest = null;
            if (inPlace)
                dest = img;
            else
                dest = img.CopyBlank();

            //ToAforgeImage() will only cast image (cause the conversion path has been checked by CanCastToAForgeImage()
            filter.Apply(
                img.ToAForgeImage(copyAlways: false, failIfCannotCast: true), 
                dest.ToAForgeImage(copyAlways: false, failIfCannotCast: true)
                        );

            return dest;
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). As destination image size may be different from source in-place filtering is not allowed.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseFilter"/>.</param>
        public static Image<TColor, TDepth> ApplyFilter<TColor, TDepth>(this Image<TColor, TDepth> img, BaseFilter filter)
            where TColor : IColor
            where TDepth : struct
        {
            return ApplyFilter<TColor, TDepth, BaseFilter>(img, filter, false);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). 
        /// <see cref="BaseUsingCopyPartialFilter"/> must copy an image if in place operation is requested, so it was decided that in-place filtering is not allowed.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseUsingCopyPartialFilter"/>.</param>
        public static Image<TColor, TDepth> ApplyFilter<TColor, TDepth>(this Image<TColor, TDepth> img, BaseUsingCopyPartialFilter filter)
            where TColor : IColor
            where TDepth : struct
        {
            return ApplyFilter<TColor, TDepth, BaseUsingCopyPartialFilter>(img, filter, false);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). 
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseInPlaceFilter"/>.</param>
        /// <param name="inPlace">Execute in place or not. Please use this switch correctly as some filters may not be processed correctly.</param>
        public static Image<TColor, TDepth> ApplyFilter<TColor, TDepth>(this Image<TColor, TDepth> img, BaseInPlaceFilter filter, bool inPlace = false)
            where TColor : IColor
            where TDepth : struct
        {
            return ApplyFilter<TColor, TDepth, BaseInPlaceFilter>(img, filter, inPlace);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseInPlacePartialFilter"/>.</param>
        /// <param name="inPlace">Execute in place or not. Please use this switch correctly as some filters may not be processed correctly.</param>
        public static Image<TColor, TDepth> ApplyFilter<TColor, TDepth>(this Image<TColor, TDepth> img, BaseInPlacePartialFilter filter, bool inPlace = false)
            where TColor : IColor
            where TDepth : struct
        {
            return ApplyFilter<TColor, TDepth, BaseInPlacePartialFilter>(img, filter, inPlace);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). As destination image size may be different from source; in-place filtering is not allowed.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseTransformationFilter"/>.</param>
        public static Image<TColor, TDepth> ApplyFilter<TColor, TDepth>(this Image<TColor, TDepth> img, BaseTransformationFilter filter)
            where TColor : IColor
            where TDepth : struct
        {
            return ApplyFilter<TColor, TDepth, TColor>(img, filter);
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). As destination image size may be different from source; in-place filtering is not allowed.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="filter">AForge <see cref="BaseTransformationFilter"/>.</param>
        public static Image<TDstColor, TDepth> ApplyFilter<TSrcColor, TDepth, TDstColor>(this Image<TSrcColor, TDepth> img, BaseTransformationFilter filter)
            where TSrcColor : IColor
            where TDstColor: IColor
            where TDepth : struct
        {
            bool castOnly = img.CanCastToAForgeImage(); //we do not want to convert just cast
            if (castOnly == false)
                throw new Exception("AForge filters can not be applied to this image format!");

            //ToAforgeImage() will only cast image (cause the conversion path has been checked by CanCastOnlyToAForgeImage()
            var dest = filter.Apply(img.ToAForgeImage()).ToImage<TDstColor, TDepth>();
            return dest;
        }
    }
}
