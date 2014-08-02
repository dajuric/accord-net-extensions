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
using Accord.Extensions.Imaging;
using System;

namespace GenericImageInteropDemo
{
    public partial class Test
    {
        public static void ImageToOpenCVIm()
        {
            var img = new Image<Bgr, int>(640, 480);

            IplImage iplImage = img.AsOpenCvImage(); 
            Console.WriteLine("Image<,> to IplImage: Color {0} is transformed into IplImage depth: {1}.", img.ColorInfo, iplImage.ChannelDepth);
        }

        public static void OpenCVImToImage()
        {
            var img = new Image<Bgr, float>(640, 480);
            IplImage iplImage = img.AsOpenCvImage();

            var convertedImg = iplImage.AsImage(); //define destructor optionally
            Console.WriteLine("IplImage to Image<,> automatically: IplImage depth {0} is transformed into: {1}.",  iplImage.ChannelDepth, convertedImg.ColorInfo);
        }
    }
}
