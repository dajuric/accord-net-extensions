using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Serves as a base class for generic image style providing some basic functions.
    /// </summary>
    public partial class Image : IImage, IEquatable<Image>, IXmlSerializable
    {      
        PinnedArray<byte> buffer = null;

        object objectReference = null; //prevents disposing parent object if sharing data (GetSubRect(..), casting...)
        Action<object> parentDestructor = null;

        #region Constructor methods

        static Image()
        {}

        /// <summary>
        /// Image constructor used during de-serialization process.
        /// </summary>
        protected Image()
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
            Image im = (Image)ctorInvoker();

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
        /// <param name="parentDestructor">If a parent needs to be destroyed or release use this function. (e.g. unpin object - GCHandle)</param>
        /// <returns>Generic image.</returns>
        public static IImage Create(ColorInfo colorInfo, IntPtr imageData, int width, int height, int stride, object parentReference = null, Action<object> parentDestructor = null)
        {
            var ctorInvoker = HelperMethods.GetGenericImageConstructor(typeof(Image<,>), colorInfo);
            Image im = (Image)ctorInvoker();

            Initialize(im, imageData, width, height, stride, parentReference, parentDestructor);
            return im;
        }

        /// <summary>
        /// Initializes generic image (allocates data).
        /// </summary>
        /// <param name="im">Generic image.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="strideAllignment">Stride alignment. Usual practice is that every image row ends with address aligned with 4.</param>
        protected static void Initialize(Image im, int width, int height, int strideAllignment = 4)
        {
            int stride = CalculateStride(im.ColorInfo, width, strideAllignment);
            var buffer = new PinnedArray<byte>(stride * height);

            im.IsAllocated = true;
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
        protected static void Initialize(Image im, int width, int height, int stride, PinnedArray<byte> buffer)
        {
            im.IsAllocated = true;
            im.buffer = buffer;
           
            initializeProperties(im, buffer.Data, width, height, stride);
        }

        /// <summary>
        /// Initializes generic image (attaches to existing unmanaged data).
        /// </summary>
        /// <param name="im">Generic image.</param>
        /// <param name="imageData">Image data pointer.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="stride">Image stride.</param>
        /// <param name="parentReference">To prevent object from deallocating use this parameter.</param>
        /// <param name="parentDestructor">If a parent needs to be destroyed or release use this function. (e.g. unpin object - GCHandle)</param>
        protected static void Initialize(Image im, IntPtr imageData, int width, int height, int stride, object parentReference = null, Action<object> parentDestructor = null)
        {
            im.IsAllocated = false;
            im.buffer = null;

            initializeProperties(im, imageData, width, height, stride);
            im.objectReference = parentReference;
            im.parentDestructor = parentDestructor;
        }

        private static void initializeProperties(Image im, IntPtr imageData, int width, int height, int stride)
        {
            im.ImageData = imageData;
            im.Width = width;
            im.Height = height;
            im.Stride = stride;
        }

        /// <summary>
        /// Calculates image stride for the specified parameters.
        /// </summary>
        /// <param name="colorInfo">Color info.</param>
        /// <param name="width">Image width.</param>
        /// <param name="allignment">Data alignment.</param>
        /// <returns>Image stride.</returns>
        protected static int CalculateStride(ColorInfo colorInfo, int width, int allignment = 4)
        {
            int stride = width * colorInfo.Size;

            if (allignment != 0 &&
                stride % allignment != 0)
            {
                stride += (allignment - (stride % allignment));
            }

            return stride;
        }

        bool isDisposed = false;

        /// <summary>
        /// Disposes generic image. 
        /// In case if data is allocated it is released.
        /// If data is shared parent reference (if exists) and parent handle (if exist) is released.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return; //if this function is called for the first time

            if (IsAllocated) //must be disposed
            {
                buffer.Dispose();
                buffer = null;
            }
            else
            {
                if (this.parentDestructor != null)
                    this.parentDestructor(objectReference);

               this.parentDestructor = null;
               this.objectReference = null;
            }

            isDisposed = true;
        }

        /// <summary>
        /// Disposes the image.
        /// </summary>
        ~Image()
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
        /// Gets image stride - number of bytes per image row.
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
        /// True if the data (internal buffer) is allocated, false otherwise (e.g. image cast).
        /// </summary>
        public bool IsAllocated { get; private set; }

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
            if (col < 0 || col >= this.Width ||
                row < 0 || row >= this.Height)
                throw new ArgumentOutOfRangeException();

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
            if (row < 0 || row >= this.Height) 
                throw new ArgumentOutOfRangeException();

            return this.ImageData + row * this.Stride;
        }

        /// <summary>
        /// Gets sub-image from specified area. Data is shared.
        /// </summary>
        /// <param name="rect">Area of an image for sub-image creation.</param>
        /// <returns>Sub-image.</returns>
        IImage IImage.GetSubRect(Rectangle rect)
        {
            if (rect.Right > this.Width || rect.Bottom > this.Height) //Location will be verified through GetData(...) function
                throw new ArgumentOutOfRangeException();

            object objRef = this.objectReference ?? this; //always show at the root

            IntPtr data = GetData(rect.Y, rect.X);
            return Image.Create(this.ColorInfo, data, rect.Width, rect.Height, this.Stride, objRef);
        }

        /// <summary>
        /// Clones an image (data is copied).
        /// </summary>
        unsafe IImage IImage.Clone()
        {
            IImage dest = Image.Create(this.ColorInfo, this.Width, this.Height);

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
            return Image.Create(this.ColorInfo, this.Width, this.Height);
        }

        #endregion

        /// <summary>
        /// Compares this image to another image. Only pointer location and image size are compared.
        /// There is no data compassion.
        /// </summary>
        /// <param name="other">Other image.</param>
        /// <returns>Whether two images are equal or not.</returns>
        public bool Equals(Image other)
        {
            if (other != null &&
                this.ImageData == other.ImageData &&
                this.Size == other.Size)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compares this image to another object. Internally the function overload is called.
        /// </summary>
        /// <param name="obj">Other.</param>
        /// <returns>Is the image equal to an object or not.</returns>
        public override bool Equals(object obj)
        { 
            return this.Equals(obj as Image);
        }

        /// <summary>
        /// Image's hash code. Pointer address is used as hash code.
        /// </summary>
        /// <returns>Image's hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (int)this.ImageData.ToInt64(); //support for 64-bit architecture
            }
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

            using (PinnedArray<byte> buff = ((this as IImage).Clone() as Image).buffer) //if an user selected GetSubRect(...) 
            {
                writer.WriteStartElement("Bytes");
                writer.WriteBase64(buff.Array, 0, buff.Array.Length);
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
