using Accord.Imaging;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Gray-Level Run-Length Matrix extensions.
    /// </summary>
    public static class GrayLevelRunLengthMatrixExtensions
    {
        /// <summary>
        /// Computes the Gray-level Run-length for the given image source.
        /// </summary>
        /// <param name="grayLevelRunLengthMatrix">Gray-Level Run-Length Matrix.</param>
        /// <param name="source">The source image.</param>
        /// <returns>An array of run-length vectors containing level counts for every width pixel in <paramref name="source"/>.</returns>
        public static double[][] Compute(this GrayLevelRunLengthMatrix grayLevelRunLengthMatrix, Image<Gray, byte> source)
        {
            return grayLevelRunLengthMatrix.Compute(source.ToAForgeImage(copyAlways: false, failIfCannotCast: true));
        }
    }
}
