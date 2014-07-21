namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Generic image save extensions.
    /// <para>An image is first converted to <see cref="System.Drawing.Bitmap"/> and then saved.</para>
    /// </summary>
    public static class ImageSaveExtensions
    {
        /// <summary>
        /// Save an image.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="filename">File name</param>
        public static void Save<TColor>(this Image<TColor, byte> image, string filename)
            where TColor : IColor3
        {
            Save<TColor, byte>(image, filename);
        }

        /// <summary>
        /// Save an image. 
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="filename">File name</param>
        public static void Save(this Image<Gray, byte> image, string filename)
        {
            Save<Gray, byte>(image, filename);
        }

        private static void Save<TColor, TDepth>(this Image<TColor, TDepth> image, string filename)
            where TColor : IColor
            where TDepth : struct
        {
            image.ToBitmap().Save(filename); 
        }
    }
}
