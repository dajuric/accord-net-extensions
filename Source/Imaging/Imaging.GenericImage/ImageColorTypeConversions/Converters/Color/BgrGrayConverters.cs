using System.Runtime.CompilerServices;

namespace Accord.Extensions.Imaging.Converters
{
    internal static class BgrGrayConverters 
    {
        #region Bgr8 -> Gray<byte>

        /// <summary>
        /// see: http://www.songho.ca/dsp/luminance/luminance.html
        /// </summary>
        public unsafe static void ConvertBgrToGray_Byte(IImage src, IImage dest)
        {
            Bgr8* srcPtr = (Bgr8*)src.ImageData;
            byte* dstPtr = (byte*)dest.ImageData;

            int width = src.Width;
            int height = src.Height;

            int srcShift = src.Stride - width * sizeof(Bgr8); //DO NOT divide with sizeof(Bgr8) as reminder may not be 0!!!
            int dstShift = dest.Stride - width * sizeof(byte);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Bgr8.ConvertBgrToGray(srcPtr, dstPtr);

                    srcPtr++;
                    dstPtr++;
                }

                srcPtr = (Bgr8*)((byte*)srcPtr + srcShift);
                dstPtr = (byte*)((byte*)dstPtr + dstShift);
            }
        }

        #endregion

        #region Gray -> Bgr (Generic)

        public unsafe static void ConvertGrayToBgr(IImage src, IImage dest)
        {
            IImage[] channels = new IImage[] { src, src, src };
            ChannelMerger.MergeChannels(channels, dest);
        }

        #endregion

    }
}
