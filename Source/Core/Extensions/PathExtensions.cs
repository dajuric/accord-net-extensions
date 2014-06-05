using System;
using System.Collections;
using System.Collections.Generic;
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
        /// <param name="path">
        /// Directory path which serves as root.
        /// </param>
        /// <returns>Relative file path. In case the relative path could not be find the empty string is returned.</returns>
        public static string GetRelativeFilePath(this string fileName, DirectoryInfo dirInfo)
        {
            Stack<string> folders = new Stack<string>();

            var fileDirInfo = new FileInfo(fileName).Directory;

            var currDirInfo = fileDirInfo;
            while (currDirInfo != null)
            {
                if (currDirInfo.FullName.Equals(dirInfo.FullName))
                    break;

                folders.Push(currDirInfo.Name);
                currDirInfo = currDirInfo.Parent;
            }

            var folderPath = Path.Combine(folders.ToArray());
            var relativeFilePath = Path.Combine(folderPath,  new FileInfo(fileName).Name);

            return (Path.DirectorySeparatorChar + relativeFilePath).NormalizePathDelimiters();
        }

        /// <summary>
        /// Replaces path delimiters with platform-specific one defined in <see cref="Path.DirectorySeparatorChar"/>.
        /// </summary>
        /// <param name="path">Path to replace delimiters.</param>
        /// <returns>Path with replaced delimiters.</returns>
        public static string NormalizePathDelimiters(this string path)
        {
            return NormalizePathDelimiters(path, Path.DirectorySeparatorChar.ToString());
        }

        /// <summary>
        /// Replaces path delimiters with specified one.
        /// </summary>
        /// <param name="path">Path to replace delimiters.</param>
        /// <param name="normalizedDelimiter">Replacing delimiter.</param>
        /// <returns>Path with replaced delimiters.</returns>
        public static string NormalizePathDelimiters(this string path, string normalizedDelimiter)
        {
            return path.Replace("//", normalizedDelimiter)
                       .Replace(@"\", normalizedDelimiter)
                       .Replace(@"\\", normalizedDelimiter)
                       .Replace(@"/" , normalizedDelimiter);
        }

        /// <summary>
        /// Checks whether the path is file or directory.
        /// </summary>
        /// <param name="path">File or directory path.</param>
        /// <returns>
        /// True if the path is directory, false if the path is file. 
        /// Null is returned if the path does not exist or in case of an internal error.
        /// </returns>
        private static bool? IsDirectory(this string path)
        {
            try
            {
                System.IO.FileAttributes fa = System.IO.File.GetAttributes(path);
                bool isDirectory = false;
                if ((fa & FileAttributes.Directory) != 0)
                {
                    isDirectory = true;
                }

                return isDirectory;
            }
            catch 
            {
                return null;
            }
        }
    }
}
