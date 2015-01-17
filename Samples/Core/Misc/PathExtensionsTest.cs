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
using System.IO;
using Accord.Extensions;

namespace Misc
{
    class PathExtensionsTest
    {
        public static void Test()
        {
            var shortFileName = "Readme.txt";
            var dir = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName, "UnmanagedLibraries");
            var fullFileName = Path.Combine(dir, shortFileName);

            Console.WriteLine();
            testRelativePath(fullFileName, dir);

            Console.WriteLine();
            testNormalizeDelimiters();

            Console.WriteLine();
            testIsSubfolder(dir);
        }

        static void testRelativePath(string fileName, string dir)
        {
            var dirInfo = new DirectoryInfo(dir);
            Console.WriteLine("Relative file '{0}' path regarding the directory '{1}' is {2}.", fileName, dir, fileName.GetRelativeFilePath(dirInfo));
        }

        static void testIsSubfolder(string dir)
        {
            var dirInfo = new DirectoryInfo(dir);
            Console.WriteLine("Dir '{0}' is subfolder regarding dir '{1}': {2}.", dirInfo.FullName, dirInfo.Parent.FullName, dirInfo.FullName.IsSubfolder(dirInfo.Parent.FullName));
        }

        static void testNormalizeDelimiters()
        {
            string filePath = @"C:/Some folder/Some child folder\file.txt";
            Console.WriteLine("Normalized delimiters: " + filePath.NormalizePathDelimiters()); //replaces path delimiters with the specified one
        }
    }
}
