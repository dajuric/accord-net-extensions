using System;
using System.Collections.Generic;

namespace Accord.Extensions.Imaging.IntegralImage
{
    static class IntegralImageExtensionsBase
    {
        delegate void MakeIntegralFunc(IImage src, IImage dest);
        static Dictionary<Type, Tuple<Type, MakeIntegralFunc>> makeIntegralFuncs;

        static IntegralImageExtensionsBase()
        {
            makeIntegralFuncs = new Dictionary<Type, Tuple<Type, MakeIntegralFunc>>();

            makeIntegralFuncs.Add(typeof(byte), new Tuple<Type, MakeIntegralFunc>(typeof(int), makeIntegral_Byte));
            makeIntegralFuncs.Add(typeof(float), new Tuple<Type, MakeIntegralFunc>(typeof(float), makeIntegral_Float));
            makeIntegralFuncs.Add(typeof(double), new Tuple<Type, MakeIntegralFunc>(typeof(double), makeIntegral_Double));
        }

        /// <summary>
        /// Calculates integral image. The dimensions of integral image are (width + 1, height + 1).
        /// Use extension functions to access integral data with ease.
        /// </summary>
        /// <returns>Integral image.</returns>
        internal static IImage MakeIntegral(IImage img)
        {
            Tuple<Type, MakeIntegralFunc> makeIntegral = null;
            if (makeIntegralFuncs.TryGetValue(img.ColorInfo.ChannelType, out makeIntegral) == false)
                throw new Exception(string.Format("MakeIntegral function can not process image of color depth type {0}", img.ColorInfo.ChannelType));

            var destColor = ColorInfo.GetInfo(img.ColorInfo.ColorType, makeIntegral.Item1);
            IImage dest = Image.Create(destColor, img.Width + 1, img.Height + 1);

            makeIntegral.Item2(img, dest);
            return dest;
        }

        #region MakeIntegral specific funcs

        unsafe static void makeIntegral_Byte(IImage src, IImage dest)
        {
            byte* srcPtr = (byte*)src.ImageData;
            int* dstPtr = (int*)dest.GetData(1, 1);

            int srcStride = src.Stride;
            int dstStride = dest.Stride;

            int width = src.Width;
            int height = src.Height;

            for (int row = 0; row < height; row++)
            {
                int* dstRowPtr = dstPtr;
                for (int col = 0; col < width; col++)
                {
                    var val = srcPtr[col];

                    var a = (dstRowPtr - 1);                          //(x-1, y)
                    var b = (int*)((byte*)dstRowPtr - dstStride);   //(x, y-1)
                    var c = (b - 1);                                  //(x-1, y-1)

                    *dstRowPtr = val + *a + *b - *c;
                    dstRowPtr++;
                }

                srcPtr = (byte*)((byte*)srcPtr + srcStride);
                dstPtr = (int*)((byte*)dstPtr + dstStride);
            }
        }

        unsafe static void makeIntegral_Float(IImage src, IImage dest)
        {
            float* srcPtr = (float*)src.ImageData;
            float* dstPtr = (float*)dest.GetData(1, 1);

            int srcStride = src.Stride;
            int dstStride = dest.Stride;

            int width = src.Width;
            int height = src.Height;

            for (int row = 0; row < height; row++)
            {
                float* dstRowPtr = dstPtr;
                for (int col = 0; col < width; col++)
                {
                    var val = srcPtr[col];

                    var a = (dstRowPtr - 1);                          //(x-1, y)
                    var b = (float*)((byte*)dstRowPtr - dstStride);   //(x, y-1)
                    var c = (b - 1);                                  //(x-1, y-1)

                    *dstRowPtr = val + *a + *b - *c;
                    dstRowPtr++;
                }

                srcPtr = (float*)((byte*)srcPtr + srcStride);
                dstPtr = (float*)((byte*)dstPtr + dstStride);
            }
        }


