using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Vision
{
    public class ImageDirectoryReader: IStreamableSource
    {
        string[] fileNames = null;

        Func<string, IImage> loader;
        long currentFrame = 0;

        #region Initialization

        public ImageDirectoryReader(string filePath, string extension, Func<string, IImage> loader, bool useNaturalSorting = true)
        {
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

        public void Open()
        {}

        public void Close()
        {
            currentFrame = 0;
        }

        public Image<TColor, TDepth> Read<TColor, TDepth>()
            where TColor: IColor
            where TDepth: struct
        {
            var image = Read();
            return ((GenericImageBase)image).Convert<TColor, TDepth>();
        }

        public IImage Read()
        {
            if (currentFrame >= fileNames.Length)
            {
                return null;
            }

            IImage image = loader(fileNames[currentFrame]);

            currentFrame++;

            return image;
        }

        public long Length
        {
            get
            {
                return fileNames.Length;
            }
        }

        public long Position
        {
            get
            {
                return currentFrame;
            }
        }

        public long Seek(long offset, SeekOrigin origin = SeekOrigin.Current)
        {
            long newPosition = 0;
            switch (origin)
            { 
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;
                case SeekOrigin.Current:
                    newPosition = this.Position + offset;
                    break;
                case SeekOrigin.End:
                    newPosition = this.Length + offset;
                    break;
            }

            currentFrame = System.Math.Min(this.Length, System.Math.Max(0, newPosition));
            return currentFrame;
        }

       public string CurrentImageName
        {
            get { return fileNames[this.Position]; }
       }
    }
}
