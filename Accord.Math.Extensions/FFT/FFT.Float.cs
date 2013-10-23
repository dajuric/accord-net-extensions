namespace Accord.Math
{
    using Accord.Core;
    using AForge.Math;
    using System;

    public static partial class FourierTransform
    {    
        public unsafe static void FFT(ComplexF[] data, Direction direction)
        {
            fixed (ComplexF* dataPtr = data)
            {
                FFT(dataPtr, data.Length, direction);
            }
        }

        /// <summary>
        /// One dimensional Fast Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">Data to transform.</param>
        /// <param name="direction">Transformation direction.</param>
        /// 
        /// <remarks><para><note>The method accepts <paramref name="data"/> array of 2<sup>n</sup> size
        /// only, where <b>n</b> may vary in the [1, 14] range.</note></para></remarks>
        /// 
        /// <exception cref="ArgumentException">Incorrect data length.</exception>
        /// 
        public unsafe static void FFT(ComplexF* data, int length, Direction direction)
        {
            int n = length;
            int m = Tools.Log2(n);

            // reorder data first
            ReorderData(data, length);

            // compute FFT
            int tn = 1, tm;

            for (int k = 1; k <= m; k++)
            {
                ComplexF[] rotation = FourierTransform.GetComplexRotation(k,  direction);

                tm = tn;
                tn <<= 1;

                for (int i = 0; i < tm; i++)
                {
                    ComplexF t = rotation[i];

                    for (int even = i; even < n; even += tn)
                    {
                        int odd = even + tm;
                        ComplexF* ce = &data[even];
                        ComplexF* co = &data[odd];
                        
                        float tr = co->Re * t.Re - co->Im * t.Im;
                        float ti = co->Re * t.Im + co->Im * t.Re;

                        co->Re = ce->Re - tr;
                        co->Im = ce->Im - ti;

                        ce->Re += tr;
                        ce->Im += ti;
                    }
                }
            }

            if (direction == Direction.Backward)
            {
                for (int i = 0; i < n; i++)
                {
                    data[i].Re /= n;
                    data[i].Im /= n;
                }
            }
        }

        public unsafe static void FFT2(ComplexF[,] data, Direction direction)
        {
            int width = data.GetLength(1);
            int height = data.GetLength(0);

            fixed (ComplexF* dataPtr = data)
            {
                FFT2(dataPtr, width, height, width, direction);
            }
        }

        /// <summary>
        /// Two dimensional Fast Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">Data to transform.</param>
        /// <param name="direction">Transformation direction.</param>
        /// 
        /// <remarks><para><note>The method accepts <paramref name="data"/> array of 2<sup>n</sup> size
        /// only in each dimension, where <b>n</b> may vary in the [1, 14] range. For example, 16x16 array
        /// is valid, but 15x15 is not.</note></para></remarks>
        /// 
        /// <exception cref="ArgumentException">Incorrect data length.</exception>
        /// 
        public unsafe static void FFT2(ComplexF* data, int width, int height, int stride, Direction direction)
        {
            const int MIN_PATCH_SIZE = 32; //how much rows/columns should one thread process

            int k = height;
            int n = width;

            // check data size
            if (
                    (!Tools.IsPowerOf2(k)) ||
                    (!Tools.IsPowerOf2(n)) ||
                    (k < minLength) || (k > maxLength) ||
                    (n < minLength) || (n > maxLength)
                    )
            {
                throw new ArgumentException("Incorrect data length.");
            }

            // process rows
            var procRow = new ParallelProcessor<bool, bool>(new System.Drawing.Size(1 /*does not matter*/, height),
                                                            () => true,
                                                            (_, __, area) =>
                                                            {
                                                                ComplexF* dataPatchPtr = data + area.Y * stride / sizeof(ComplexF); //get row

                                                                for (int i = 0; i < area.Height; i++)
                                                                {
                                                                    // transform it
                                                                    FourierTransform.FFT(dataPatchPtr, n, direction);

                                                                    dataPatchPtr += stride / sizeof(ComplexF);
                                                                }
                                                            },
                                                            new ParallelOptions { ParallelTrigger = (size) => size.Height >= MIN_PATCH_SIZE
                                                                                  /*,ForceSequential = true*/}
                                                              );

            // process columns
            //(y and x are swaped => proc thinks it is diving horizontal pacthes but instead we are using them as vertical ones)
            var procCol = new ParallelProcessor<bool, bool>(new System.Drawing.Size(1 /*does not matter*/, width),
                                                            () => true,
                                                            (_, __, area) =>
                                                            {
                                                                ComplexF* dataPatchPtr = &data[area.Y]; //get column 

                                                                fixed (ComplexF* _col = new ComplexF[k])
                                                                {
                                                                    ComplexF* col = _col;

                                                                    for (int j = 0; j < area.Height; j++)
                                                                    {
                                                                        // copy column
                                                                        ComplexF* dataColPtr = &dataPatchPtr[j];
                                                                        for (int i = 0; i < k; i++)
                                                                        {
                                                                            col[i] = *dataColPtr;
                                                                            dataColPtr += stride / sizeof(ComplexF);
                                                                        }

                                                                        // transform it
                                                                        FourierTransform.FFT(col, k, direction);

                                                                        // copy back
                                                                        dataColPtr = &dataPatchPtr[j];
                                                                        for (int i = 0; i < k; i++)
                                                                        {
                                                                            *dataColPtr = col[i];
                                                                            dataColPtr += stride / sizeof(ComplexF);
                                                                        }
                                                                    }
                                                                }
                                                            },
                                                            new ParallelOptions { ParallelTrigger = (size) => size.Height >= MIN_PATCH_SIZE
                                                                                 /*,ForceSequential = true */}
                                                            );

           procRow.Process(true);
           procCol.Process(true);         
        }

        #region Private Region

        private static ComplexF[,][] complexRotation = new ComplexF[maxBits, 2][];

        // Get rotation of complex number
        private static ComplexF[] GetComplexRotation(int numberOfBits, Direction direction)
        {
            int directionIndex = (direction == Direction.Forward) ? 1 : 0;

            // check if the array is already calculated
            if (complexRotation[numberOfBits - 1, directionIndex] == null)
            {
                int n = 1 << (numberOfBits - 1);
                float uR = 1.0f;
                float uI = 0.0f;
                double angle = System.Math.PI / n * (-(int)(direction));
                float wR = (float)System.Math.Cos(angle);
                float wI = (float)System.Math.Sin(angle);
                float t;
                ComplexF[] rotation = new ComplexF[n];

                for (int i = 0; i < n; i++)
                {
                    rotation[i] = new ComplexF { Re = uR, Im = uI};
                    t = uR * wI + uI * wR;
                    uR = uR * wR - uI * wI;
                    uI = t;
                }

                complexRotation[numberOfBits - 1, directionIndex] = rotation;
            }
            return complexRotation[numberOfBits - 1, directionIndex];
        }

        // Reorder data for FFT using
        private unsafe static void ReorderData(ComplexF* data, int length)
        {
            int len = length;

            // check data length
            if ((len < minLength) || (len > maxLength) || (!Tools.IsPowerOf2(len)))
                throw new ArgumentException("Incorrect data length.");

            int[] rBits = GetReversedBits(Tools.Log2(len));

            for (int i = 0; i < len; i++)
            {
                int s = rBits[i];

                if (s > i)
                {
                    ComplexF t = data[i];
                    data[i] = data[s];
                    data[s] = t;
                }
            }
        }

        #endregion
    }
}