        unsafe static void makeIntegral_Double(IImage src, IImage dest)
        {
            double* srcPtr = (double*)src.ImageData;
            double* dstPtr = (double*)dest.GetData(1, 1);

            int srcStride = src.Stride;
            int dstStride = dest.Stride;

            int width = src.Width;
            int height = src.Height;

            for (int row = 0; row < height; row++)
            {
                double* dstRowPtr = dstPtr;
                for (int col = 0; col < width; col++)
                {
                    var val = srcPtr[col];

                    var a = (dstRowPtr - 1);                          //(x-1, y)
                    var b = (double*)((byte*)dstRowPtr - dstStride);   //(x, y-1)
                    var c = (b - 1);                                  //(x-1, y-1)

                    *dstRowPtr = val + *a + *b - *c;
                    dstRowPtr++;
                }

                srcPtr = (double*)((byte*)srcPtr + srcStride);
                dstPtr = (double*)((byte*)dstPtr + dstStride);
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides extensions for gray integral image of type <see cref="System.Byte"/>.
    /// </summary>
    public static class IntegralImageExtensionsColorByte
    {
        /// <summary>
        /// Calculates integral image. The dimensions of integral image are (width + 1, height + 1).
        /// Use extension functions to access integral data with ease.
        /// <see cref="GetSum"/>
        /// </summary>
        /// <returns>Integral image.</returns>
        public static Image<Gray, int> MakeIntegral(this Image<Gray, byte> img)
        {
            return IntegralImageExtensionsBase.MakeIntegral((IImage)img) as Image<Gray, int>;
        }

        /// <summary>
        /// Gets sum under image region (requires only 4 lookups).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="x">Location X.</param>
        /// <param name="y">Location Y.</param>
        /// <param name="width">Region width.</param>
        /// <param name="height">Region height.</param>
        /// <returns>Sum of pixels under specified region.</returns>
        public unsafe static int GetSum(this Image<Gray, int> img, int x, int y, int width, int height)
        {
            int* a = (int*)img.GetData(y, x);                 //(x, y)
            int* c = (int*)((byte*)a + img.Stride * height);  //(x, y+height)
            int* b = c + width;                               //(x+width, y+height)
            int* d = a + width;                               //(x+width, y)

            return *a + *b - *c - *d;
        }
    }

    /// <summary>
    /// Provides extensions for gray integral image of type <see cref="System.Single"/>.
    /// </summary>
    public static class IntegralImageExtensionsColorFloat
    {
        /// <summary>
        /// Calculates integral image. The dimensions of integral image are (width + 1, height + 1).
        /// Use extension functions to access integral data with ease.
        /// <see cref="GetSum"/>
        /// </summary>
        /// <returns>Integral image.</returns>
        public static Image<Gray, float> MakeIntegral(this Image<Gray, float> img)
        {
            return IntegralImageExtensionsBase.MakeIntegral((IImage)img) as Image<Gray, float>;
        }

        /// <summary>
        /// Gets sum under image region (requires only 4 lookups).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="x">Location X.</param>
        /// <param name="y">Location Y.</param>
        /// <param name="width">Region width.</param>
        /// <param name="height">Region height.</param>
        /// <returns>Sum of pixels under specified region.</returns>
        public unsafe static float GetSum(this Image<Gray, float> img, int x, int y, int width, int height)
        {
            float* a = (float*)img.GetData(y, x); //(x, y)
            float* c = (float*)((byte*)a + img.Stride * height);  //(x, y+height)
            float* b = c + width;               //(x+width, y+height)
            float* d = a + width;               //(x+width, y)

            return *a + *b - *c - *d;
        }
    }

    /// <summary>
    /// Provides extensions for gray integral image of type <see cref="System.Double"/>.
    /// </summary>
    public static class IntegralImageExtensionsColorDouble
    {
        /// <summary>
        /// Calculates integral image. The dimensions of integral image are (width + 1, height + 1).
        /// Use extension functions to access integral data with ease.
        /// <see cref="GetSum"/>
        /// </summary>
        /// <returns>Integral image.</returns>
        public static Image<Gray, double> MakeIntegral(this Image<Gray, double> img)
        {
            return IntegralImageExtensionsBase.MakeIntegral((IImage)img) as Image<Gray, double>;
        }

        /// <summary>
        /// Gets sum under image region (requires only 4 lookups).
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="x">Location X.</param>
        /// <param name="y">Location Y.</param>
        /// <param name="width">Region width.</param>
        /// <param name="height">Region height.</param>
        /// <returns>Sum of pixels under specified region.</returns>
        public unsafe static double GetSum(this Image<Gray, double> img, int x, int y, int width, int height)
        {
            double* a = (double*)img.GetData(y, x); //(x, y)
            double* c = (double*)((byte*)a + img.Stride * height);  //(x, y+height)
            double* b = c + width;               //(x+width, y+height)
            double* d = a + width;               //(x+width, y)

            return *a + *b - *c - *d;
        }
    }
}
