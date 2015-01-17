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
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("********* Image<,> <=> Array conversions (built-in) ****************"); Console.ResetColor();
            Test.TestArrayToImage();

            Console.WriteLine();
            Test.TestArrayAsImageGray();

            Console.WriteLine();
            Test.TestImageToArray();


            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("********* Image<,> <=> OpenCV conversions (built-in) ****************"); Console.ResetColor();
            Test.ImageToOpenCVIm();

            Console.WriteLine();
            Test.OpenCVImToImage();



            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("*********** Image<,> <=> Bitmap conversions (BitmapInterop) ****************"); Console.ResetColor();
            Test.TestImageToBitmap();

            Console.WriteLine();
            Test.TestBitmapToImage();



            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("****** Image<,> <=> AForge UnmanagedImage conversions  (AForgeInterop) *******"); Console.ResetColor();
            Test.AForgeImToImage();

            Console.WriteLine();
            Test.ImageToAForgeIm();
        }
    }
}
