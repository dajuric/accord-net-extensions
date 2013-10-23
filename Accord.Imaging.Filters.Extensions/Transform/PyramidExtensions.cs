using Accord.Core;
using System.Drawing;

namespace Accord.Imaging.Filters
{
    public static class PyramidExtensions //TODO: PyrDown without levels, PyrUp, public interface
    {
        const int DOWNSAMPLE_FACTOR = 2;

        public static Image<TColor, TDepth> PyrDown<TColor, TDepth>(this Image<TColor, TDepth> im, int level = 1)
            where TColor : IColor
            where TDepth : struct
        {
            if (level == 0)
                return im;

            double pyrScale = GetPyramidScale(level);

            Size newSize = new Size
            {
                Width = (int)(im.Width * pyrScale),
                Height = (int)(im.Height * pyrScale)
            };

           return ResizeNearsetNeighbur.Resize(im, newSize);
        }

        public static double GetPyramidScale(int levelDepth)
        {
            double factor = System.Math.Pow(DOWNSAMPLE_FACTOR, levelDepth);
            factor = 1 / factor;

            return factor;
        }

        public static double[] GetPyramidScales(int levels)
        {
            var scales = new double[levels + 1];
            scales[0] = 1;

            for (int i = 1; i <= levels; i++)
            {
                scales[i] = scales[i - 1] / DOWNSAMPLE_FACTOR;
            }

            return scales;
        }

        public static PointF PyrDown(this PointF pt, int level = 1)
        {
            float pyrScale = (float)GetPyramidScale(level);

            return new PointF
            {
                X = pt.X * pyrScale,
                Y = pt.Y * pyrScale
            };
        }

        public static PointF PyrUp(this PointF pt, int level = 1)
        {
            float pyrScale = (float)GetPyramidScale(level);

            return new PointF
            {
                X = pt.X / pyrScale,
                Y = pt.Y / pyrScale
            };
        }

        public static RectangleF PyrDown(this RectangleF rect, int level = 1)
        {
            float  pyrScale = (float)GetPyramidScale(level);

            return new RectangleF 
            {
                X = rect.X * pyrScale,
                Y=rect.Y * pyrScale,
                Width = rect.Width * pyrScale,
                Height = rect.Height * pyrScale
            };
        }

    }
}
