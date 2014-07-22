using AForge.Imaging.Filters;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for contrast correction.
    /// </summary>
    public static class CorrectContrastExtensions
    {
        /// <summary>
        /// Adjusts pixels' contrast value by increasing RGB values of bright pixel and decreasing
        /// pixel values of dark pixels (or vise versa if contrast needs to be decreased).
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="factor">Factor which is used to adjust contrast. Factor values greater than
        /// 0 increase contrast making light areas lighter and dark areas darker. Factor values
        /// less than 0 decrease contrast - decreasing variety of contrast.</param>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        public static Image<Gray, byte> CorrectContrast(this Image<Gray, byte> im, int factor = 10, bool inPlace = false)
        {
            return CorrectContrast<Gray, byte>(im, factor, inPlace);
        }

        /// <summary>
        /// Adjusts pixels' contrast value by increasing RGB values of bright pixel and decreasing
        /// pixel values of dark pixels (or vise versa if contrast needs to be decreased).
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="factor">Factor which is used to adjust contrast. Factor values greater than
        /// 0 increase contrast making light areas lighter and dark areas darker. Factor values
        /// less than 0 decrease contrast - decreasing variety of contrast.</param>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        public static Image<TColor, byte> CorrectContrast<TColor>(this Image<TColor, byte> im, int factor = 10, bool inPlace = false)
            where TColor : IColor3
        {
            return CorrectContrast<TColor, byte>(im, factor, inPlace);
        }

        /// <summary>
        /// Adjusts pixels' contrast value by increasing RGB values of bright pixel and decreasing
        /// pixel values of dark pixels (or vise versa if contrast needs to be decreased).
        /// </summary>
        /// <param name="im">Image.</param>
        /// <param name="factor">Factor which is used to adjust contrast. Factor values greater than
        /// 0 increase contrast making light areas lighter and dark areas darker. Factor values
        /// less than 0 decrease contrast - decreasing variety of contrast.</param>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        private static Image<TColor, TDepth> CorrectContrast<TColor, TDepth>(this Image<TColor, TDepth> im, int factor = 10, bool inPlace = false)
            where TColor : IColor
            where TDepth : struct
        {
            ContrastCorrection conrastCorrection = new ContrastCorrection(factor);
            return im.ApplyFilter(conrastCorrection, inPlace);
        }
    }
}
