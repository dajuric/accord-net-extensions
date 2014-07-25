using System.Diagnostics;
using Accord.Extensions.Imaging;
using System;

namespace GenericImageInteropDemo
{
    public partial class Test
    {
        public static void ImageToOpenCVIm()
        {
            var img = new Image<Bgr, int>(640, 480);

            IplImage iplImage = img.AsOpenCvImage(); 
            Console.WriteLine("Image<,> to IplImage: Color {0} is transformed into IplImage depth: {1}.", img.ColorInfo, iplImage.ChannelDepth);
        }

        public static void OpenCVImToImage()
        {
            var img = new Image<Bgr, float>(640, 480);
            IplImage iplImage = img.AsOpenCvImage();

            var convertedImg = iplImage.AsImage(); //define destructor optionally
            Console.WriteLine("IplImage to Image<,> automatically: IplImage depth {0} is transformed into: {1}.",  iplImage.ChannelDepth, convertedImg.ColorInfo);
        }
    }
}
