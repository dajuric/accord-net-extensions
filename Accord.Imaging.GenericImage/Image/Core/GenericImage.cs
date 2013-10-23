using Accord.Core;
using Accord.Imaging.Converters;
using Accord.Imaging.Helper;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Accord.Imaging
{
    /// <summary>
    /// Generic image.
    /// </summary>
    /// <typeparam name="TColor">Color from type <see cref="IColor"/>.</typeparam>
    /// <typeparam name="TDepth">Primitive type.</typeparam>
    public partial class Image<TColor, TDepth> : GenericImageBase
                                where TColor: IColor
                                where TDepth: struct
    {
        #region Constructor methods

        /// <summary>
        /// Initializes (registers) color converters.
        /// </summary>
        static Image()
        {
            ColorDepthConverterFactory.Initialize();
        }

        /// <summary>
        /// Do not remove! Needed for dynamic image creation via cached expressions.
        /// </summary>
        private Image()
        {
            this.ColorInfo = ColorInfo.GetInfo<TColor, TDepth>(); //an early init is needed during deserialization
        }

        /// <summary>
        /// Construct an image from channels.
        /// </summary>
        /// <param name="channels">Channels. The number of channels must be the same as number of channels specified by this color type.</param>
        public Image(Image<Gray, TDepth>[] channels)
            :this()
        {
            if (ColorInfo.NumberOfChannels != channels.Length)
                throw new Exception("Number of channels must be the same as number of channels specified by this color type!");

            int width = channels[0].Width;
            int height = channels[0].Height;

            GenericImageBase.Initialize(this, width, height);
            ChannelMerger.MergeChannels<TColor, TDepth>(channels, this);
        }

        /// <summary>
        /// Constructs an image. (allocation)
        /// </summary>
        /// <param name="size">Image size.</param>
        public Image(Size size)
            :this()
        {
            GenericImageBase.Initialize(this, size.Width, size.Height);
        }

        /// <summary>
        /// Constructs an image. (allocation)
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public Image(int width, int height)
            :this()
        {
            GenericImageBase.Initialize(this, width, height);
        }

        /// <summary>
        /// Constructs an image and sets pixel value. (allocation)
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="value">User selected image color.</param>
        public Image(int width, int height, TColor value)
            : this()
        {
            GenericImageBase.Initialize(this, width, height);
            ValueSetter.SetValue(this, value);
        }

        /// <summary>
        /// Constructs an image from unmanaged data. Data is shared.
        /// </summary>
        /// <param name="imageData">Pointer to unmanaged data.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="stride">Image stride.</param>
        /// <param name="parentReference">To prevent object from deallocating use this parameter.</param>
        /// <param name="parentHandle">If using pinned object use GCHandle to release an allocated handle.</param>
        public Image(IntPtr imageData, int width, int height, int stride, object parentReference, GCHandle parentHandle = default(GCHandle))
            : this()
        {
            GenericImageBase.Initialize(this, imageData, width, height, stride, parentReference, parentHandle);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets image channel.
        /// Channel size must be the same as image size.
        /// </summary>
        /// <param name="channelIdx">Index of an channel to get or replace.</param>
        /// <returns>Image channel.</returns>
        public Image<Gray, TDepth> this[int channelIdx]
        {
            get { return ((IImage)this)[channelIdx] as Image<Gray, TDepth>; }
            set { ((IImage)this)[channelIdx] = value; }
        }

        /// <summary>
        /// Gets or sets image color at an location.
        /// If you need fast access to an image pixel use unmanaged approach.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <returns>Color</returns>
        public TColor this[int row, int col]
        {
            get
            {
                IntPtr data = GetData(row, col);
                return HelperMethods.PointerToColor<TColor, TDepth>(data);
            }
            set 
            {
                IntPtr data = GetData(row, col);
                HelperMethods.ColorToPointer<TColor, TDepth>(value, data);
            }
        }

        #endregion

        #region Basic helper methods

        /// <summary>
        /// Gets sub-image from specified area. Data is shared.
        /// </summary>
        /// <param name="rect">Area of an image for sub-image creation.</param>
        /// <returns>Sub-image.</returns>
        public Image<TColor, TDepth> GetSubRect(Rectangle rect)
        {
            return ((IImage)this).GetSubRect(rect) as Image<TColor, TDepth>;
        }

        /// <summary>
        /// Clones an image (data is copied).
        /// </summary>
        /// <returns>New cloned image.</returns>
        public Image<TColor, TDepth> Clone()
        {
            return ((IImage)this).Clone() as Image<TColor, TDepth>;
        }

        /// <summary>
        /// Copies all image information except image data.
        /// Image data is blank-field.
        /// </summary>
        /// <returns>New cloned image with blank data.</returns>
        public Image<TColor, TDepth> CopyBlank()
        {
            return ((IImage)this).CopyBlank() as Image<TColor, TDepth>;
        }

        #endregion
    }

}
