using Accord.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using System.Threading;
using Accord.Imaging.Helper;

namespace Accord.Vision
{
    /// <summary>
    /// Video capture class. (I NEED HELP WITH THIS). TODO: This implementation should be in separate assembly (ies - regarding how many architectures should be supported) if the Accord.NET should be used on different architectures.
    /// </summary>
    public class Capture: CaptureBase
    {    
        IVideoSource videoSource;
        
        /// <summary>
        /// Creates capture from camera which has index: <see cref="cameraIdx"/>
        /// </summary>
        /// <param name="cameraIdx">Camera index.</param>
        public Capture(int cameraIdx = 0)
            :base(cameraIdx)
        {
            var videoDevices = GetVideoDevices();
            if (cameraIdx > (videoDevices.Length - 1))
                throw new Exception(string.Format("No camera found at index {0}", cameraIdx));

            FilterInfo videoDevice = videoDevices[cameraIdx];
            var captureDevice = new VideoCaptureDevice(videoDevice.MonikerString);
            videoSource = captureDevice;
            initalize();
        }

        /// <summary>
        /// Creates capture from <see cref="filerInfo"/>.
        /// </summary>
        /// <param name="filterInfo">Filter info.</param>
        public Capture(FilterInfo filterInfo)
        {
            videoSource = new VideoCaptureDevice(filterInfo.MonikerString);
            initalize();
        }

        /// <summary>
        /// Creates capture from video file.
        /// </summary>
        /// <param name="fileName">Video file name.</param>
        public Capture(string fileName)
        {
            var captureDevice = new FileVideoSource(fileName);
            videoSource = captureDevice;
            initalize();
        }

        private void initalize()
        {
            videoSource.NewFrame += videoSource_NewFrame;
        }

        /// <summary>
        /// Gets all supported video devices.
        /// </summary>
        /// <returns>FilterInfo array.</returns>
        public static FilterInfo[] GetVideoDevices()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            
            var devicesArr = new FilterInfo[videoDevices.Count];
            for (int i = 0; i < videoDevices.Count; i++)
            {
                devicesArr[i] = videoDevices[i];
            }

            return devicesArr;
        }

        /// <summary>
        /// Starts capture.
        /// </summary>
        public override void Start()
        {
            //buffer = new Image<Bgr, byte>(this.VideoSize);
            videoSource.Start();
        }

        /// <summary>
        /// Stops capture.
        /// </summary>
        public override void Stop()
        { 
            videoSource.Stop();
        }

        /// <summary>
        /// Get current video resolution. 
        /// Set the user defined resolution or the closest one if exact is not supported.
        /// </summary>
        public override Size VideoSize 
        {
            get 
            {
                if (videoSource is VideoCaptureDevice)
                {
                    return ((VideoCaptureDevice)videoSource).VideoResolution.FrameSize;
                }
                else
                {
                    return Size.Empty;
                }
            }
            set
            {
                if (videoSource is VideoCaptureDevice)
                {
                    var videoCapture = (VideoCaptureDevice)videoSource;
                    videoCapture.VideoResolution = getClosestMatchingResolution(videoCapture.VideoCapabilities, value);
                }
                else
                {
                    //not supported
                }
            }
        }

        private VideoCapabilities getClosestMatchingResolution(VideoCapabilities[] capabilities, Size frameSize)
        {
            Func<Size, float> similarityFunc = (size) => 
            {
                int dist = System.Math.Abs(frameSize.Width - size.Width) + System.Math.Abs(frameSize.Height - size.Height);
                return (float)1 / (dist + 1);
            };

            VideoCapabilities bestMatchingCapabilities = null;
            float bestScore = 0;
            foreach (var c in capabilities)
            {
                float sim = similarityFunc(c.FrameSize);

                if (sim > bestScore)
                {
                    bestScore = sim;
                    bestMatchingCapabilities = c;
                }
            }

            return bestMatchingCapabilities;
        }

        /// <summary>
        /// Gets video capabilities for selected device.
        /// </summary>
        public VideoCapabilities[] VideoCapabilities 
        {
            get 
            {
                if (videoSource is VideoCaptureDevice)
                {
                    return ((VideoCaptureDevice)videoSource).VideoCapabilities;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Flips horizontal.
        /// </summary>
        public override bool FlipHorizontal //HOT TO DO IT ? (without making Image<,> methods)*/
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Flips vertical.
        /// </summary>
        public override bool FlipVertical //HOT TO DO IT ? (without making Image<,> methods)*/
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
 
        void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            base.OnVideoFrame(eventArgs.Frame);
        }
    }
}
