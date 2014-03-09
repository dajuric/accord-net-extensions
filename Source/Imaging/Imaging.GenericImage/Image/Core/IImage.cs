using System;
using System.Drawing;

namespace Accord.Extensions.Imaging
{
    public interface IImage: IDisposable
    {
        IntPtr ImageData { get; }
        int Width { get; }
        int Height { get; }
        int Stride { get; }
        Int32Size Size { get; }
        ColorInfo ColorInfo { get; }

        IntPtr GetData(int row, int col);
        IntPtr GetData(int row);
        IImage GetSubRect(Int32Rect rect);
        IImage Clone();
        IImage CopyBlank();

        IImage this[int channelIdx] { get; set; }
    }
}
