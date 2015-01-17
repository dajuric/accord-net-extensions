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

using System.Diagnostics;
using Accord.Extensions.Imaging;
using System;
using AForge.Imaging;
using System.Drawing;

namespace GenericImageInteropDemo
{
    public partial class Test
    {
        public static void AForgeImToImage()
        { 
            var bmp = new Bitmap(640, 480, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            UnmanagedImage uIm = UnmanagedImage.FromManagedImage(bmp);

            var img = uIm.AsImage();
            Console.WriteLine("UnmanagedImage to Image<,> automatically: Bitmap pixel format {0} is transformed into: {1}.", uIm.PixelFormat, img.ColorInfo);

            var img2 = uIm.ToImage<Bgr, byte>();
            Console.WriteLine("UnmanagedImage to Image<,> forced: Bitmap pixel format {0} is transformed into: {1}.", uIm.PixelFormat, img.ColorInfo);
        }

        public static void ImageToAForgeIm()
        {
            var img = new Image<Bgr, byte>(640, 480);

            UnmanagedImage uIm = img.ToAForgeImage(); //data is shared if possible
            Console.WriteLine("Image<,> to UnmanagedImage: Color {0} is transformed into bitmap pixel format: {1}. Data is {2} shared.", img.ColorInfo, uIm.PixelFormat, img.CanCastToAForgeImage() ? "": "not");
        }
    }
}
