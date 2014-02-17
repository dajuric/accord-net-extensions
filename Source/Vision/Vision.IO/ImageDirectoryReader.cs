using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Accord.Extensions.Vision
{
    public class ImageDirectoryReader<TImage> : StreamableSource<TImage>
        where TImage: IImage
    {
        string[] fileNames = null;

        Func<string, TImage> loader;
        long currentFrame = 0;

        #region Initialization

        public ImageDirectoryReader(string filePath, string extension, Func<string, TImage> loader, bool useNaturalSorting = true)
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

        #endregion

        public override void Open() { }

        public override void Close()
        {
            currentFrame = 0;
        }

        object syncObj = new object();
        protected override bool Read(out TImage image)
        {
            lock (syncObj)
            {
                image = default(TImage);

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
