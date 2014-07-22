using System;
using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains extensions for Kuwahara filter.
    /// </summary>
    public static class KuwaharaExtensions
    {
        /// <summary>
        /// Kuwahara filter.
        /// <para>Accord.NET internal call. See: <see cref="Accord.Imaging.Filters.Kuwahara"/> for details.</para>
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="size">the size of the kernel used in the Kuwahara filter. This should be odd and greater than or equal to five</param>
        /// <param name="blockSize">the size of each of the four inner blocks used in the Kuwahara filter. This is always half the <paramref name="size"/> minus one.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static Image<Gray, byte> Kuwahara(this Image<Gray, byte> img, int size = 5, int blockSize = 2, bool inPlace = false)
        {
            Kuwahara k = new Kuwahara();
            return img.ApplyFilter(k, inPlace);
        }
    }
}
