using System;
using PointF = AForge.Point;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extension methods for pyramid image calculation.
    /// </summary>
    public static class PyramidExtensions //TODO: PyrDown without levels, PyrUp, public interface
    {
        const int DOWNSAMPLE_FACTOR = 2;

        /// <summary>
        /// Resides the input image by <paramref name="level"/> * <see cref="DOWNSAMPLE_FACTOR"/>.
        /// <para>It is assumed that input image is already blurred.</para>
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="im">Input image.</param>
        /// <param name="level">Pyramid level. If zero an original image will be returned.</param>
        /// <returns></returns>
        public static Image<TColor, TDepth> PyrDown<TColor, TDepth>(this Image<TColor, TDepth> im, int level = 1)
            where TColor : IColor
            where TDepth : struct
        {
            if (level < 0)
                new ArgumentOutOfRangeException("level", "Level must be greater or equal than zero.");

            if (level == 0)
                return im;

            double pyrScale = GetPyramidScale(level);

            Size newSize = new Size
            {
                Width = (int)(im.Width * pyrScale),
                Height = (int)(im.Height * pyrScale)
            };

           return ResizeNearsetNeighbur.ResizeNN(im, newSize);
        }

        /// <summary>
        /// Gets pyramid scale for the specified level.
        /// </summary>
        /// <param name="levelDepth">Pyramid level.</param>
        /// <returns>Resize scale.</returns>
        public static double GetPyramidScale(int levelDepth)
        {
            double factor = System.Math.Pow(DOWNSAMPLE_FACTOR, levelDepth);
            factor = 1 / factor;

            return factor;
        }
    }
}
