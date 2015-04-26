#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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
using System.Runtime.InteropServices;
using System.Security;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Fonts
    /// </summary>
    public enum FontTypes
    {
        /// <summary>
        /// HERSHEY_SIMPLEX
        /// </summary>
        HERSHEY_SIMPLEX = 0,
        /// <summary>
        /// HERSHEY_PLAIN
        /// </summary>
        HERSHEY_PLAIN = 1,
        /// <summary>
        /// HERSHEY_DUPLEX
        /// </summary>
        HERSHEY_DUPLEX = 2,
        /// <summary>
        /// HERSHEY_COMPLEX
        /// </summary>
        HERSHEY_COMPLEX = 3,
        /// <summary>
        /// HERSHEY_TRIPLEX
        /// </summary>
        HERSHEY_TRIPLEX = 4,
        /// <summary>
        /// HERSHEY_COMPLEX_SMALL
        /// </summary>
        HERSHEY_COMPLEX_SMALL = 5,
        /// <summary>
        /// HERSHEY_SCRIPT_SIMPLEX
        /// </summary>
        HERSHEY_SCRIPT_SIMPLEX = 6,
        /// <summary>
        /// HERSHEY_SCRIPT_COMPLEX
        /// </summary>
        HERSHEY_SCRIPT_COMPLEX = 7
    }

    /// <summary>
    /// Managed structure equivalent to CvFont
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Font
    {
        /// <summary>
        /// For QT
        /// </summary>
        private IntPtr fontName;
        /// <summary>
        /// For QT
        /// </summary>
        private CvScalar color;

        ///<summary>
        /// Font type
        ///</summary>
        public FontTypes FontType;
        ///<summary>
        /// font data and metrics 
        ///</summary>
        private IntPtr ascii;
        /// <summary>
        /// 
        /// </summary>
        private IntPtr greek;
        /// <summary>
        /// 
        /// </summary>
        private IntPtr cyrillic;
        /// <summary>
        /// hscale
        /// </summary>
        public float HorizontalScale;
        /// <summary>
        /// vscale
        /// </summary>
        public float VerticalScale;
        ///<summary>
        /// slope coefficient: 0 - normal, >0 - italic 
        ///</summary>
        public float Shear;
        ///<summary>
        /// letters thickness 
        ///</summary>
        public int Thickness;
        ///<summary>
        /// horizontal interval between letters 
        ///</summary>
        private float dx;
        /// <summary>
        /// type of line
        /// </summary>
        private LineTypes LineType;

        /// <summary>
        /// Create a Font of the specific type, horizontal scale and vertical scale
        /// </summary>
        /// <param name="type">The type of the font</param>
        /// <param name="hscale">The horizontal scale of the font</param>
        /// <param name="vscale">the vertical scale of the fonr</param>
        public Font(FontTypes type, double hscale, double vscale)
            : this()
        {
            CvCoreInvoke.cvInitFont(ref this, type, hscale, vscale, 0, 1, LineTypes.EightConnected);
        }

        /// <summary>
        /// Calculates the binding rectangle for the given text string when the font is used
        /// </summary>
        /// <param name="text">Input string</param>
        /// <param name="baseline">y-coordinate of the baseline relatively to the bottom-most text point</param>
        /// <returns>size of the text string. Height of the text does not include the height of character parts that are below the baseline</returns>
        public Size GetTextSize(string text, int baseline)
        {
            var size = new Size();
            CvCoreInvoke.cvGetTextSize(text, ref this, ref size, ref baseline);
            return size;
        }

        /// <summary>
        /// Hershey duplex font with vertical scale of two.
        /// </summary>
        public static Font Big = new Font(FontTypes.HERSHEY_DUPLEX, 1, 2);
        /// <summary>
        /// Hershey duplex font with vertical scale of 0.5.
        /// </summary>
        public static Font Normal = new Font(FontTypes.HERSHEY_DUPLEX, 1, 0.5f);
        /// <summary>
        /// Hershey duplex font with vertical scale of 0.25.
        /// </summary>
        public static Font Small = new Font(FontTypes.HERSHEY_DUPLEX, 1, 0.25f); 
    }

    /// <summary>
    /// CV's 4 element vector.
    /// </summary>
    internal struct CvScalar
    {
        /// <summary>
        /// First value.
        /// </summary>
        public double V0;
        /// <summary>
        /// Second value.
        /// </summary>
        public double V1;
        /// <summary>
        /// Third value.
        /// </summary>
        public double V2;
        /// <summary>
        /// Fourth value.
        /// </summary>
        public double V3;
    }

    /// <summary>
    /// The type of line for drawing
    /// </summary>
    internal enum LineTypes
    {
        /// <summary>
        /// 8-connected
        /// </summary>
        EightConnected = 8,
        /// <summary>
        /// 4-connected
        /// </summary>
        FourConnected = 4,
        /// <summary>
        /// Antialias
        /// </summary>
        Antialiased = 16
    }

    /// <summary>
    /// Internal class for OpenCV highgui library invocation.
    /// </summary>
    internal static class CvCoreInvoke
    {
        public const CallingConvention CvCallingConvention = CallingConvention.Cdecl;
        public const string OPENCV_CORE_LIBRARY = "opencv_core248";

        /// <summary>
        /// Draws the line segment between pt1 and pt2 points in the image. The line is clipped by the image or ROI rectangle. For non-antialiased lines with integer coordinates the 8-connected or 4-connected Bresenham algorithm is used. Thick lines are drawn with rounding endings. Antialiased lines are drawn using Gaussian filtering.
        /// </summary>
        /// <param name="img">The image</param>
        /// <param name="pt1">First point of the line segment</param>
        /// <param name="pt2">Second point of the line segment</param>
        /// <param name="color">Line color</param>
        /// <param name="thickness">Line thickness. </param>
        /// <param name="lineType">Type of the line:
        /// 8 (or 0) - 8-connected line.
        /// 4 - 4-connected line.
        /// CV_AA - antialiased line. 
        /// </param>
        /// <param name="shift">Number of fractional bits in the point coordinates</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvLine(IplImage* img, Point pt1, Point pt2,
                                                CvScalar color, int thickness,
                                                LineTypes lineType, int shift);

        /// <summary>
        /// Draws a single or multiple polygonal curves
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="pts">Array of pointers to polylines</param>
        /// <param name="npts">Array of polyline vertex counters</param>
        /// <param name="contours">Number of polyline contours</param>
        /// <param name="isClosed">
        /// Indicates whether the polylines must be drawn closed. 
        /// If !=0, the function draws the line from the last vertex of every contour to the first vertex.
        /// </param>
        /// <param name="color">Polyline color</param>
        /// <param name="thickness">Thickness of the polyline edges</param>
        /// <param name="lineType">Type of the line segments, see cvLine description</param>
        /// <param name="shift">Number of fractional bits in the vertex coordinates</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvPolyLine(IplImage* img, [In]IntPtr[] pts, [In]int[] npts, int contours, [MarshalAs(UnmanagedType.Bool)]  bool isClosed,
                                                    CvScalar color, int thickness, 
                                                    LineTypes lineType, int shift);

        /// <summary>
        /// Draws a rectangle with two opposite corners pt1 and pt2
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="pt1">A rectangle vertex</param>
        /// <param name="pt2">Opposite rectangle vertex</param>
        /// <param name="color">Line color </param>
        /// <param name="thickness">Thickness of lines that make up the rectangle. Negative values make the function to draw a filled rectangle.</param>
        /// <param name="lineType">Type of the line</param>
        /// <param name="shift">Number of fractional bits in the point coordinates</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvRectangle(IplImage* img, Point pt1, Point pt2,
                                                     CvScalar color, int thickness,
                                                     LineTypes lineType, int shift);

        /// <summary>
        /// Draws a rectangle specified by a CvRect structure
        /// </summary>
        /// /// <param name="img">Image</param>
        /// <param name="rect">The rectangle to be drawn</param>
        /// <param name="color">Line color </param>
        /// <param name="thickness">Thickness of lines that make up the rectangle. Negative values make the function to draw a filled rectangle.</param>
        /// <param name="lineType">Type of the line</param>
        /// <param name="shift">Number of fractional bits in the point coordinates</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvRectangleR(IplImage* img, Rectangle rect,
                                                      CvScalar color, int thickness,
                                                      LineTypes lineType, int shift);

        /// <summary>
        /// Draws a simple or filled circle with given center and radius. The circle is clipped by ROI rectangle.
        /// </summary>
        /// <param name="img">Image where the circle is drawn</param>
        /// <param name="center">Center of the circle</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="color">Color of the circle</param>
        /// <param name="thickness">Thickness of the circle outline if positive, otherwise indicates that a filled circle has to be drawn</param>
        /// <param name="lineType">Type of the circle boundary</param>
        /// <param name="shift">Number of fractional bits in the center coordinates and radius value</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvCircle(IplImage* img, Point center, int radius,
                                                  CvScalar color, int thickness,
                                                  LineTypes lineType, int shift);

        /// <summary>
        /// Draws a simple or thick elliptic arc or fills an ellipse sector. The arc is clipped by ROI rectangle. A piecewise-linear approximation is used for antialiased arcs and thick arcs. All the angles are given in degrees.
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="center">Center of the ellipse</param>
        /// <param name="axes">Length of the ellipse axes</param>
        /// <param name="angle">Rotation angle</param>
        /// <param name="startAngle">Starting angle of the elliptic arc</param>
        /// <param name="endAngle">Ending angle of the elliptic arc</param>
        /// <param name="color">Ellipse color</param>
        /// <param name="thickness">Thickness of the ellipse arc</param>
        /// <param name="lineType">Type of the ellipse boundary</param>
        /// <param name="shift">Number of fractional bits in the center coordinates and axes' values</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvEllipse(IplImage* img, Point center, Size axes, double angle, double startAngle, double endAngle,
                                                   CvScalar color, int thickness,
                                                   LineTypes lineType, int shift);

        /// <summary>
        /// Draws contour outlines in the image if thickness &gt;=0 or fills area bounded by the contours if thickness&lt;0.
        /// </summary>
        /// <param name="img">Image where the contours are to be drawn. Like in any other drawing function, the contours are clipped with the ROI</param>
        /// <param name="contour">Pointer to the first contour</param>
        /// <param name="externalColor">Color of the external contours</param>
        /// <param name="holeColor">Color of internal contours </param>
        /// <param name="maxLevel">Maximal level for drawn contours. If 0, only contour is drawn. If 1, the contour and all contours after it on the same level are drawn. If 2, all contours after and all contours one level below the contours are drawn, etc. If the value is negative, the function does not draw the contours following after contour but draws child contours of contour up to abs(maxLevel)-1 level. </param>
        /// <param name="thickness">Thickness of lines the contours are drawn with. If it is negative the contour interiors are drawn</param>
        /// <param name="lineType">Type of the contour segments</param>
        /// <param name="offset">Shift all the point coordinates by the specified value. It is useful in case if the contours retrived in some image ROI and then the ROI offset needs to be taken into account during the rendering. </param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvDrawContours(IplImage* img, IntPtr contour,
                                                        CvScalar externalColor, CvScalar holeColor, int maxLevel, int thickness,
                                                        LineTypes lineType, Point offset);

        /// <summary>
        /// Fills convex polygon interior. This function is much faster than The function cvFillPoly and can fill not only the convex polygons but any monotonic polygon, i.e. a polygon whose contour intersects every horizontal line (scan line) twice at the most
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="pts">Array of pointers to a single polygon</param>
        /// <param name="npts">Polygon vertex counter</param>
        /// <param name="color">Polygon color</param>
        /// <param name="lineType">Type of the polygon boundaries</param>
        /// <param name="shift">Number of fractional bits in the vertex coordinates</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvFillConvexPoly(IplImage* img, [In] Point[] pts, int npts,
                                                          CvScalar color, 
                                                          LineTypes lineType, int shift);

        #region Text
        /// <summary>
        /// Initializes the font structure that can be passed to text rendering functions
        /// </summary>
        /// <param name="font">Pointer to the font structure initialized by the function</param>
        /// <param name="fontFace">Font name identifier. Only a subset of Hershey fonts are supported now</param>
        /// <param name="hscale">Horizontal scale. If equal to 1.0f, the characters have the original width depending on the font type. If equal to 0.5f, the characters are of half the original width</param>
        /// <param name="vscale">Vertical scale. If equal to 1.0f, the characters have the original height depending on the font type. If equal to 0.5f, the characters are of half the original height</param>
        /// <param name="shear">Approximate tangent of the character slope relative to the vertical line. Zero value means a non-italic font, 1.0f means 45 slope, etc. thickness Thickness of lines composing letters outlines. The function cvLine is used for drawing letters</param>
        /// <param name="thickness">Thickness of the text strokes</param>
        /// <param name="lineType">Type of the strokes</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvInitFont(ref Font font, FontTypes fontFace, double hscale, double vscale, double shear,
                                                    int thickness, LineTypes lineType);

        /// <summary>
        /// Renders the text in the image with the specified font and color. The printed text is clipped by ROI rectangle. Symbols that do not belong to the specified font are replaced with the rectangle symbol.
        /// </summary>
        /// <param name="img">Input image</param>
        /// <param name="text">String to print</param>
        /// <param name="org">Coordinates of the bottom-left corner of the first letter</param>
        /// <param name="font">Pointer to the font structure</param>
        /// <param name="color">Text color</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public unsafe static extern void cvPutText(IplImage* img, [MarshalAs(UnmanagedType.LPStr)] String text, Point org, ref Font font, CvScalar color);

        /// <summary>
        /// Calculates the binding rectangle for the given text string when a specified font is used
        /// </summary>
        /// <param name="textString">Input string</param>
        /// <param name="font">The font structure</param>
        /// <param name="textSize">Resultant size of the text string. Height of the text does not include the height of character parts that are below the baseline</param>
        /// <param name="baseline">y-coordinate of the baseline relatively to the bottom-most text point</param>
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvention)]
        public static extern void cvGetTextSize([MarshalAs(UnmanagedType.LPStr)] String textString, ref Font font, ref Size textSize, ref int baseline);
        #endregion

        static CvCoreInvoke()
        {
            Platform.AddDllSearchPath();
        }
    }

   
}
