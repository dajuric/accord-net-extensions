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

namespace GenericImage
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("******************************************* Supported paths ********************************************"); //TODO: (critical) should forbid some paths 
            Console.WriteLine();
            Test.WriteAllConversionPaths();
            Console.WriteLine();
 
            Console.WriteLine("*************** Color casting *********************");
            Test.TestColorCasting();
            Console.WriteLine();

            Console.WriteLine("******************************************* AForge vs Image<,> - performance ********************************************");
            Console.WriteLine();

            Console.WriteLine("Color conversions...");
            Test.TestColorConversion();
            Console.WriteLine();

            Console.WriteLine("FFT...");
            Test.TestFFT();
            Console.WriteLine();

            Console.WriteLine("Color filtering...");
            Test.TestColorFiltering();
            Console.WriteLine();

            Console.WriteLine("Channel modifier...");
            Test.TestChannelModifier();
            Console.WriteLine();

            Console.WriteLine("Small kernel...");
            Test.TestConvolve(11);
            Console.WriteLine();

            Console.WriteLine("Big kernel... (please be very patient) - few minutes");
            Test.TestConvolve(99);
            Console.WriteLine();

            Console.WriteLine("**********************************************************************************************************");
            Console.WriteLine();
        }
    }
}
