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
