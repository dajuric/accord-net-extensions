using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Vision
{
    public interface IStreamableSource
    {
        long Length { get; }

        long Position { get; }

        long Seek(long offset, SeekOrigin origin = SeekOrigin.Current);

        void Open();
        void Close();

        IImage Read();
    }

    public static class StreamableSourceExtensions
    {
        public static Image<TColor, TDepth> ReadAs<TColor, TDepth>(this IStreamableSource source)
            where TColor: IColor
            where TDepth: struct
        {
            var image = source.Read();
            return ((GenericImageBase)image).Convert<TColor, TDepth>();
        }
    }
}
