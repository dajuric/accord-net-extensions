#define LOG

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
        public static List<Image<TColor, byte>> LoadBackgroundImages<TColor>(string backgroundListPath)
            where TColor: IColor
        {
#if LOG
            int nLoadedFiles = 0;
#endif

            var images = new List<Image<TColor, byte>>();

            var dir = new FileInfo(backgroundListPath).DirectoryName;

            using (TextReader reader = File.OpenText(backgroundListPath))
            {
                string imName;
                while ((imName = reader.ReadLine()) != null)
                {
                    var imPath = Path.Combine(dir, imName);
                    var im = loadImageFromRaw<TColor>(imPath);
                    images.Add(im);

#if LOG
                    nLoadedFiles++;
                    Console.Write("\rLoaded negative samples {0}.", nLoadedFiles);
#endif
                }
            }

#if LOG
            Console.WriteLine();
#endif

            return images;
        }

        public static void LoadSampleData<TColor>(string listPath, PicoClassifier<TColor> picoClassifier, out List<Image<TColor, byte>> images, out List<Rectangle> windows, bool onlyReferenceSamples = true)
            where TColor: IColor
        {
#if LOG
            int nLoadedFiles = 0;
#endif

            var dir = new FileInfo(listPath).DirectoryName;

            windows = new List<Rectangle>();
            images = new List<Image<TColor, byte>>();

            using (TextReader reader = File.OpenText(listPath))
            {
                string sampleDataFileName=null;
                while ((sampleDataFileName = reader.ReadLine()) != null)
                {
                    Image<TColor, byte> sampleImage;
                    List<Rectangle> sampleWindows;
                    loadSampleData(dir, sampleDataFileName, picoClassifier, out sampleImage, out sampleWindows);

                    foreach (var window in sampleWindows)
                    {
                        windows.Add(window);

                        if (onlyReferenceSamples)
                            images.Add(sampleImage);
                        else
                            images.Add(sampleImage.Clone());

#if LOG
                        nLoadedFiles++;
                        Console.Write("\rLoaded positive samples {0}.", nLoadedFiles);
#endif
                    }
                }
            }

#if LOG
            Console.WriteLine();
#endif
        }

        private static void loadSampleData<TColor>(string dirPath, string sampleDataFileName, PicoClassifier<TColor> picoClassifier, out Image<TColor, byte> image, out List<Rectangle> windows)
            where TColor: IColor
        {
            windows = new List<Rectangle>();

            using (TextReader reader = File.OpenText(Path.Combine(dirPath, sampleDataFileName)))
            {
                var imName = reader.ReadLine(); //read image name
                var imPath = Path.Combine(dirPath, imName);
                image = loadImageFromRaw<TColor>(imPath);

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

                    /*if (rect.Right > image.Width || rect.Bottom > image.Height)
                        Console.WriteLine();*/

                    rect = rect.Intersect(image.Size, preserveScale: true); //ensure that the window is inside the image
                    windows.Add(rect);
                }
            }
        }

        static Image<TColor, byte> loadImageFromRaw<TColor>(string listPath)
            where TColor: IColor
        {
            using (var reader = new BinaryReader(new FileStream(listPath, FileMode.Open, FileAccess.Read)))
            {
                int w = reader.ReadInt32();
                int h = reader.ReadInt32();

                byte[] bytes = new byte[w * h];
                reader.Read(bytes, 0, bytes.Length);

                return bytes.ToImage<TColor>(w, h);
            }
        }
    }
}
