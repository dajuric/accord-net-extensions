using Accord.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
    public static class SaveExtensions
    {
        /// <summary>
        /// Save an image. Uses <see cref="ToBitmap"/>.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="filename">File name</param>
        public static void Save<TColor>(this Image<TColor, byte> image, string filename)
            where TColor : IColor3
        {
            Save<TColor, byte>(image, filename);
        }

        /// <summary>
        /// Save an image. Uses <see cref="ToBitmap"/>.
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
