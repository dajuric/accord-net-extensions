using System;
using System.IO;

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides methods for string which is treated as file and directory path.
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// Gets whether the path is child path.
        /// </summary>
        /// <param name="childPath">The child path.</param>
        /// <param name="parentPath">The parent path.</param>
        /// <returns>True if the child path is indeed child path (or the same) as parent path, otherwise false.</returns>
        public static bool IsSubfolder(this string childPath, string parentPath)
        {
            var parentUri = new Uri(parentPath);
            var childUri = new DirectoryInfo(childPath);
                
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

        /// <summary>
        /// replaces path delimiters with specified one.
        /// </summary>
        /// <param name="path">Path to replace delimiters.</param>
        /// <param name="normalizedDelimiter">Replacing delimiter.</param>
        /// <returns>Path with replaced delimiters.</returns>
        public static string NormalizePathDelimiters(this string path, string normalizedDelimiter = "/")
        {
            return path.Replace("//", normalizedDelimiter)
                       .Replace(@"\", normalizedDelimiter)
                       .Replace(@"\\", normalizedDelimiter)
                       .Replace(@"/" , normalizedDelimiter);
        }
    }
}
