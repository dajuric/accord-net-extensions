using System;
using System.Drawing;

namespace Accord.Imaging
{
    public interface IImage: IDisposable
    {
        IntPtr ImageData { get; }
        int Width { get; }
        int Height { get; }
        int Stride { get; }
        Size Size { get; }
        ColorInfo ColorInfo { get; }

        IntPtr GetData(int row, int col);
        IntPtr GetData(int row);
        IImage GetSubRect(Rectangle rect);
        IImage Clone();
        IImage CopyBlank();

        IImage this[int channelIdx] { get; set; }
    }
}
