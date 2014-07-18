using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            //emulate input args
            string fileName = Path.Combine(getResourceDir(), "Welcome Accord.NET Extensions.mp4");

            if (args.Length == 1)
                fileName = args[0];

            //get output dir (same as file name and in the same folder as video)
            var fileInfo = new FileInfo(fileName);
            var fileNameNoExt = fileInfo.Name.Replace(fileInfo.Extension, String.Empty);
            string outputDir = Path.Combine(fileInfo.DirectoryName, fileNameNoExt);

            //open video
            var reader = new FileCapture(fileName);
            reader.Open();

            Console.WriteLine("Extracting video frames...");

            var videoExtractor = new VideoExtractor(reader, outputDir);
            videoExtractor.Start((percentage) =>
            {
                Console.Write("\r Completed: {0} %", (int)(percentage * 100));
            });

            Console.WriteLine();

        }

        private static string getResourceDir()
        {
            return Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName, "Resources");
        }
    }
}
