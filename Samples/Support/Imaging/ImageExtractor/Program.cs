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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.Extensions;

namespace ImageExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            //emulate input args
            //string fileMask = Path.Combine(@"C:\Users\Darko\Google disk\Anotacija ruke - Biljana\*.mp4");
            //string fileMask = Path.Combine(@"C:\Users\Darko-Home\Desktop\Svjetla - baza podataka\Stražnja", "AA012201.mxf");
            string fileMask = Path.Combine(getResourceDir(), "Welcome Accord.NET Extensions.mp4");

            if (args.Length == 1)
                fileMask = args[0];

            var fileNames = enumerateFiles(fileMask);
            foreach (var fileName in fileNames)
            {
                extractVideo(fileName);
            }
        }

        private static void extractVideo(string fileName)
        {
            //get output dir (same as file name and in the same folder as video)
            var fileInfo = new FileInfo(fileName);
            var fileNameNoExt = fileInfo.Name.Replace(fileInfo.Extension, String.Empty);
            string outputDir = Path.Combine(fileInfo.DirectoryName, fileNameNoExt);

            //open video
            var reader = new FileCapture(fileName);
            reader.Open();

            Console.WriteLine("Extracting video frames - {0}...", fileNameNoExt);

            var videoExtractor = new VideoExtractor(reader, outputDir, "{0}.jpg");
            videoExtractor.Start((percentage) =>
            {
                Console.Write("\r Completed: {0} %", (int)(percentage * 100));
            });

            Console.WriteLine();
        }

        private static IEnumerable<string> enumerateFiles(string fileMask)
        {
            var pathDelimiter = Path.DirectorySeparatorChar;

            fileMask = fileMask.NormalizePathDelimiters(pathDelimiter.ToString());
            string fileMaskWithoutPath = fileMask.Split(pathDelimiter).Last();
            string path = fileMask.Replace(fileMaskWithoutPath, String.Empty);

            var fileNames = Directory.EnumerateFiles(path, fileMaskWithoutPath);
            return fileNames;
        }

        private static string getResourceDir()
        {
            return Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName, "Resources");
        }
    }
}
