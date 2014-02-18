using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Provides a streamable view of an image directory.
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
        public ImageDirectoryReader(string dirPath, string extension, bool useNaturalSorting, Func<string, IImage> loader = null)
        {
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
                        .OrderBy(f => f.Name, new NaturalSortComparer<string>())
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
        protected override bool Read(out IImage image)
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
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            this.currentFrame = base.Seek(offset, origin);
            return Math.Max(0, Math.Min(currentFrame, this.Length));
        }

        /// <summary>
        /// Gets the current image file name.
        /// </summary>
        public string CurrentImageName
        {
            get { return fileNames[this.Position]; }
        }
    }
}
