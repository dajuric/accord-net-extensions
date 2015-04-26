using Accord.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides basic image extensions for a two-dimensional array.
    /// </summary>
    public static class ArrayImageBasicExtensions
    {
        /// <summary>
        /// Gets image width.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>The image width.</returns>
        public static int Width<T>(this T[,] image)
        {
            return image.GetLength(1);
        }

        /// <summary>
        /// Gets image height.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>The image height.</returns>
        public static int Height<T>(this T[,] image)
        {
            return image.GetLength(0);
        }

        /// <summary>
        /// Gets image size.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="image">Image.</param>
        /// <returns>The image size.</returns>
        public static Size Size<T>(this T[,] image)
        {
            return new Size(image.Width(), image.Height());
        }

        /// <summary>
        /// Pins the array and returns the corresponding generic image.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="array">The array to lock.</param>
        /// <returns>The generic image which shares data with the pined array.</returns>
        public static Image<TColor> Lock<TColor>(this TColor[,] array)
            where TColor : struct
        {
            return Image<TColor>.Lock(array);
        }

        /// <summary>
        /// Pins the array and returns the corresponding generic image of a specified portion.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="array">The array to lock.</param>
        /// <param name="area">Working area.</param>
        /// <returns>The generic image which shares data with the pined array.</returns>
        public static Image<TColor> Lock<TColor>(this TColor[,] array, Rectangle area)
          where TColor : struct
        {
            return Image<TColor>.Lock(array).GetSubRect(area);
        }

        /// <summary>
        /// Sets the specified value for each element of the array.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="array">Array with value type elements.</param>
        /// <param name="value">Value to set.</param>
        public static void SetValue<T>(this T[,] array, T value)
            where T : struct
        {
            ParallelLauncher.Launch((thread) =>
            {
                array[thread.Y, thread.X] = value;
            },
            array.Width(), array.Height());
        }

        /// <summary>
        /// Sets the specified value for each element of the array.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="array">Array with value type elements.</param>
        /// <param name="value">Value to set.</param>
        /// <param name="area">Working area.</param>
        public static void SetValue<T>(this T[,] array, T value, Rectangle area)
            where T : struct
        {
            ParallelLauncher.Launch((thread) =>
            {
                array[thread.Y + area.Y, thread.X + area.X] = value;
            },
            area.Width, area.Height);
        }

        /// <summary>
        /// Sets all elements of the array to the default value.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="array">Array to clear.</param>
        public static void Clear<T>(this T[,] array)
            where T: struct
        {
            Array.Clear(array, 0, array.Length);
        }

        /// <summary>
        /// Performs deep cloning of the specified array.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="array">Array.</param>
        /// <returns>Cloned array.</returns>
        public static T[,] Clone<T>(this T[,] array)
            where T: struct
        {
            return (T[,])array.Clone();
        }

        /// <summary>
        /// Creates new array of the same size as the source array.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="array">Array.</param>
        /// <returns>New empty array.</returns>
        public static T[,] CopyBlank<T>(this T[,] array)
        {
            return new T[array.Height(), array.Width()];
        }

        /// <summary>
        /// Gets the element info of the specified array.
        /// </summary>
        /// <typeparam name="TColor">Element type.</typeparam>
        /// <param name="source">Array.</param>
        /// <returns>Array element info.</returns>
        public static ColorInfo ColorInfo<TColor>(this TColor[,] source)
            where TColor: struct
        {
            return Accord.Extensions.Imaging.ColorInfo.GetInfo<TColor>();
        }

        /// <summary>
        /// Calculates image stride for the specified alignment.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="allignment">Data alignment.</param>
        /// <returns>Image stride.</returns>
        public static int CalculateStride<TImage>(this TImage image, int allignment = 4)
            where TImage: IImage
        {
            int stride = image.Width * image.ColorInfo.Size;

            if (allignment != 0 &&
                stride % allignment != 0)
            {
                stride += (allignment - (stride % allignment));
            }

            return stride;
        }

        /// <summary>
        /// Two source filter operation.
        /// </summary>
        /// <param name="sourceA">First image.</param>
        /// <param name="sourceB">Second image.</param>
        /// <param name="destination">Destination image.</param>
        public delegate void UnsafeTwoSourceFilterFunc(IImage sourceA, IImage sourceB, IImage destination);

        /// <summary>
        /// Executes the user defined two source filter operation.
        /// </summary>
        /// <typeparam name="TColor">Image color.</typeparam>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="func">User defined operation.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>he result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] Calculate<TColor>(this TColor[,] imageA, TColor[,] imageB, UnsafeTwoSourceFilterFunc func, bool inPlace = false)
            where TColor : struct
        {
            TColor[,] dest = imageA;
            if (!inPlace)
                dest = imageA.CopyBlank();

            using (var uImg = imageA.Lock())
            using (var uImg2 = imageB.Lock())
            using (var uDest = dest.Lock())
            {
                func(uImg, uImg2, uDest);
            }

            return dest;
        }

        /// <summary>
        /// Executes the user defined two source filter operation.
        /// </summary>
        /// <typeparam name="TColor">Image color.</typeparam>
        /// <param name="imageA">First image.</param>
        /// <param name="imageB">Second image.</param>
        /// <param name="func">User defined operation.</param>
        /// <param name="inPlace">If true the result is going to be stored in the first image. If false a new image is going to be created.</param>
        /// <returns>he result image. If <paramref name="inPlace"/> is set to true, the return value can be discarded.</returns>
        public static TColor[,] Calculate<TColor>(this TColor[,] imageA, TColor[,] imageB, TwoSourceFilterFunc<TColor> func, bool inPlace = false)
            where TColor : struct
        {
            TColor[,] dest = imageA;
            if (!inPlace)
                dest = imageA.CopyBlank();

            ParallelLauncher.Launch(thread =>
            {
                func(ref imageA[thread.Y, thread.X], ref imageB[thread.Y, thread.X], ref dest[thread.Y, thread.X]);
            },
            imageA.Width(), imageA.Height());

            return dest;
        }
    }

    /// <summary>
    /// Two source filter operation.
    /// </summary>
    /// <param name="src1">First image.</param>
    /// <param name="src2">Second image.</param>
    /// <param name="dest">Destination image.</param>
    public delegate void TwoSourceFilterFunc<TColor>(ref TColor src1, ref TColor src2, ref TColor dest)
        where TColor : struct;
}
