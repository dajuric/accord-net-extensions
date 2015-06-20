#region Licence and Terms
// DotImaging Framework
// https://github.com/dajuric/dot-imaging
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
using System.IO;
using System.Runtime.InteropServices;

namespace DotImaging
{
    /// <summary>
    /// Contains functions and properties for platform interoperability.
    /// </summary>
    internal static class Platform //TODO: replace with extension when available
    {
        /// <summary>
        /// Operating system type.
        /// </summary>
        public enum OperatingSystem
        {
            /// <summary>
            /// Windows family.
            /// </summary>
            Windows,
            /// <summary>
            /// Linux family
            /// </summary>
            Linux,
            /// <summary>
            /// MacOS family
            /// </summary>
            MacOS
        }

        static Platform()
        {
            RunningPlatform = getRunningPlatform();
        }

        /// <summary>
        /// Gets operating system name.
        /// <para>
        /// Taken from: <a href="http://stackoverflow.com/questions/10138040/how-to-detect-properly-windows-linux-mac-operating-systems"/> and modified.
        /// </para>
        /// </summary>
        private static OperatingSystem getRunningPlatform()
        { 
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    // Well, there are chances MacOSX is reported as Unix instead of MacOSX.
                    // Instead of platform check, we'll do a feature checks (Mac specific root folders)
                    if (Directory.Exists("/Applications")
                        & Directory.Exists("/System")
                        & Directory.Exists("/Users")
                        & Directory.Exists("/Volumes"))
                        return OperatingSystem.MacOS;
                    else
                        return OperatingSystem.Linux;

                case PlatformID.MacOSX:
                    return OperatingSystem.MacOS;

                default:
                    return OperatingSystem.Windows;
            }
        }

        /// <summary>
        /// Gets operating system name.
        /// </summary>
        public static OperatingSystem RunningPlatform
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds the specified directory to unmanaged library search path for functions that load unmanaged library. See <paramref name="dllDirectory"/> attribute is also included.
        /// Internally it changes process environmental variable.
        /// </summary>
        /// <param name="dllDirectory">Directory where to search unmanaged libraries.</param>
        public static void AddDllSearchPath(string dllDirectory)
        {
            dllDirectory = dllDirectory.NormalizePathDelimiters(Path.DirectorySeparatorChar.ToString());

            var path = "";
            switch (RunningPlatform)
            {
                case OperatingSystem.Windows:
                    path = "PATH";  
                    break;
                case OperatingSystem.MacOS:
                    path = "LD_LIBRARY_PATH";
                    break;
                case OperatingSystem.Linux:
                    path = "DYLD_FRAMEWORK_PATH";
                    break;
            }

            Environment.SetEnvironmentVariable(path, Environment.GetEnvironmentVariable(path) + Path.PathSeparator + dllDirectory);
        }

        /// <summary>
        /// Gets a default unmanaged library search directory.
        /// The default directory is platform specific:
        /// <para>Windows: /UnmanagedLibraries/Windows/x86/ or /UnmanagedLibraries/Windows/x64/</para>
        /// <para>  MacOS: /UnmanagedLibraries/MacOS/</para>
        /// <para>  Linux: /UnmanagedLibraries/Linux/</para>
        /// </summary>
        /// <param name="rootDirectory">Root directory which marks the starting point (e.g. executing assembly directory).</param>
        /// <returns>Default unmanaged library search directory.</returns>
        public static string GetDefaultDllSearchPath(string rootDirectory)
        {
            var baseDirectory = Path.Combine(rootDirectory, "UnmanagedLibraries");
            var loadDirectory = Path.Combine(baseDirectory, Platform.RunningPlatform.ToString());

            if (Platform.RunningPlatform == Platform.OperatingSystem.Windows)
                loadDirectory = Path.Combine(loadDirectory, Environment.Is64BitProcess ? "x64" : "x86");

            return loadDirectory;
        }

        /// <summary>
        /// Adds the default directory to unmanaged library search path for functions that load unmanaged library. The root directory is the current directory. 
        /// The default directory is platform specific:
        /// <para>Windows: /UnmanagedLibraries/Windows/x86/ or /UnmanagedLibraries/Windows/x64/</para>
        /// <para>  MacOS: /UnmanagedLibraries/MacOS/</para>
        /// <para>  Linux: /UnmanagedLibraries/Linux/</para>
        /// </summary>
        public static void AddDllSearchPath()
        { 
            var dllSearchPathPath = GetDefaultDllSearchPath(Directory.GetCurrentDirectory());
            AddDllSearchPath(dllSearchPathPath);
        }

        /// <summary>
        /// Gets a platform specific module format (e.g. Windows {0}.dll).
        /// </summary>
        /// <returns>Module format string.</returns>
        public static String GetModuleFormatString()
        {
            String formatString = null;

            switch (RunningPlatform)
            {
                case OperatingSystem.Windows:
                    formatString = "{0}.dll";
                    break;
                case OperatingSystem.MacOS:
                    formatString = "lib{0}.dylib";
                    break;
                case OperatingSystem.Linux:
                    formatString = "lib{0}.so";
                    break;
                default:
                    formatString = "{0}";
                    break;
            }

            return formatString;
        }
    }
}
