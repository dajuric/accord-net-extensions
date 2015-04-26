using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides extensions for color format conversion to and from CvSclalar.
    /// </summary>
    internal static class ColorCvScalarConversionExtensions
    {
        public static CvScalar ToCvScalar(this Bgr<byte> color, byte opacity = Byte.MaxValue)
        {
            return new CvScalar{ V0 = color.B, V1 = color.G, V2 = color.R, V3 = opacity };
        }

        public static CvScalar ToCvScalar(this Bgra<byte> color)
        {
            return new CvScalar{ V0 = color.B, V1 = color.G, V2 = color.R, V3 = color.A };
        }
    }

    /// <summary>
    /// Provides extension drawing methods which operate on color RGB/RGBA images.
    /// </summary>
    public static class Drawing
    {
        #region Rectangle

        /// <summary>
        /// Draws rectangle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="rect">Rectangle.</param>
        /// <param name="color">Object's color.</param>
        /// <param name="thickness">Border thickness. If less than zero structure will be filled.</param>
        /// <param name="opacity">Sets alpha channel where 0 is transparent and 255 is full opaque.</param>
        public unsafe static void Draw(this Bgr<byte>[,] image, Rectangle rect, Bgr<byte> color, int thickness, byte opacity = Byte.MaxValue)
        {
            if (float.IsNaN(rect.X) || float.IsNaN(rect.Y))
                return;

            using(var img = image.Lock())
            {
                var iplImage = img.AsOpenCvImage();
                CvCoreInvoke.cvRectangleR(&iplImage, rect, color.ToCvScalar(opacity), thickness, LineTypes.EightConnected, 0);
            }
        }

        #endregion

        #region Text

        /// <summary>
        /// Draws text on the provided image.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="text">User text.</param>
        /// <param name="font">Font.</param>
        /// <param name="botomLeftPoint">Bottom-left point.</param>
        /// <param name="color">Text color.</param>
        /// <param name="opacity">Sets alpha channel where 0 is transparent and 255 is full opaque.</param>
        public unsafe static void Draw(this Bgr<byte>[,] image, string text, Font font, Point botomLeftPoint, Bgr<byte> color, byte opacity = Byte.MaxValue)
        {
            using(var img = image.Lock())
            {
                var iplImage = img.AsOpenCvImage();
                CvCoreInvoke.cvPutText(&iplImage, text, botomLeftPoint, ref font, color.ToCvScalar());
            }
        }

        #endregion

        #region Box & Ellipse

        /// <summary>
        /// Draws Box2D.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="box">Box 2D.</param>
        /// <param name="color">Object's color.</param>
        /// <param name="thickness">Border thickness.</param>
        /// <param name="opacity">Sets alpha channel where 0 is transparent and 255 is full opaque.</param>
        public unsafe static void Draw(this Bgr<byte>[,] image, Box2D box, Bgr<byte> color, int thickness, byte opacity = Byte.MaxValue)
        {
            if (thickness < 1)
                throw new NotSupportedException("Only positive values are valid!");

            var vertices = box.GetVertices();

            using(var img = image.Lock())
            {
                var iplImage = img.AsOpenCvImage();

                for (int i = 0; i < vertices.Length; i++)
                {
                    int idx2 = (i + 1) % vertices.Length;

                    CvCoreInvoke.cvLine(&iplImage, vertices[i].Round(), vertices[idx2].Round(), 
                                           color.ToCvScalar(opacity), thickness, 
                                           LineTypes.EightConnected, 0);
                }
            }
        }

        /// <summary>
        /// Draws ellipse.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="ellipse">Ellipse.</param>
        /// <param name="color">Object's color.</param>
        /// <param name="thickness">Border thickness.</param>
        public unsafe static void Draw(this Bgr<byte>[,] image, Ellipse ellipse, Bgr<byte> color, int thickness)
        {
            using(var img = image.Lock())
            {
                var iplImage = img.AsOpenCvImage();
                CvCoreInvoke.cvEllipse(&iplImage, ellipse.Center.Round(), Size.Round(ellipse.Size), ellipse.Angle, 
                                          0, 2*System.Math.PI, color.ToCvScalar(), thickness, LineTypes.EightConnected, 0);
            }
        }

        #endregion

        #region Contour

        /// <summary>
        /// Draws contour.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="contour">Contour points.</param>
        /// <param name="color">Contour color.</param>
        /// <param name="thickness">Contours thickness.</param>
        /// <param name="opacity">Sets alpha channel where 0 is transparent and 255 is full opaque.</param>
        public unsafe static void Draw(this Bgr<byte>[,] image, Point[] contour, Bgr<byte> color, int thickness, byte opacity = Byte.MaxValue)
        {
            var contourHandle = GCHandle.Alloc(contour, GCHandleType.Pinned);

            using(var img = image.Lock())
            {
                var iplImage = img.AsOpenCvImage();

                //TODO - noncritical: implement with cvContour
                CvCoreInvoke.cvPolyLine(&iplImage, new IntPtr[]{contourHandle.AddrOfPinnedObject()}, new int[]{ contour.Length}, 1, 
                                           true, color.ToCvScalar(), thickness, LineTypes.EightConnected, 0);
            }

            contourHandle.Free();
        }

        #endregion

        #region Circle

        /// <summary>
        /// Draws circle.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="circle">Circle</param>
        /// <param name="color">Circle color.</param>
        /// <param name="thickness">Contours thickness.</param>
        public unsafe static void Draw(this Bgr<byte>[,] image, Circle circle, Bgr<byte> color, int thickness)
        {
            using(var img = image.Lock())
            {
                var iplImage = img.AsOpenCvImage();
                var center = new Point(circle.X, circle.Y);

                CvCoreInvoke.cvCircle(&iplImage, center, circle.Radius, color.ToCvScalar(), 
                                         thickness, LineTypes.EightConnected, 0);
            }
        }

        /// <summary>
        /// Draws circles.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="circles">Circles</param>
        /// <param name="color">Circle color.</param>
        /// <param name="thickness">Contours thickness.</param>
        public unsafe static void Draw(this Bgr<byte>[,] image, IEnumerable<Circle> circles, Bgr<byte> color, int thickness)
        {
            using(var img = image.Lock())
            {
                var iplImage = img.AsOpenCvImage();

                foreach (var circle in circles)
	            {
		            var center = new Point(circle.X, circle.Y);

                    CvCoreInvoke.cvCircle(&iplImage, center, circle.Radius, color.ToCvScalar(), 
                                             thickness, LineTypes.EightConnected, 0);
	            }
            }
        }

        #endregion

        #region Annotations

        /// <summary>
        /// Draws rectangle annotation.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="rect">User specified area to annotate.</param>
        /// <param name="text">Label.</param>
        /// <param name="font">Font to use. Default is "Arial" of size 10, style: Bold.</param>
        public static void DrawAnnotation(this Bgr<byte>[,] image, Rectangle rect, string text, Font font)
        {
            const int VERTICAL_OFFSET = 5;

            image.Draw(rect, Bgr<byte>.Red, 1);

            var textSize = font.GetTextSize(text, 0);
            var bottomLeftPt = new Point(rect.X + rect.Width / 2 - textSize.Width / 2, rect.Top - VERTICAL_OFFSET);
            image.Draw(text, font, new Point(), Bgr<byte>.Black);
        }

        #endregion
    }
}
