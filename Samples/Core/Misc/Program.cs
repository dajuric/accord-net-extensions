using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions;

namespace Misc
{
    class Program
    {
        static void Main(string[] args)
        {
            testStringPathExtensions(); //TODO: finish
        }

        static void testStringPathExtensions()
        {
            string filePath = @"C:/Some folder/Some child folder\file.txt";

            Console.WriteLine("Normalized delimiters: " + filePath.NormalizePathDelimiters());
            //Console.WriteLine("Normalized delimiters: " + filePath.NormalizePathDelimiters());
        }
    }
}
