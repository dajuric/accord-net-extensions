using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace RT
{
    public class TrainDataLoader
    {
        public static List<Image<Gray, byte>> LoadBackgroundImages(string backgroundListPath)
        {
            var images = new List<Image<Gray, byte>>();

            var dir = new FileInfo(backgroundListPath).DirectoryName;

            using (TextReader reader = File.OpenText(backgroundListPath))
            {
                string imName;
                while ((imName = reader.ReadLine()) != null)
                {
                    var imPath = Path.Combine(dir, imName);
                    var im = loadImageFromRaw(imPath);
                    images.Add(im);
                }
            }

            return images;
        }

        public static void LoadSampleData(string listPath, PicoClassifier picoClassifier, out List<Image<Gray, byte>> images, out List<Rectangle> windows, bool onlyReferenceSamples = true)
        {
            var dir = new FileInfo(listPath).DirectoryName;

            windows = new List<Rectangle>();
            images = new List<Image<Gray, byte>>();

            using (TextReader reader = File.OpenText(listPath))
            {
                string sampleDataFileName=null;
                while ((sampleDataFileName = reader.ReadLine()) != null)
                {
                    Image<Gray, byte> sampleImage;
                    List<Rectangle> sampleWindows;
                    loadSampleData(dir, sampleDataFileName, picoClassifier, out sampleImage, out sampleWindows);

                    for (int i = 0; i < sampleWindows.Count; i++)
                    {
                        Rectangle window = sampleWindows[i].Intersect(sampleImage.Size);
                        windows.Add(window);

                        if (onlyReferenceSamples)
                            images.Add(sampleImage);
                        else
                            images.Add(sampleImage.Clone());
                    }
                }
            }
        }

        private static void loadSampleData(string dirPath, string sampleDataFileName, PicoClassifier picoClassifier, out Image<Gray, byte> image, out List<Rectangle> windows)
        {
            windows = new List<Rectangle>();

            using (TextReader reader = File.OpenText(Path.Combine(dirPath, sampleDataFileName)))
            {
                var imName = reader.ReadLine(); //read image name
                var imPath = Path.Combine(dirPath, imName);
                image = loadImageFromRaw(imPath);

                int nROIs = Int32.Parse(reader.ReadLine()); //read number of ROIs

                while (nROIs > 0)
                {
                    nROIs--;
                    var rcs = reader.ReadLine().Split(' ');
                    var rect = picoClassifier.GetRegion(
                        new AForge.Point
                        {
                            X = Single.Parse(rcs[1], CultureInfo.InvariantCulture),
                            Y = Single.Parse(rcs[0], CultureInfo.InvariantCulture)
                        },
                        Single.Parse(rcs[2], CultureInfo.InvariantCulture));

                    windows.Add(rect);
                }
            }
        }

        static Image<Gray, byte> loadImageFromRaw(string listPath)
        {
            using (var reader = new BinaryReader(new FileStream(listPath, FileMode.Open, FileAccess.Read)))
            {
                int w = reader.ReadInt32();
                int h = reader.ReadInt32();

                byte[] bytes = new byte[w * h];
                reader.Read(bytes, 0, bytes.Length);

                return bytes.ToImage(w);
            }
        }
    }
}
