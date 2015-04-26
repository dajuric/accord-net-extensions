#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
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
        /// Returns an enumerable collection of file information that matches a specified search pattern and search subdirectory option.
        /// </summary>
        /// <param name="dirInfo">Directory info.</param>
        /// <param name="searchPatterns">The search strings (e.g. new string[]{ ".jpg", ".bmp" }</param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation
        /// should include only the current directory or all subdirectories. The default
        /// value is <see cref="System.IO.SearchOption.TopDirectoryOnly"/>.
        ///</param>
        /// <returns>An enumerable collection of files that matches <paramref name="searchPatterns"/> and <paramref name="searchOption"/>.</returns>
        public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo dirInfo, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var fileInfos = new List<FileInfo>();
            foreach (var searchPattern in searchPatterns)
            {
                var dirFileInfos = dirInfo.EnumerateFiles(searchPattern, searchOption);
                fileInfos.AddRange(dirFileInfos);
            }

            return fileInfos;
        }

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
        /// <param name="dirInfo">
        /// Directory info of a directory path which serves as root.
        /// </param>
        /// <returns>Relative file path. In case the relative path could not be find the empty string is returned.</returns>
        public static string GetRelativeFilePath(this string fileName, DirectoryInfo dirInfo)
        {
            Stack<string> folders = new Stack<string>();

            var fileDirInfo = new FileInfo(fileName).Directory;

            var currDirInfo = fileDirInfo;
            while (currDirInfo != null)
            {
                if (String.Equals(currDirInfo.FullName.Trim(Path.DirectorySeparatorChar), 
                                  dirInfo.FullName.Trim(Path.DirectorySeparatorChar)))
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
