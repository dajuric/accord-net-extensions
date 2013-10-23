using Accord.Imaging;
using Accord.Imaging.Converters;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public partial class Test
    {
        public void WriteAllConversionPaths()
        {
            writeConversionPaths();
        }

        static void writeConversionPaths()
        {
            Console.BufferHeight = 1000;
            Console.BufferWidth = 250;

            var colorTypes = new Type[] { typeof(Color3), typeof(Color4), typeof(Gray), typeof(Bgr), typeof(Hsv), typeof(Complex) };
            var depthTypes = new Type[] { typeof(byte), typeof(short), typeof(int), typeof(float), typeof(double) };

            var allPossibleCombinations = getAllPossibleColors(colorTypes, depthTypes);

            foreach (var colorComb1 in allPossibleCombinations)
            {
                foreach (var colorComb2 in allPossibleCombinations)
                {
                    writeColorConversionPath(colorComb1, colorComb2);
                }
                Console.WriteLine();
            }
        }

        static List<ColorInfo> getAllPossibleColors(Type[] colorTypes, Type[] depthTypes)
        {
            var combinations = new List<ColorInfo>();

            foreach (var colorType in colorTypes)
            {
                foreach (var depthType in depthTypes)
                {
                    var colorInfo = ColorInfo.GetInfo(colorType, depthType);
                    combinations.Add(colorInfo);
                }
            }

            return combinations;
        }

        static void writeColorConversionPath(ColorInfo c1, ColorInfo c2)
        {
            var path = ColorConverter.GetMostInexepnsiveConversionPath(c1, c2);
            Console.Write("{0} => {1}:  ", c1, c2);
            Console.CursorLeft = 40;

            Console.Write("Cast: {0}", !ColorConverter.ConversionPathCopiesData(path));
            Console.CursorLeft = 55;

            if (path == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("NO PATH");
                Console.ResetColor();
                return;
            }
            else if (path.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("return source");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var stage in path)
            {
                Console.Write("=>  ({0} => {1})", stage.SourceColorInfo, stage.DestColorInfo);
            }
            Console.ResetColor();

            Console.WriteLine();
        }
    }
}
