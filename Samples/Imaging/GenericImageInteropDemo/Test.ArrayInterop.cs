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

namespace GenericImageInteropDemo
{
    public partial class Test
    {
        public static void TestArrayToImage()
        {
            byte[, ,] arr = new byte[3, 480, 640];
            arr[0, 5, 5] = 10;
            arr[1, 5, 5] = 20;
            arr[2, 5, 5] = 30;
            Image<Bgr, byte> img = arr.ToImage<Bgr, byte>();

            Console.WriteLine("Color image => array: " + img[5, 5]);
            /*Debug.Assert(img[5, 5].B == arr[0, 5, 5] && 
                         img[5, 5].G == arr[1, 5, 5] && 
                         img[5, 5].R == arr[2, 5, 5]);*/

            //img.Save("arrayToImgTest.png");
        }

        public static void TestArrayAsImageGray()
        {
            byte[,] arr = new byte[480, 640];
            arr[5, 5] = 10;
            Image<Gray, byte> img = arr.AsImage(); //shares data between array and image (array is pinned)

            Console.WriteLine("Gray image as array: " + img[5, 5]);
            //Debug.Assert(img[5, 5].Intensity == arr[5, 5]);
         
            //img.Save("arrayAsImgGrayTest.png");
        }

        public static void TestImageToArray()
        {
            Image<Bgr, byte> img = new Image<Bgr, byte>(640, 480);
            img[5, 5] = new Bgr(128, 64, 32);

            var arr = img.ToArray();

            Console.WriteLine("Color image to array: " + arr[0, 5, 5] + " " + arr[1, 5, 5] + " " + arr[2, 5, 5]);
            /*Debug.Assert(arr[5, 5, 0] == 128 &&
                         arr[5, 5, 1] == 64 &&
                         arr[5, 5, 2] == 32);*/
        }
    }
}
