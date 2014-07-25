using System.Diagnostics;
using Accord.Extensions.Imaging;
using System;
using System.Drawing;

namespace GenericImageInteropDemo
{
    public partial class Test
    {
        public static void TestImageToBitmap()
        {
            Console.WriteLine("Image<,> to Bitmap (data sharing)");
            Image<Bgr, byte> img = new Image<Bgr, byte>(640, 480);
            img[10, 10] = Bgr8.Red;

            var bmp = img.ToBitmap(copyAlways: false, failIfCannotCast: true); //ensure data sharing (data is shared by default if possible, otherwise conversion is done)


            //try with image of type System.Single.
            Image<Bgr, float> img2 = new Image<Bgr, float>(640, 480);

            Console.WriteLine("Image<,> to Bitmap (data sharing forced)");
            try
            {
                var bmp2 = img2.ToBitmap(failIfCannotCast: true); //will fail
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void TestBitmapToImage()
        {
            var bmp = new Bitmap(640, 480, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

            //ToImage() methods are the default way for image loading as the Image<,> is platform abstract
            //user can use other loaders such as OpenCV loader  (conversion from IplImage)
            var img = bmp.ToImage();
            Console.WriteLine("Bitmap to Image<,> automatically: Bitmap pixel format {0} is transformed into: {1}.", bmp.PixelFormat, img.ColorInfo);

            var img2 = bmp.ToImage<Bgr, int>();
            Console.WriteLine("Bitmap to Image<,> forced: Bitmap pixel format {0} is transformed into: {1}.", bmp.PixelFormat, img2.ColorInfo);
        }
    }
}
