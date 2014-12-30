#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Drawing;
using System.IO;
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Filters;
using AForge.Imaging;
using ColorConverter = Accord.Extensions.Imaging.Converters.ColorDepthConverter;

namespace Test
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        unsafe static void Main()
        {
            Test test = new Test();
            test.Bla();
            return;

            test.TestRunningWeightedVariance();
            return;

            var resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            var imgColor = Bitmap.FromFile(Path.Combine(resourceDir, "testColorBig.jpg")).ToImage<Bgr, byte>();
            imgColor = imgColor.CorrectContrast(105);
            
            /*var bmp1 = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile("nature-spring.jpg");
            var image1 = bmp1.ToImage<Gray, float>();

            var res1 = ResizeNearsetNeighbur.Resize(image1, new Size(640, 480));
            ImageBox.Show("Interpolated image", res1.ToBitmap());

            var res = new Image<Bgr, float>(320, 200);
            image1.GetRectSubPix(new PointF(1.9f, 1.9f), res);
            ImageBox.Show("Interpolated image", res.ToBitmap());*/

            test.TestLKFlow();

          
            return; //uncomment if you want execute functions below

            var bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile("nature-spring.jpg");
            var image = bmp.ToImage<Bgr, byte>();
            UnmanagedImage uIm = UnmanagedImage.FromManagedImage(bmp);

            /********************** Bitmap <-> Image<,> *************************/
            //from Bitmap...
            IImage bmpImg1 = bmp.ToImage(); //generic image (dest format depends on bmp pixel format) (this case: <Color3, byte>)
            Image<Bgr, byte> bmpImg2 = bmp.ToImage<Bgr, byte>(); //in this case additional cast is performed (<Color3, byte> => <Bgr, byte>) (no data convert)
            //to Bitmap...
            Bitmap bmpFromImg = bmpImg2.ToBitmap(copyAlways: false /*do not copy if you do not have to*/); //<Bgr, byte> can be casted to <Color3, byte> therefore data is shared between Bitmap and bmPimg2

            /********************** UnmanagedImage <-> Image<,> *************************/
            //from UnmanagedImage...
            var im1FromUIm = uIm.AsImage(); //generic image (dest format depends on bmp pixel format)
            var im2FromUIm = uIm.ToImage<Bgr, byte>(); //in this case additional cast is performed (<Color3, byte> => <Bgr, byte>) (no data convert)
            //to UnmanagedImage...
            var uIm2 = im1FromUIm.ToAForgeImage(copyAlways: false, failIfCannotCast: false); 
            /******************* some AForge filter recreation... ***********************/

            /********************** Array <-> Image<,> (also eliminates need for Matrix, UnmanagedImage converters) ********************************/ 
            int[,] arr = new int[480,640];
            
            //from Array...
            var image1FromArray = arr.ToImage(); //supported for all 2D/3D arrays
            var castedImageFromArray = arr.AsImage(); //supported only on 2D arrays (data is shared)
            
            //to Array ...
            var arrFromIm = image1FromArray.ToArray(); //output is 2D or 3D array (see function overloads)


            /**************** channel rotate *******************/
            //Image<,> => flexible
            var channels = image.SplitChannels();
            var dest = new Image<Bgr, byte>(new Image<Gray, byte>[] { channels[1], channels[0], channels[2] });

            //AForge 
            AForge.Imaging.Filters.RotateChannels rc = new AForge.Imaging.Filters.RotateChannels();
            rc.Apply(uIm);

            /**************** channel extract *******************/
            //Image<,> => simple
            var ch = image[0];

            //AForge 
            AForge.Imaging.Filters.ExtractChannel ec = new AForge.Imaging.Filters.ExtractChannel(0);
            ec.Apply(uIm);

            /**************** Max (see Min also) *******************/
            //Image<,> 
            image.Max(image, inPlace:true);

            //AForge 
            AForge.Imaging.Filters.Merge m = new AForge.Imaging.Filters.Merge(uIm);
            m.Apply(uIm);

            /**************** Sobel *******************/
            var bmpSquareGray = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile("square.bmp");
            var grayIm = bmpSquareGray.ToImage<Gray, float>(); //currently there are no overloads for magnitude for <byte>, will be fixed later

            //Image<,> => flexible
            var sobelX = grayIm.Sobel(1, 0, 3);
            var sobelY = grayIm.Sobel(0, 1, 3);
            var mag = sobelX.Magnitude(sobelY); //should use Threshold for values > 255 (not implemented yet)
            //mag.ToBitmap().Save("bla.bmp");  //img.Save(..) is available for <IColor3, byte> and <Gray, byte> (Bitmap compatible formats) should change ?
            //var mag = sobelX.Abs().Add(sobelY.Abs()).Scale(0, 255).Convert<Gray, byte>(); //should work later (it is not implemented)

            //AForge 
            AForge.Imaging.Filters.SobelEdgeDetector sobel = new AForge.Imaging.Filters.SobelEdgeDetector();
            var destSobel = sobel.Apply(grayIm.ToAForgeImage());

            //destSobel.ToManagedImage().Save("sobelAForge.bmp");
            return;
         }

    }

}
