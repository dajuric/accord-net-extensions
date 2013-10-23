using Accord.Core;
using Accord.Imaging.Helper;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Accord.Imaging
{
    public partial class GenericImageBase : IImage, IEquatable<GenericImageBase>, IXmlSerializable
    {
        bool mustBeDisposed;
        PinnedArray<byte> buffer = null;

        object objectReference = null; //prevents disposing parent object if sharing data (GetSubRect(..), casting...)
        GCHandle parentHandle = default(GCHandle);

        #region Constructor methods

        static GenericImageBase()
        {}

        protected GenericImageBase()
        { }

        /// <summary>
        /// Creates an generic image.
        /// </summary>
        /// <param name="colorInfo">Color info.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <returns>New generic image (interface).</returns>
        public static IImage Create(ColorInfo colorInfo, int width, int height)
        {
            var ctorInvoker = HelperMethods.GetGenericImageConstructor(typeof(Image<,>), colorInfo);
            GenericImageBase im = (GenericImageBase)ctorInvoker();

            Initialize(im, width, height);
            return im;
        }

        /// <summary>
        /// Creates an generic image from unmanaged data. Data is shared.
        /// </summary>
        /// <param name="colorInfo">Color info.</param>
        /// <param name="imageData">Unmanaged data pointer.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="stride">Image stride.</param>
        /// <param name="parentReference">To prevent object from deallocating use this parameter.</param>
        /// <param name="parentHandle">If using pinned object use GCHandle to release an allocated handle.</param>
        /// <returns>Generic image.</returns>
        internal static IImage Create(ColorInfo colorInfo, IntPtr imageData, int width, int height, int stride, object parentReference, GCHandle parentHandle = default(GCHandle))
        {
            var ctorInvoker = HelperMethods.GetGenericImageConstructor(typeof(Image<,>), colorInfo);
            GenericImageBase im = (GenericImageBase)ctorInvoker();

            Initialize(im, imageData, width, height, stride, parentReference, parentHandle);
            return im;
        }

        /// <summary>
        /// Initializes generic image (allocates data).
        /// </summary>
        /// <param name="im">Generic image.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        protected static void Initialize(GenericImageBase im, int width, int height)
        {
            int stride = calculateStride(im.ColorInfo, width);
            var buffer = new PinnedArray<byte>(stride * height);

            im.mustBeDisposed = true;
            im.buffer = buffer;

            initializeProperties(im, buffer.Data, width, height, stride);
        }

        /// <summary>
        /// Initializes generic image (attaches to existing unmanaged data).
        /// </summary>
        /// <param name="im">Generic image.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="stride">Image stride.</param>
        /// <param name="buffer">Image buffer.</param>
        protected static void Initialize(GenericImageBase im, int width, int height, int stride, PinnedArray<byte> buffer)
        {
            im.mustBeDisposed = true;
            im.buffer = buffer;
           
            initializeProperties(im, buffer.Data, width, height, stride);
        }

        /// <summary>
        /// Initializes generic image (attaches to existing unmanaged data).
        /// </summary>
        /// <param name="im">Generic image.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="stride">Image stride.</param>
        /// <param name="parentReference">To prevent object from deallocating use this parameter.</param>
        /// <param name="parentHandle">If using pinned object use GCHandle to release an allocated handle.</param>
        protected static void Initialize(GenericImageBase im, IntPtr imageData, int width, int height, int stride, object parentReference, GCHandle parentHandle = default(GCHandle))
        {
            im.mustBeDisposed = false;
            im.buffer = null;

            initializeProperties(im, imageData, width, height, stride);

            if (parentReference != null)
            {
                im.objectReference = parentReference;
                im.parentHandle = parentHandle;
            }
        }

        private static void initializeProperties(GenericImageBase im, IntPtr imageData, int width, int height, int stride)
        {
            im.ImageData = imageData;
            im.Width = width;
            im.Height = height;
            im.Stride = stride;
        }

        protected static int calculateStride(ColorInfo colorInfo, int width, int allignment = 4)
        {
            int stride = width * colorInfo.Size;

            if (stride % allignment != 0)
                stride += (allignment - (stride % allignment));

            return stride;
        }

        /// <summary>
        /// Disposes generic image. 
        /// In case if data is allocated it is released.
        /// If data is shared parent reference (if exists) and parent handle (if exist) is released.
        /// </summary>
        public void Dispose()
        {
            if (mustBeDisposed && buffer != null) //must be disposed AND this function is called for the first time
            {
                buffer.Dispose();
                buffer = null;
            }
            else
            {
               if (this.parentHandle.IsAllocated)
                   this.parentHandle.Free();

               this.objectReference = null;
            }
        }

        ~GenericImageBase()
        {
            Dispose();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets unmanaged image data.
        /// </summary>
        public IntPtr ImageData { get; private set; }
        /// <summary>
        /// Gets image width.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Gets image height.
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Gets image stride.
        /// </summary>
        public int Stride { get; private set; }
        /// <summary>
        /// Gets image size.
        /// </summary>
        public Size Size { get { return new Size(this.Width, this.Height); } }
        /// <summary>
        /// Gets image color info.
        /// </summary>
        public ColorInfo ColorInfo { get; protected set; }

        /// <summary>
        /// Gets or sets image channel.
        /// Channel size must be the same as image size.
        /// </summary>
        /// <param name="channelIdx">Index of an channel to get or replace.</param>
        /// <returns>Image channel.</returns>
        IImage IImage.this[int channelIdx] 
        {
            get 
            {
                return ChannelSplitter.SplitChannels(this, channelIdx)[0];
            }
            set 
            {
                ChannelMerger.MergeChannels(new IImage[] { value }, this, channelIdx);
            }
        }

        #endregion

        #region Basic helper methods

        /// <summary>
        /// Gets image data at specified location.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <returns>Data pointer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr GetData(int row, int col)
        {
            return this.GetData(row) + col * this.ColorInfo.Size;
        }

        /// <summary>
        /// Gets image data at specified location.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>Data pointer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr GetData(int row)
        {
            return this.ImageData + row * this.Stride;
        }

        /// <summary>
        /// Gets sub-image from specified area. Data is shared.
        /// </summary>
        /// <param name="rect">Area of an image for sub-image creation.</param>
        /// <returns>Sub-image.</returns>
        IImage IImage.GetSubRect(Rectangle rect)
        {
            object objRef = this.objectReference ?? this; //always show at the root

            IntPtr data = GetData(rect.Y, rect.X);
            return GenericImageBase.Create(this.ColorInfo, data, rect.Width, rect.Height, this.Stride, objRef);
        }

        /// <summary>
        /// Clones an image (data is copied).
        /// </summary>
        unsafe IImage IImage.Clone()
        {
            IImage dest = GenericImageBase.Create(this.ColorInfo, this.Width, this.Height);

            HelperMethods.CopyImage(this.ImageData, dest.ImageData, this.Stride, dest.Stride, this.Width * this.ColorInfo.Size, this.Height);

            return dest;
        }


        /// <summary>
        /// Copies all image information except image data.
        /// Image data is blank-field.
        /// </summary>
        /// <returns>New cloned image with blank data.</returns>
        IImage IImage.CopyBlank()
        {
            return GenericImageBase.Create(this.ColorInfo, this.Width, this.Height);
        }

        #endregion

        /// <summary>
        /// Compares this image to another image. Only pointer location and image size are compared.
        /// There is no data compassion.
        /// </summary>
        /// <param name="other">Other image.</param>
        /// <returns>Whether two images are equal or not.</returns>
        public bool Equals(GenericImageBase other)
        {
            if (this.ImageData == other.ImageData &&
                this.Size == other.Size)
            {
                return true;
            }

            return false;
        }

        #region Serialization Members

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            int height = Int32.Parse(reader.GetAttribute("Rows"));
            int width = Int32.Parse(reader.GetAttribute("Cols"));

            reader.MoveToContent();
            reader.ReadToFollowing("Bytes");

            Initialize(this, width, height);
            reader.ReadElementContentAsBase64(this.buffer.Array, 0, this.buffer.SizeInBytes);
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        { 
            writer.WriteAttributeString("Rows", this.Height.ToString());
            writer.WriteAttributeString("Cols", this.Width.ToString());
            writer.WriteAttributeString("NumberOfChannels", this.ColorInfo.NumberOfChannels.ToString()); //not used (EmguCV compatibility)
            writer.WriteAttributeString("CompressionRatio", 0.ToString()); //not used (EmguCV compatibility)

            using (PinnedArray<byte> buff = ((this as IImage).Clone() as GenericImageBase).buffer) //if an user selected GetSubRect(...) 
            {
                writer.WriteStartElement("Bytes");
                writer.WriteBase64(buff.Array, 0, buff.Array.Length);
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
