#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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

using Accord.Extensions.Imaging;
using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericImageInteropDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var img = new Bgr<byte>[480, 640];

            //***********************************************************************************************************************************************************************
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("********* TColor[,] <=> Image<> conversions (built-in) ****************"); Console.ResetColor();
            //to Image<>
            Image<Bgr<byte>> lockedImg = img.Lock();
            //from Image<>
            var arr = lockedImg.Clone();

            //***********************************************************************************************************************************************************************
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("********* Image<,> <=> OpenCV conversions (built-in) ****************"); Console.ResetColor();
            //to IplImage
            IplImage iplImage;
            using (var uImg = img.Lock())
            {
                iplImage = uImg.AsOpenCvImage(); //data is shared
            }
            //from IplImage
            var imgFromIpl = iplImage.AsImage(); 

            //***********************************************************************************************************************************************************************
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("*********** Image<,> <=> Bitmap conversions (BitmapInterop) ****************"); Console.ResetColor();
            //to Bitmap
            var bmp = img.ToBitmap();
            //from Bitmap
            var imgFromBmp = bmp.ToArray();

            //***********************************************************************************************************************************************************************
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("****** TColor[,] <=> AForge UnmanagedImage conversions  (AForgeInterop) *******"); Console.ResetColor();
            //to AForge image
            using (var uImg = img.Lock())
            {
                UnmanagedImage uIm = uImg.AsAForgeImage(); //data is shared
            }

            //from AForge image
            UnmanagedImage aforgeIm = UnmanagedImage.Create(640, 480, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var imgFromAForge = aforgeIm.AsImage(); //TODO: extensions directly to array
        }
    }
}
