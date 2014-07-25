using System.Diagnostics;
using Accord.Extensions.Imaging;
using System;
using AForge.Imaging;
using System.Drawing;

namespace GenericImageInteropDemo
{
    public partial class Test
    {
        public static void AForgeImToImage()
        { 
            var bmp = new Bitmap(640, 480, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            UnmanagedImage uIm = UnmanagedImage.FromManagedImage(bmp);

            var img = uIm.AsImage();
            Console.WriteLine("UnmanagedImage to Image<,> automatically: Bitmap pixel format {0} is transformed into: {1}.", uIm.PixelFormat, img.ColorInfo);

            var img2 = uIm.ToImage<Bgr, byte>();
            Console.WriteLine("UnmanagedImage to Image<,> forced: Bitmap pixel format {0} is transformed into: {1}.", uIm.PixelFormat, img.ColorInfo);
        }

        public static void ImageToAForgeIm()
        {
            var img = new Image<Bgr, byte>(640, 480);

            UnmanagedImage uIm = img.ToAForgeImage(); //data is shared if possible
            Console.WriteLine("Image<,> to UnmanagedImage: Color {0} is transformed into bitmap pixel format: {1}. Data is {2} shared.", img.ColorInfo, uIm.PixelFormat, img.CanCastToAForgeImage() ? "": "not");
        }
    }
}
