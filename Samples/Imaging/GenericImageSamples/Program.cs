using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //fast casting 
            Console.WriteLine("*************** Color casting *********************");
            Test.TestColorCasting();
            Console.WriteLine();

            Console.WriteLine("*************** Image<,> <=> Array conversions *********************");
            Test.TestArrayToImage();
            Test.TestArrayAsImageGray();
            Console.WriteLine();
        }
    }
}
