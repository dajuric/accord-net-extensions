using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions
{
    public static class PathExtensions
    {
        public static bool IsSubfolder(this string childPath, string parentPath, bool onlyStrictSubfolder = true)
        {
            var parentUri = new Uri(parentPath);

            var dirChild = new DirectoryInfo(childPath);
            var childUri = (onlyStrictSubfolder) ? dirChild.Parent : dirChild;
                
            while (childUri != null)
            {
                if (new Uri(childUri.FullName) == parentUri)
                {
                    return true;
                }

                childUri = childUri.Parent;
            }

            return false;
        }

        /// <summary>
        /// Gets relative file path regarding specified directory.
        /// </summary>
        /// <param name="fileName">Full file name and path.</param>
        /// <param name="directoryPath">Directory path which serves as root directory.</param>
        /// <returns>Relative file path. In case the relative path could not be find the empty string is returned.</returns>
        public static string GetRelativeFilePath(this string fileName, string directoryPath)
        {
            fileName = fileName.NormalizePathDelimiters();
            directoryPath = directoryPath.NormalizePathDelimiters(); 

            bool isEqual = true;
            int lastEqualIdx = -1;

            while (isEqual && Math.Min(directoryPath.Length, fileName.Length) > (lastEqualIdx + 1))
            {
                if (directoryPath[lastEqualIdx + 1] == fileName[lastEqualIdx + 1])
                    lastEqualIdx++;
                else
                    isEqual = false;
            }

            if (lastEqualIdx == -1)
                return String.Empty;

            return "/" + fileName.Substring(lastEqualIdx + 1);
        }

        public static string NormalizePathDelimiters(this string path, string normalizedDelimiter = "/")
        {
            return path.Replace("//", normalizedDelimiter)
                       .Replace(@"\", normalizedDelimiter)
                       .Replace(@"\\", normalizedDelimiter)
                       .Replace(@"/" , normalizedDelimiter);
        }
    }
}
