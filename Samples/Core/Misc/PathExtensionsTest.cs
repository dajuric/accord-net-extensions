using Accord.Extensions;
using System;
using System.IO;

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
