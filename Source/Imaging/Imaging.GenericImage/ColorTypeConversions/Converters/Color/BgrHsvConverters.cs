#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Accord.Extensions.Imaging.Converters
{
    static class BgrHsvConverters
    {
        #region Bgr8 -> Hsv8

        /// <summary>
        /// see: http://stackoverflow.com/questions/3018313/algorithm-to-convert-rgb-to-hsv-and-hsv-to-rgb
        /// and: http://www.cs.rit.edu/~ncs/color/t_convert.html
        /// range for  HSV is H:[0-180], S:[0-255], V:[0-255]
        /// </summary>
        public unsafe static void ConvertBgrToHsv_Byte(IImage src, IImage dest)
        {
            Bgr8* srcPtr = (Bgr8*)src.ImageData;
            Hsv8* dstPtr = (Hsv8*)dest.ImageData;
            
            int width = src.Width;
            int height = src.Height;

            int srcShift = src.Stride - width * sizeof(Bgr8); //DO NOT divide with sizeof(Bgr8) as reminder may not be 0!!!
            int dstShift = dest.Stride - width * sizeof(Hsv8);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Bgr8.ConvertBgrToHsv(srcPtr, dstPtr);

                    srcPtr++;
                    dstPtr++;
                }

                srcPtr = (Bgr8*)((byte*)srcPtr + srcShift);
                dstPtr = (Hsv8*)((byte*)dstPtr + dstShift);
            }
        }

        #endregion

        #region Hsv8 -> Bgr8

        /// <summary>
        /// see: http://stackoverflow.com/questions/3018313/algorithm-to-convert-rgb-to-hsv-and-hsv-to-rgb
        /// and: http://www.cbusforums.com/forums/showthread.php?t=8657
        /// range for  HSV is H:[0-180], S:[0-255], V:[0-255]
        /// </summary>
        public unsafe static void ConvertHsvToBgr_Byte(IImage src, IImage dest)
        {
            Hsv8* srcPtr = (Hsv8*)src.ImageData;
            Bgr8* dstPtr = (Bgr8*)dest.ImageData;

            int width = src.Width;
            int height = src.Height;

            int srcShift = src.Stride - width * sizeof(Hsv8); //DO NOT divide with sizeof(Bgr8) as reminder may not be 0!!!
            int dstShift = dest.Stride - width * sizeof(Bgr8);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                { 
                    Hsv8.ConvertHsvToBgr(srcPtr, dstPtr);

                    srcPtr++;
                    dstPtr++;
                }

                srcPtr = (Hsv8*)((byte*)srcPtr + srcShift);
                dstPtr = (Bgr8*)((byte*)dstPtr + dstShift);
            }
        }

        #endregion
    }
}
