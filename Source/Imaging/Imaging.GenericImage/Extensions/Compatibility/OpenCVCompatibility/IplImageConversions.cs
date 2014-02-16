using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Imaging
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct IplImage
    {
        public const uint IPL_DEPTH_SIGN = 0x80000000;
        
        public enum IplChannelDepth : int
        {
            /// <summary>
            /// Unsigned 8-bit integer
            /// </summary>
           IPL_DEPTH_8U = 8,
            /// <summary>
            /// Signed 8-bit integer
            /// </summary>
           IPL_DEPTH_8S = -2147483640,
            /// <summary>
            /// Unsigned 16-bit integer
            /// </summary>
           IPL_DEPTH_16U = 16,
            /// <summary>
            /// Signed 16-bit integer
            /// </summary>
           IPL_DEPTH_16S = -2147483632,
            /// <summary>
            /// Signed 32-bit integer.
            /// </summary>
           IPL_DEPTH_32S = -2147483616,
            /// <summary>
            /// Single-precision floating point
            /// </summary>
           IPL_DEPTH_32F = 32,
            /// <summary>
            /// Double-precision floating point
            /// </summary>
           IPL_DEPTH_64F = 64
        }

        public enum ChannelDataOrder : int
        {
            /// <summary>
            /// Interleaved color channels.
            /// </summary>
            INTERLEAVED = 0, 
            /// <summary>
            /// Separate color channels. (CreateImage only creates images with interleaved channels.)
            /// </summary>
            SEPARATE = 1
        }

        public enum DataOrigin : int
        {
            TopLeft = 0,
            BottomLeft = 1
        }

        /// <summary>
        /// Size of the structure
        /// </summary>
        public int StructureSize;
        /// <summary>
        /// Version, always equal 0
        /// </summary>
        public int ID;
        /// <summary>
        /// Number of channels. Most OpenCV functions support 1-4 channels.
        /// </summary>
        public int NumberOfChannels;
        /// <summary>
        /// Ignored by OpenCV (set to zero).
        /// </summary>
        public int AlphaChannel;
        /// <summary>
        /// Channel depth in bits + the optional sign bit
        /// </summary>
        public IplChannelDepth ChannelDepth;
        /// <summary>
        /// Ignored by OpenCV. The OpenCV function CvtColor requires the source and destination color spaces as parameters.
        /// </summary>
        public fixed byte ColorModel[4];
        /// <summary>
        /// Ignored by OpenCV
        /// </summary>
        public fixed byte ChannelSeq[4];
        /// <summary>
        /// Interleaved or separate (not used) channel order.
        /// </summary>
        public ChannelDataOrder DataOrder;
        /// <summary>
        /// Data origin: top-left or bottom-right (not used).
        /// </summary>
        public DataOrigin Origin;
        /// <summary>
        /// Alignment of image rows (4 or 8). (Ignored by OpenCV)
        /// </summary>
        public int Align;
        /// <summary>
        /// Image width
        /// </summary>
        public int Width;
        /// <summary>
        /// Image height
        /// </summary>
        public int Height;
        /// <summary>
        /// Region Of Interest (ROI). If not NULL, only this image region will be processed.
        /// It is always null if generic image is represented as OpenCV image.
        /// </summary>
        public void* ROI;
        /// <summary>
        /// Must be NULL in OpenCV.
        /// </summary>
        public void* MaskROI;
        /// <summary>
        /// Must be NULL in OpenCV.
        /// </summary>
        public void* ImageId;
        /// <summary>
        /// Must be NULL in OpenCV.
        /// </summary>
        public void* TileInfo;
        /// <summary>
        /// Image data size in bytes.
        /// </summary>
        public int ImageSize;
        /// <summary>
        /// A pointer to the aligned image data.
        /// </summary>
        public byte* ImageData;
        /// <summary>
        /// The size of an aligned image row, in bytes.
        /// </summary>
        public int WidthStep;
        /// <summary>
        /// Border completion mode, ignored by OpenCV.
        /// </summary>
        public fixed int BorderMode[4];
        /// <summary>
        /// Border completion mode, ignored by OpenCV.
        /// </summary>
        public fixed int BorderConst[4];
        /// <summary>
        /// A pointer to the origin of the image data (not necessarily aligned). This is used for image deallocation.
        /// During casting between generic image to OpenCV image it is set to null.
        /// </summary>
        public byte* ImageDataOrigin;

        internal IplImage(IImage image, Func<Type, IplChannelDepth> translationFunc)
        {
            /************************ default values initialization *********************************/
            this.StructureSize = sizeof(IplImage);
            this.ID = 0;
            this.Align = 4;
            this.AlphaChannel = 0;
            this.DataOrder = ChannelDataOrder.INTERLEAVED;
            this.Origin = DataOrigin.TopLeft;
            this.ROI = null;
            this.MaskROI = null;
            this.ImageId = null;
            this.TileInfo = null;
            this.ImageDataOrigin = null;
            /************************ default values initialization *********************************/

            var colorInfo = image.ColorInfo;

            this.Align = (image.Stride % 8 == 0) ? 8 :
                         (image.Stride % 4 == 0) ? 4 : 0; //TODO: check does OpenCV supports non-aligned images

            this.ChannelDepth = translationFunc(colorInfo.ChannelType);
            this.NumberOfChannels = colorInfo.NumberOfChannels;

            this.Width = image.Width;
            this.Height = image.Height;
            this.WidthStep = image.Stride;
            this.ImageSize = colorInfo.Size * image.Stride * image.Height;

            this.ImageData = (byte*)image.ImageData;
           
        }

        public static IplImage FromPointer(IntPtr pointerToStructure)
        {
            return (IplImage)Marshal.PtrToStructure(pointerToStructure, typeof(IplImage));
        }
    }

    public static class ImageOpenCVImageConversions
    {
        static Map<Type, IplImage.IplChannelDepth> depthAssociations;
        static Map<Type, int> colorAssociations;

        static ImageOpenCVImageConversions()
        {
            depthAssociations = new Map<Type, IplImage.IplChannelDepth>();
 
            depthAssociations.Add(typeof(sbyte),   IplImage.IplChannelDepth.IPL_DEPTH_8S);
            depthAssociations.Add(typeof(byte),    IplImage.IplChannelDepth.IPL_DEPTH_8U);

            depthAssociations.Add(typeof(short),   IplImage.IplChannelDepth.IPL_DEPTH_16S);
            depthAssociations.Add(typeof(ushort),  IplImage.IplChannelDepth.IPL_DEPTH_16U);

            depthAssociations.Add(typeof(int),     IplImage.IplChannelDepth.IPL_DEPTH_32S);

            depthAssociations.Add(typeof(float),   IplImage.IplChannelDepth.IPL_DEPTH_32F);
            depthAssociations.Add(typeof(double),  IplImage.IplChannelDepth.IPL_DEPTH_64F);

            /****************************************************************************************/
            colorAssociations = new Map<Type, int>();

            colorAssociations.Add(typeof(Gray), 1);
            colorAssociations.Add(typeof(Color2), 2);
            colorAssociations.Add(typeof(Color3), 3);
            colorAssociations.Add(typeof(Color4), 4);
        }

        /// <summary>
        /// Casts an image top OpeCV image. No data copy is involved.
        /// </summary>
        /// <param name="image">Generic image.</param>
        /// <returns>OpeCV image format.</returns>
        public static IplImage AsOpenCvImage(this IImage image)
        {
            return new IplImage(image, (channelType) => 
            {
                /*if (image.IsOpenCVCompatibile() == false)
                    throw new Exception("The image stride must be compatibile to OpenCV image stride!");*/

                IplImage.IplChannelDepth value;
                bool exist = depthAssociations.Forward.TryGetValue(channelType, out value);

                if (!exist)
                    throw new Exception("The image can not be casted to IplImage because the image depth type is not supported by OpenCV!");
         
                return value;
            });
        }


        public static unsafe IImage AsImage(this IplImage iplImage)
        {
            var depthType = depthAssociations.Reverse[iplImage.ChannelDepth];
            var colorType = colorAssociations.Reverse[iplImage.NumberOfChannels];

            var colorInfo = ColorInfo.GetInfo(colorType, depthType);
            return GenericImageBase.Create(colorInfo, (IntPtr)iplImage.ImageData, iplImage.Width, iplImage.Height, iplImage.WidthStep, null);
        }      
    }
}
