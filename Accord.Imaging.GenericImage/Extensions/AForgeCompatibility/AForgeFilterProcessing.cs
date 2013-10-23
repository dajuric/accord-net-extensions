using Accord.Core;
using AForge.Imaging.Filters;
using System;
using System.Drawing;

namespace Accord.Imaging
{
    public static class AForgeFilterProcessing
    {
        static AForgeFilterProcessing()
        { }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor).
        /// </summary>
        /// <param name="filter">AForge filter.</param>
        /// <param name="inPlace">Execute in place or not. Please use this switch correctly as some filters may not be processed correctly.</param>
        /// <returns>Processed image. In case <see cref="inPlace"/> is set to true, result is processed source image (can be discarded).</returns>
        public static Image<TColor, TDepth> ApplyFilter<TColor, TDepth>(this Image<TColor, TDepth> img, IFilter filter, bool inPlace = false)
            where TColor: IColor
            where TDepth:struct
        {
            bool castOnly = img.CanCastToAForgeImage(); //we do not want to convert just cast
            if (castOnly == false)
                throw new Exception("AForge filters can not be applied to this image format!");

            Image<TColor, TDepth> dest = null;
            if (inPlace)
                dest = img;
            else
                dest = img.CopyBlank();

            //ToAforgeImage() will only cast image (cause the conversion path has been checked by CanCastOnlyToAForgeImage()
            filter.Apply(img.ToAForgeImage(), dest.ToAForgeImage());
            return dest;
        }

        /// <summary>
        /// Executes specified filter on an image (without using parallel processor). As destination image size may be different from source in-place filtering is not allowed.
        /// </summary>
        /// <param name="filter">AForge filter.</param>
        /// <param name="destImgSize">Destination image size. Useful if using filters which transform image. (e.g. resize)</param>
        /// <returns>Processed image.</returns>
        public static Image<TColor, TDepth> ApplyFilter<TColor, TDepth, TFilter>(this Image<TColor, TDepth> img, TFilter filter, Func<TFilter, Size> destImgSize)
            where TColor : IColor
            where TDepth : struct
            where TFilter: IFilter
        {
            bool castOnly = img.CanCastToAForgeImage(); //we do not want to convert just cast
            if (castOnly == false)
                throw new Exception("AForge filters can not be applied to this image format!");

            Image<TColor, TDepth> dest = new Image<TColor, TDepth>(destImgSize(filter));

            //ToAforgeImage() will only cast image (cause the conversion path has been checked by CanCastOnlyToAForgeImage()
            filter.Apply(img.ToAForgeImage(), dest.ToAForgeImage());
            return dest;
        }
    }
}
