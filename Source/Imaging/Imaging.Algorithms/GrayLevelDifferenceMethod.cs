using Accord.Imaging;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Gray-Level Difference Method (GLDM) extensions.
    /// </summary>
    public static class GrayLevelDifferenceMethodExtensions
    {
        /// <summary>
        ///  Gray-Level Difference Method (GLDM).
        ///  <para>Computes an gray-level histogram of difference values between adjacent pixels in an image.</para>
        ///  <para>Accord.NET internal call. Please see: <see cref="Accord.Imaging.GrayLevelDifferenceMethod">Gray-Level Difference Method</see> for details.</para>
        /// </summary>
        /// <param name="image">The source image.</param>
        /// <param name="autoGray">Whether the maximum value of gray should be automatically computed from the image. </param>
        /// <param name="degree">The direction at which the co-occurrence should be found.</param>
        /// <returns>An histogram containing co-occurrences for every gray level in <paramref name="image"/>.</returns>
        public static int[] GrayLevelDifferenceMethod(this Image<Gray, byte> image, CooccurrenceDegree degree, bool autoGray = true)
        {
            GrayLevelDifferenceMethod gldm = new GrayLevelDifferenceMethod(degree, autoGray);
            var hist = gldm.Compute(image.ToAForgeImage(copyAlways: false, failIfCannotCast: true));

            return hist;
        }

    }
}
