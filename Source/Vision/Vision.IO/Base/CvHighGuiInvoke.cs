using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Accord.Extensions.Vision
{
    /// <summary>
    /// Internal class for OpenCV highgui library invocation.
    /// </summary>
    internal static class CvHighGuiInvoke
    {
        public const CallingConvention CvCallingConvetion = CallingConvention.Cdecl;
        public const string OPENCV_CORE_LIBRARY = "opencv_core248";
        public const string OPENCV_HIGHGUI_LIBRARY = "opencv_highgui248";

        [SuppressUnmanagedCodeSecurity]
        [DllImport(OPENCV_HIGHGUI_LIBRARY, CallingConvention = CvCallingConvetion)]
        public static extern IntPtr cvCreateCameraCapture(int index);

        [DllImport(OPENCV_HIGHGUI_LIBRARY, CallingConvention = CvCallingConvetion)]
        public static extern IntPtr cvCreateFileCapture([MarshalAs(UnmanagedType.LPStr)] string filename);


        [SuppressUnmanagedCodeSecurity]
        [DllImport(OPENCV_HIGHGUI_LIBRARY, CallingConvention = CvCallingConvetion)]
        public static extern void cvReleaseCapture(ref IntPtr capture);


        [SuppressUnmanagedCodeSecurity]
        [DllImport(OPENCV_HIGHGUI_LIBRARY, CallingConvention = CvCallingConvetion)]
        public static extern int cvGrabFrame(IntPtr capture);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(OPENCV_HIGHGUI_LIBRARY, CallingConvention = CvCallingConvetion)]
        public static extern IntPtr cvQueryFrame(IntPtr capture);


        [SuppressUnmanagedCodeSecurity]
        [DllImport(OPENCV_HIGHGUI_LIBRARY, CallingConvention = CvCallingConvetion)]
        public static extern double cvGetCaptureProperty(IntPtr capture, CaptureProperty propertyId);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(OPENCV_HIGHGUI_LIBRARY, CallingConvention = CvCallingConvetion)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool cvSetCaptureProperty(IntPtr capture, CaptureProperty propertyId, double value);

        public static Int32Size GetImageSize(IntPtr capturePtr)
        {
            return new Int32Size
            {
                Width = (int)CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FrameWidth),
                Height = (int)CvHighGuiInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FrameHeight)
            };
        }

        public static bool SetImageSize(IntPtr capturePtr, Int32Size newSize)
        {
            bool success;
            success = CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.FrameWidth, newSize.Width);
            success &= CvHighGuiInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.FrameHeight, newSize.Height);

            return success;
        }


        /************************************************ image IO ****************************************************/

        [SuppressUnmanagedCodeSecurity]
        [DllImport(OPENCV_HIGHGUI_LIBRARY, CallingConvention = CvCallingConvetion)]
        public static extern IntPtr cvLoadImage([MarshalAs(UnmanagedType.LPStr)] String filename, ImageLoadType loadType);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(OPENCV_CORE_LIBRARY, CallingConvention = CvCallingConvetion)]
        public static extern void cvReleaseImage(ref IntPtr image);
        /************************************************ image IO ****************************************************/


        static CvHighGuiInvoke()
        {
            Platform.AddDllSearchPath();
        }
    }

    /// <summary>
    /// OpenCV capture properties for camera and video.
    /// </summary>
    internal enum CaptureProperty: int
    {
        PosFrames = 1,
        FrameWidth = 3,
        FrameHeight = 4,
        FPS = 5,
        FrameCount = 7,

        /************** camera properties ******************/
        Brightness = 10,
        Contrast = 11,
        Saturation = 12,
        Hue = 13,
        Gain = 14,
        Exposure = 15,
        /************** camera properties ******************/

        ConvertRGB = 16
    }

    /// <summary>
    /// OpenCV image load mode.
    /// </summary>
    internal enum ImageLoadType: int
    {
        /// <summary>
        /// Loads the image as is (including the alpha channel if present)
        /// </summary>
        Unchanged = -1,

        /// <summary>
        /// Loads the image as an intensity one
        /// </summary>
        Grayscale = 0,

        /// <summary>
        ///  Loads the image in the RGB format
        /// </summary>
        Color = 1,
    }
}
