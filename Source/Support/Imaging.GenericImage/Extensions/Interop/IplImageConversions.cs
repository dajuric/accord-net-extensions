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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Represents the OpenCV's IplImage structure which enables OpenCV / EmguCV interoperability.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct IplImage
    {
        /// <summary>
        /// Constant for signed data types.
        /// </summary>
        public const uint IPL_DEPTH_SIGN = 0x80000000;

        /// <summary>
        /// Depth constants for IplImage channels.
        /// </summary>
        public enum IplChannelDepth : uint
        {
            /// <summary>
            /// Unsigned 1bit
            /// </summary>
            IPL_DEPTH_1U = 1,
            /// <summary>
            /// Unsigned 8-bit integer
            /// </summary>
            IPL_DEPTH_8U = 8,
            /// <summary>
            /// Signed 8-bit integer
            /// </summary>
            IPL_DEPTH_8S = (IPL_DEPTH_SIGN | 8),
            /// <summary>
            /// Unsigned 16-bit integer
            /// </summary>
            IPL_DEPTH_16U = 16,
            /// <summary>
            /// Signed 16-bit integer
            /// </summary>
            IPL_DEPTH_16S = (IPL_DEPTH_SIGN | 16),
            /// <summary>
            /// Signed 32-bit integer.
            /// </summary>
            IPL_DEPTH_32S = (IPL_DEPTH_SIGN | 32),
            /// <summary>
            /// Single-precision floating point
            /// </summary>
            IPL_DEPTH_32F = 32,
            /// <summary>
            /// Double-precision floating point
            /// </summary>
            IPL_DEPTH_64F = 64
        }

        /// <summary>
        /// IplImage channel data order.
        /// </summary>
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

        /// <summary>
        /// IplImage data origin.
        /// </summary>
        public enum DataOrigin : int
        {
            /// <summary>
            /// Data origin is located in the top-left corner.
            /// </summary>
            TopLeft = 0,
            /// <summary>
            /// Data origin is located in the bottom-left corner (Windows bitmap).
            /// </summary>
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
        public IntPtr ROI;
        /// <summary>
        /// Must be NULL in OpenCV.
        /// </summary>
        public IntPtr MaskROI;
        /// <summary>
        /// Must be NULL in OpenCV.
        /// </summary>
        public IntPtr ImageId;
        /// <summary>
        /// Must be NULL in OpenCV.
        /// </summary>
        public IntPtr TileInfo;
        /// <summary>
        /// Image data size in bytes.
        /// </summary>
        public int ImageSize;
        /// <summary>
        /// A pointer to the aligned image data.
        /// </summary>
        public IntPtr ImageData;
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
        public IntPtr ImageDataOrigin;

        internal IplImage(IImage image, Func<Type, IplChannelDepth> translationFunc)
        {
            /************************ default values initialization *********************************/
            this.StructureSize = sizeof(IplImage);
            this.ID = 0;
            this.Align = 4;
            this.AlphaChannel = 0;
            this.DataOrder = ChannelDataOrder.INTERLEAVED;
            this.Origin = DataOrigin.TopLeft;
            this.ROI = IntPtr.Zero;
            this.MaskROI = IntPtr.Zero;
            this.ImageId = IntPtr.Zero;
            this.TileInfo = IntPtr.Zero;
            this.ImageDataOrigin = IntPtr.Zero;
            /************************ default values initialization *********************************/

            var colorInfo = image.ColorInfo;

            this.Align = (image.Stride % 8 == 0) ? 8 :
                         (image.Stride % 4 == 0) ? 4 : 0; //TODO: check does OpenCV supports non-aligned images

            this.ChannelDepth = translationFunc(colorInfo.ColorType);
            this.NumberOfChannels = colorInfo.ChannelCount;

            this.Width = image.Width;
            this.Height = image.Height;
            this.WidthStep = image.Stride;
            this.ImageSize = colorInfo.Size * image.Stride * image.Height;

            this.ImageData = image.ImageData;

        }

        /// <summary>
        /// Converts a pointer to an IplImage structure.
        /// </summary>
        /// <param name="pointerToStructure"></param>
        /// <returns></returns>
        public static IplImage FromPointer(IntPtr pointerToStructure)
        {
            if (pointerToStructure.Equals(IntPtr.Zero))
                return default(IplImage);

            return (IplImage)Marshal.PtrToStructure(pointerToStructure, typeof(IplImage));
        }
    }

    /// <summary>
    /// Contains methods for casting an generic image to an IplImage.
    /// </summary>
    public static class ImageOpenCVImageConversions
    {
        class IplColorInfo: IEquatable<IplColorInfo>
        {
            public IplColorInfo(int channelCount, IplImage.IplChannelDepth channelDepth)
            {
                this.ChannelCount = channelCount;
                this.ChannelDepth = channelDepth;
            }

            public int ChannelCount { get; private set; }
            public IplImage.IplChannelDepth ChannelDepth { get; private set; }

            public bool Equals(IplColorInfo other)
            {
                if (other.ChannelCount == this.ChannelCount &&
                    other.ChannelDepth == this.ChannelDepth)
                    return true;

                return false;
            }

            public override int GetHashCode()
            {
                return this.ChannelCount ^ (int)this.ChannelDepth;
            }
        }

        class GenericImageConstructor
        {
            public static GenericImageConstructor Create<TColor>()
                where TColor: struct, IColor
            {
                GenericImageConstructor ctor = new GenericImageConstructor();
                ctor.ColorType = typeof(TColor);

                ctor.ToImage = (iplImage, destructor) =>
                {
                    if (destructor != null)
                        return new Image<TColor>(iplImage.ImageData, iplImage.Width, iplImage.Height, iplImage.WidthStep, iplImage, destructor);
                    else
                        return new Image<TColor>(iplImage.ImageData, iplImage.Width, iplImage.Height, iplImage.WidthStep, iplImage);
                };

                return ctor;
            }

            public Type ColorType { get; private set; }

            public Func<IplImage, Action<object>, IImage> ToImage { get; private set; }

            private GenericImageConstructor()
            { }
        }

        static Dictionary<Type, IplColorInfo> colorAssociations;
        static Dictionary<IplColorInfo, GenericImageConstructor> colorConstructorPairs;

        static ImageOpenCVImageConversions()
        {
            colorAssociations = new Dictionary<Type, IplColorInfo>();

            //Gray
            colorAssociations.Add(typeof(Gray<sbyte>),  new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_8S));
            colorAssociations.Add(typeof(Gray<byte>),   new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_8U));
            colorAssociations.Add(typeof(Gray<short>),  new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_16S));
            colorAssociations.Add(typeof(Gray<ushort>), new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_16U));
            colorAssociations.Add(typeof(Gray<int>),    new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_32S));
            colorAssociations.Add(typeof(Gray<float>),  new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_32F));
            colorAssociations.Add(typeof(Gray<double>), new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_64F));

            //Color2 - TODO ??

            //Bgr
            colorAssociations.Add(typeof(Bgr<sbyte>), new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_8S));
            colorAssociations.Add(typeof(Bgr<byte>), new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_8U));
            colorAssociations.Add(typeof(Bgr<short>), new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_16S));
            colorAssociations.Add(typeof(Bgr<ushort>), new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_16U));
            colorAssociations.Add(typeof(Bgr<int>), new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_32S));
            colorAssociations.Add(typeof(Bgr<float>), new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_32F));
            colorAssociations.Add(typeof(Bgr<double>), new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_64F));

            //Bgra
            colorAssociations.Add(typeof(Bgra<sbyte>), new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_8S));
            colorAssociations.Add(typeof(Bgra<byte>), new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_8U));
            colorAssociations.Add(typeof(Bgra<short>), new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_16S));
            colorAssociations.Add(typeof(Bgra<ushort>), new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_16U));
            colorAssociations.Add(typeof(Bgra<int>), new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_32S));
            colorAssociations.Add(typeof(Bgra<float>), new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_32F));
            colorAssociations.Add(typeof(Bgra<double>), new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_64F));
            /****************************************************************************************/


            colorConstructorPairs = new Dictionary<IplColorInfo, GenericImageConstructor>();

            //Gray
            colorConstructorPairs.Add(new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_8S), GenericImageConstructor.Create<Gray<sbyte>>());
            colorConstructorPairs.Add(new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_8U), GenericImageConstructor.Create<Gray<byte>>());
            colorConstructorPairs.Add(new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_16S), GenericImageConstructor.Create<Gray<short>>());
            colorConstructorPairs.Add(new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_16U), GenericImageConstructor.Create<Gray<ushort>>());
            colorConstructorPairs.Add(new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_32S), GenericImageConstructor.Create<Gray<int>>());
            colorConstructorPairs.Add(new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_32F), GenericImageConstructor.Create<Gray<float>>());
            colorConstructorPairs.Add(new IplColorInfo(1, IplImage.IplChannelDepth.IPL_DEPTH_64F), GenericImageConstructor.Create<Gray<double>>());

            //Color2 - TODO

            //Bgr
            colorConstructorPairs.Add(new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_8S), GenericImageConstructor.Create<Bgr<sbyte>>());
            colorConstructorPairs.Add(new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_8U), GenericImageConstructor.Create<Bgr<byte>>());
            colorConstructorPairs.Add(new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_16S), GenericImageConstructor.Create<Bgr<short>>());
            colorConstructorPairs.Add(new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_16U), GenericImageConstructor.Create<Bgr<ushort>>());
            colorConstructorPairs.Add(new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_32S), GenericImageConstructor.Create<Bgr<int>>());
            colorConstructorPairs.Add(new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_32F), GenericImageConstructor.Create<Bgr<float>>());
            colorConstructorPairs.Add(new IplColorInfo(3, IplImage.IplChannelDepth.IPL_DEPTH_64F), GenericImageConstructor.Create<Bgr<double>>());

            //Bgra
            colorConstructorPairs.Add(new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_8S), GenericImageConstructor.Create<Bgra<sbyte>>());
            colorConstructorPairs.Add(new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_8U), GenericImageConstructor.Create<Bgra<byte>>());
            colorConstructorPairs.Add(new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_16S), GenericImageConstructor.Create<Bgra<short>>());
            colorConstructorPairs.Add(new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_16U), GenericImageConstructor.Create<Bgra<ushort>>());
            colorConstructorPairs.Add(new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_32S), GenericImageConstructor.Create<Bgra<int>>());
            colorConstructorPairs.Add(new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_32F), GenericImageConstructor.Create<Bgra<float>>());
            colorConstructorPairs.Add(new IplColorInfo(4, IplImage.IplChannelDepth.IPL_DEPTH_64F), GenericImageConstructor.Create<Bgra<double>>());
        }

        /// <summary>
        /// Casts an image to OpeCV image (IplImage). No data copy is involved.
        /// </summary>
        /// <param name="image">Generic image.</param>
        /// <returns>OpeCV image format.</returns>
        public static IplImage AsOpenCvImage(this IImage image)
        {
            return new IplImage(image, (channelType) =>
            {
                /*if (image.IsOpenCVCompatibile() == false)
                    throw new Exception("The image stride must be compatible to OpenCV image stride!");*/

                IplColorInfo iplColorInfo;
                bool exist = colorAssociations.TryGetValue(channelType, out iplColorInfo);

                if (!exist)
                    throw new Exception("The image can not be casted to IplImage because the image depth type is not supported by OpenCV!");

                return iplColorInfo.ChannelDepth;
            });
        }

        /// <summary>
        /// Casts iplImage in generic image representation.
        /// </summary>
        /// <param name="iplImage">IplImage structure.</param>
        /// <param name="destructor">Destructor which is called when created generic image is disposed.</param>
        /// <returns>Image.</returns>
        public static unsafe IImage AsImage(this IplImage iplImage, Action<object> destructor = null)
        {
            if (iplImage.Equals(default(IplImage)))
                return null;

            var imgCtorInfo = colorConstructorPairs[new IplColorInfo(iplImage.NumberOfChannels, iplImage.ChannelDepth)];
            return imgCtorInfo.ToImage(iplImage, destructor);    
        }
    }
}
