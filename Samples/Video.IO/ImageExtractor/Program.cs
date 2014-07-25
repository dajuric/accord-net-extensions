using Accord.Extensions.Vision;
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
            //string fileMask = Path.Combine(@"C:\Users\Darko-Home\Desktop\HandVideos\popravljeno", "*.mp4");
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

            var videoExtractor = new VideoExtractor(reader, outputDir, 95, "{0}.jpg");
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
