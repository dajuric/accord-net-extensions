using AForge.Imaging.Filters;

namespace Accord.Extensions.Imaging
{
    public static class ContrastStrechExtensions
    {
        /// <summary>
        /// Stretches intensity values in a linear way across full pixel range.
        /// </summary>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        public static Image<Gray, byte> StretchContrast(this Image<Gray, byte> im, bool inPlace = false)
        {
            return StretchContrast<Gray, byte>(im, inPlace);
        }

        /// <summary>
        /// Stretches intensity values in a linear way across full pixel range.
        /// </summary>
        /// <param name="inPlace">Process in place or make not. If in place is set to true, returned value may be discarded.</param>
        /// <returns>Corrected image.</returns>
        public static Image<TColor, byte> StretchContrast<TColor>(this Image<TColor, byte> im, bool inPlace = false)
            where TColor: IColor3
        {
            return StretchContrast<TColor, byte>(im, inPlace);
        }

        private static Image<TColor, TDepth> StretchContrast<TColor, TDepth>(this Image<TColor, TDepth> im, bool inPlace = false)
            where TColor : IColor
            where TDepth : struct
        {
            ContrastStretch conrastStrech = new ContrastStretch();
            return im.ApplyFilter(conrastStrech, inPlace);
        }
    }
}
