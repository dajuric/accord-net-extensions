using Accord.Math;
using System;

namespace Accord.Imaging
{
    public static partial class ComplexImageExtensions
    {
        /// <summary>
        /// Calculates Fast Fourier transform.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="direction">Forward or bacward direction.</param>
        /// <param name="inPlace">Process in place or not.</param>
        /// <returns>Processed image. If <see cref="inPlace"/> is used the result is the same as input image therefore may be ommited.</returns>
        public unsafe static Image<Complex, float> FFT(this Image<Complex, float> image, FourierTransform.Direction direction, bool inPlace = false)
        {
            return FFT<float>(image, direction, inPlace);
        }

        /// <summary>
        /// Calculates Fast Fourier transform.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="direction">Forward or bacward direction.</param>
        /// <param name="inPlace">Process in place or not.</param>
        /// <returns>Processed image. If <see cref="inPlace"/> is used the result is the same as input image therefore may be ommited.</returns>
        private unsafe static Image<Complex, double> FFT(this Image<Complex, double> image, FourierTransform.Direction direction, bool inPlace = false)
        {
            return FFT<double>(image, direction, inPlace);
        }

        internal unsafe static Image<Complex, TDepth> FFT<TDepth>(this Image<Complex, TDepth> image, FourierTransform.Direction direction, bool inPlace = false)
            where TDepth:struct
        {
            Image<Complex, TDepth> dest = null;
            if (inPlace)
                dest = image;
            else
                dest = image.Clone();

            if (typeof(TDepth).Equals(typeof(float)))
            {
                FourierTransform.FFT2((ComplexF*)dest.ImageData, dest.Width, dest.Height, dest.Stride, direction);
            }
            else if (typeof(TDepth).Equals(typeof(double)))
            {
                throw new NotImplementedException();
                //FourierTransform.FFT2((Complex*)dest.ImageData, dest.Width, dest.Height, dest.Stride, direction);
            }
            else
                throw new NotSupportedException();
            
            return dest;
        }
    }
}
