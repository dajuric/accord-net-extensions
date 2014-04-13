using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Represents directory stream-able source and provides functions and properties to access data in a stream-able way.
    /// </summary>
    public class ImageDirectoryReader : StreamableSource
    {
        string[] fileNames = null;

        Func<string, IImage> loader;
        long currentFrame = 0;

        #region Initialization

        /// <summary>
        /// Creates an instance of <see cref="ImageDirectoryReader"/>.
        /// </summary>
        /// <param name="dirPath">The directory path.</param>
        /// <param name="extension">The image extension.</param>
        /// <param name="useNaturalSorting">Use natural sorting, otherwise raw image order is used.</param>
        /// <param name="loader">Loader image function. If null default loader is used.</param>
        /// <exception cref="DirectoryNotFoundException">Directory can not be found.</exception>
        public ImageDirectoryReader(string dirPath, string extension, bool useNaturalSorting = true, Func<string, IImage> loader = null)
        {
            if (Directory.Exists(dirPath) == false)
                throw new DirectoryNotFoundException(String.Format("Dir: {0} cannot be found!", dirPath));

            loader = loader ?? cvLoader;
            
            this.IsLiveStream = false;
            this.CanSeek = true;
         
            this.loader = loader;

            string ext = "*." + extension.TrimStart('.', '*');
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);

            IEnumerable<string> files = null;

            if (useNaturalSorting)
            {
                files = directoryInfo.EnumerateFiles(ext, SearchOption.TopDirectoryOnly)
                        .OrderBy(f => f.Name, new NaturalSortComparer())
                        .Select(f => f.FullName);
            }
            else
            {
                files = from file in directoryInfo.EnumerateFiles(ext, SearchOption.TopDirectoryOnly)
                        select file.FullName;
            }

            this.fileNames = files.ToArray();
        }

        private static IImage cvLoader(string fileName)
        {
            var cvImg = CvHighGuiInvoke.cvLoadImage(fileName, ImageLoadType.Unchanged);
            var image = IplImage.FromPointer(cvImg).AsImage((_) => 
                                                      {
                                                          if (cvImg == IntPtr.Zero) return;
                                                          CvHighGuiInvoke.cvReleaseImage(ref cvImg);
                                                      });

            return image;
        }

        #endregion

        /// <summary>
        /// Open the current stream. This overload does not do anything.
        /// </summary>
        public override void Open() { }

        /// <summary>
        /// Closes the current stream. This overload resets position to zero.
        /// </summary>
        public override void Close()
        {
            currentFrame = 0;
        }

        object syncObj = new object();
        protected override bool ReadInternal(out IImage image)
        {
            lock (syncObj)
            {
                image = default(IImage);

                if (this.Position >= this.Length)
                    return false;

                image = loader(fileNames[currentFrame]);
                currentFrame++;
            }

            return true;
        }

        /// <summary>
        /// Gets the total number of files in the specified directory which match the specified search criteria.
        /// </summary>
        public override long Length { get { return fileNames.Length; } }

        /// <summary>
        /// Gets the current position within the stream.
        /// </summary>
        public override long Position { get { return currentFrame; } }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A frame index offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            this.currentFrame = base.Seek(offset, origin);
            return Math.Max(0, Math.Min(currentFrame, this.Length));
        }


        #region Specific function

        /// <summary>
        /// Gets the current image file name.
        /// <para>If the position of the stream is equal to the stream length null is returned.</para>
        /// </summary>
        public string CurrentImageName
        {
            get { return (this.Position < fileNames.Length) ? fileNames[this.Position] : null; }
        }

        #endregion
    }
}
