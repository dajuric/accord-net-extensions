using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Accord.Extensions
{
    /// <summary>
    /// Contains functions and properties for platform interoperability.
    /// </summary>
    public static class Platform
    {
        /// <summary>
        /// Operating system type.
        /// </summary>
        public enum OperatingSystem
        {
            Windows,
            Linux,
            MacOS
        }

        static Platform()
        {
            RunningPlatform = getRunningPlatform();
        }

        /// <summary>
        /// Gets operating system name.
        /// Taken from: <see cref="http://stackoverflow.com/questions/10138040/how-to-detect-properly-windows-linux-mac-operating-systems"/> and modified.
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
        /// Adds the specified directory to unmanaged library search path for functions that load unmanaged library. <see cref="DllImport"/> attribute is also included.
        /// Internally it changes process environmental variable.
        /// </summary>
        /// <param name="dllDirectory">Directory where to search unmanaged libraries.</param>
        public static void AddDllSearchPath(string dllDirectory)
        {
            dllDirectory = dllDirectory.NormalizePathDelimiters("\\");

            var path = "";
            switch (RunningPlatform)
            {
                case OperatingSystem.Windows:
                    path = "PATH";
                    Environment.SetEnvironmentVariable(path, Environment.GetEnvironmentVariable(path) + ";" + dllDirectory);
                    break;
                case OperatingSystem.MacOS:
                    path = "LD_LIBRARY_PATH";
                    //Environment.SetEnvironmentVariable(path, Environment.GetEnvironmentVariable(path) + ":" + dllDirectory); //TODO - critical: check is that valid ?
                    throw new NotImplementedException("How to change " + path);
                    //break;
                case OperatingSystem.Linux:
                    path = "DYLD_FRAMEWORK_PATH";
                    throw new NotImplementedException("How to change " + path);
                    //break;
            }
        }

        /// <summary>
        /// Adds the default directory to unmanaged library search path for functions that load unmanaged library. <see cref="DllImport"/> attribute is also included.
        /// The default directory is platform specific:
        /// <para>Windows: /UnmanagedLibraries/Windows/x86/ or /UnmanagedLibraries/Windows/x64/</para>
        /// <para>  MacOS: /UnmanagedLibraries/MacOS/</para>
        /// <para>  Linux: /UnmanagedLibraries/Linux/</para>
        /// </summary>
        public static void AddDllSearchPath()
        {
            var baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UnmanagedLibraries");
            var loadDirectory = Path.Combine(baseDirectory, Platform.RunningPlatform.ToString());

            if (Platform.RunningPlatform == Platform.OperatingSystem.Windows)
                loadDirectory = Path.Combine(loadDirectory, Environment.Is64BitProcess ? "x64" : "x86");

            AddDllSearchPath(loadDirectory);
        }


        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        private static extern IntPtr LoadWindowsLibrary([MarshalAs(UnmanagedType.LPStr)] string fileName);

        /// <summary>
        /// Flags for <see cref="LoadUnixLibrary"/> function.
        /// </summary>
        enum LoadUnixLibFlags
        {
            RTLD_LAZY = 0x1,
            RTLD_NOW = 0x2,
            RTLD_LOCAL = 0x4,
            RTLD_GLOBAL = 0x8,
            RTLD_NOLOAD = 0x10,
            RTLD_NODELETE = 0x80
            //RTLD_DEEPBIND ??
        }

        [DllImport("dl", EntryPoint = "dlopen")]
        private static extern IntPtr LoadUnixLibrary([MarshalAs(UnmanagedType.LPStr)] string fileName, LoadUnixLibFlags falgs);

        /// <summary>
        /// Loads an unmanaged library. use <see cref="GetModuleFormatString"/> to set the appropriate extension if necessary.
        /// </summary>
        /// <param name="fileName">Unmanaged library file name.</param>
        /// <returns>Pointer to an loaded library.</returns>
        public static IntPtr LoadLibrary(string fileName)
        {
            var loadDir = new FileInfo(fileName).DirectoryName;

            var oldCurrDir = Environment.CurrentDirectory; //temporary swap paths to load possible dependencies
            Environment.CurrentDirectory = loadDir;

            fileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName)); //filename without extension
            fileName = String.Format(GetModuleFormatString(), fileName);

            IntPtr ptr = IntPtr.Zero;

            if (File.Exists(fileName))
            {
                ptr = (RunningPlatform == OperatingSystem.Windows) ?
                                LoadWindowsLibrary(fileName) :
                                LoadUnixLibrary(fileName, LoadUnixLibFlags.RTLD_NOW);
            }

            Environment.CurrentDirectory = oldCurrDir;

            return ptr;
        }

        /// <summary>
        /// Gets a platform specific module format (e.g. Windows {0}.dll).
        /// </summary>
        /// <returns>Modlule format string.</returns>
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
