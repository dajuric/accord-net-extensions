using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Accord.Extensions.Vision
{
    public class ImageDirectoryReader : StreamableSource
    {
        string[] fileNames = null;

        Func<string, IImage> loader;
        long currentFrame = 0;

        #region Initialization

        public ImageDirectoryReader(string filePath, string extension, bool useNaturalSorting = true)
            : this(filePath, extension, useNaturalSorting, (x) => cvLoader(x))
        {}

        public ImageDirectoryReader(string filePath, string extension, bool useNaturalSorting, Func<string, IImage> loader)
        {
            this.IsLiveStream = false;
            this.CanSeek = true;

            this.loader = loader;

            string ext = "*." + extension.TrimStart('.', '*');
            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);

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

        public override void Open() { }

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

        public override long Length { get { return fileNames.Length; } }

        public override long Position { get { return currentFrame; } }

        public override long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            this.currentFrame = base.Seek(offset, origin);
            return currentFrame;
        }

        public string CurrentImageName
        {
            get { return fileNames[this.Position]; }
        }
    }
}
