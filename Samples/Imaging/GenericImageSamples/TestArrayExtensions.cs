using Accord.Extensions.Imaging;
using System.Diagnostics;

namespace GenericImage
{
    public partial class Test
    {
        public static void TestArrayToImage()
        {
            byte[, ,] arr = new byte[3, 480, 640];
            arr[0, 5, 5] = 10;
            arr[1, 5, 5] = 20;
            arr[2, 5, 5] = 30;
            Image<Bgr, byte> img = arr.ToImage<Bgr, byte>();

            Debug.Assert(img[5, 5].B == arr[0, 5, 5] && 
                         img[5, 5].G == arr[1, 5, 5] && 
                         img[5, 5].R == arr[2, 5, 5]);

            //Console.Writeline(img.ToArray()); //convert back
            //img.Save("arrayToImgTest.png");
        }

        public static void TestArrayAsImageGray()
        {
            byte[,] arr = new byte[480, 640];
            arr[5, 5] = 10;
            Image<Gray, byte> img = arr.AsImage();
           
            Debug.Assert(img[5, 5].Intensity == arr[5, 5]);
         
            //Console.Writeline(img.ToArray()); //convert back
            //img.Save("arrayAsImgGrayTest.png");
        }
    }
}
