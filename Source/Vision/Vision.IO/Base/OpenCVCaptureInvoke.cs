using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Accord.Extensions.Vision
{
    internal static class CvCaptureInvoke
    {
        public const CallingConvention CvCallingConvetion = CallingConvention.Cdecl;
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

        public static Size GetImageSize(IntPtr capturePtr)
        {
            return new Size
            {
                Width = (int)CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FrameWidth),
                Height = (int)CvCaptureInvoke.cvGetCaptureProperty(capturePtr, CaptureProperty.FrameHeight)
            };
        }

        public static bool SetImageSize(IntPtr capturePtr, Size newSize)
        {
            bool success;
            success = CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.FrameWidth, newSize.Width);
            success &= CvCaptureInvoke.cvSetCaptureProperty(capturePtr, CaptureProperty.FrameHeight, newSize.Height);

            return success;
        }


        static CvCaptureInvoke()
        {
            Platform.AddDllSearchPath();
        }
    }

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
}
