using PointF = AForge.Point;

namespace Accord.Extensions.Imaging.Filters
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

            Int32Size newSize = new Int32Size
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
    }
}
