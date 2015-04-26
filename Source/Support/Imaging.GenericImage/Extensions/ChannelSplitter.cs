using System.Linq;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Provides channel splitting extensions.
    /// </summary>
    public static class ChannelSplitter
    {
        /// <summary>
        /// Extracts the specified image channels.
        /// </summary>
        /// <typeparam name="TSrcColor">Source color type.</typeparam>
        /// <typeparam name="TDepth">Channel depth type.</typeparam>
        /// <param name="image">Image.</param>
        /// <param name="channelIndices">Channel indicies to extract. If null, all channels are extracted.</param>
        /// <returns>Channel collection.</returns>
        public static unsafe Gray<TDepth>[][,] SplitChannels<TSrcColor, TDepth>(this TSrcColor[,] image, params int[] channelIndices)
            where TSrcColor : struct, IColor<TDepth>
            where TDepth : struct
        {
            Rectangle area = new Rectangle(0, 0, image.Width(), image.Height());
            return image.SplitChannels<TSrcColor, TDepth>(area, channelIndices);
        }

        /// <summary>
        /// Extracts the specified image channels.
        /// </summary>
        /// <typeparam name="TSrcColor">Source color type.</typeparam>
        /// <typeparam name="TDepth">Channel depth type.</typeparam>
        /// <param name="image">Image.</param>
        /// <param name="area">Working area.</param>
        /// <param name="channelIndices">Channel indicies to extract. If null, all channels are extracted.</param>
        /// <returns>Channel collection.</returns>
        public static unsafe Gray<TDepth>[][,] SplitChannels<TSrcColor, TDepth>(this TSrcColor[,] image, Rectangle area, params int[] channelIndices)
            where TSrcColor: struct, IColor<TDepth>
            where TDepth: struct
        {
            if (channelIndices == null || channelIndices.Length == 0)
            {
                channelIndices = Enumerable.Range(0, ColorInfo.GetInfo<TSrcColor>().ChannelCount).ToArray();
            }

            var channels = new Gray<TDepth>[channelIndices.Length][,];
            for (int i = 0; i < channelIndices.Length; i++)
			{
                channels[i] = GetChannel<TSrcColor, TDepth>(image, area, channelIndices[i]);
			}

            return channels;
        }

        /// <summary>
        /// Extracts a single image channel.
        /// </summary>
        /// <typeparam name="TSrcColor">Source color type.</typeparam>
        /// <typeparam name="TDepth">Channel depth type.</typeparam>
        /// <param name="image">Image.</param>
        /// <param name="channelIndex">Channel index.</param>
        /// <returns>Extracted channel.</returns>
        public static unsafe Gray<TDepth>[,] GetChannel<TSrcColor, TDepth>(this TSrcColor[,] image, int channelIndex)
            where TSrcColor : struct, IColor<TDepth>
            where TDepth : struct
        {
            Rectangle area = new Rectangle(0, 0, image.Width(), image.Height());
            return image.GetChannel<TSrcColor, TDepth>(channelIndex);
        }

        /// <summary>
        /// Extracts a single image channel.
        /// </summary>
        /// <typeparam name="TSrcColor">Source color type.</typeparam>
        /// <typeparam name="TDepth">Channel depth type.</typeparam>
        /// <param name="image">Image.</param>
        /// <param name="area">Working area.</param>
        /// <param name="channelIndex">Channel index.</param>
        /// <returns>Extracted channel.</returns>
        public static unsafe Gray<TDepth>[,] GetChannel<TSrcColor, TDepth>(this TSrcColor[,] image, Rectangle area, int channelIndex)
            where TSrcColor: struct, IColor<TDepth>
            where TDepth: struct
        {
            int width = area.Width;
            int height = area.Height;

            var dest = new Gray<TDepth>[area.Height, area.Width];

            using (var lockedImage = image.Lock())
            using (var dstImg = dest.Lock())
            {
                var srcImg = lockedImage.GetSubRect(area);
                int channelSize = srcImg.ColorInfo.ChannelSize;
                int colorSize = srcImg.ColorInfo.Size;

                byte* srcPtr = (byte*)srcImg.ImageData + channelIndex * srcImg.ColorInfo.ChannelSize;
                byte* dstPtr = (byte*)dstImg.ImageData;

                for (int row = 0; row < height; row++)
                {
                    byte* srcColPtr = srcPtr;
                    byte* dstColPtr = dstPtr;
                    for (int col = 0; col < width; col++)
                    {
                        /********** copy channel byte-per-byte ************/
                        for (int partIdx = 0; partIdx < channelSize; partIdx++)
                        {
                            dstColPtr[partIdx] = srcColPtr[partIdx];
                        }

                        srcColPtr += colorSize; //move to the next column
                        dstColPtr += channelSize;
                        /********** copy channel byte-per-byte ************/
                    }

                    srcPtr += srcImg.Stride;
                    dstPtr += dstImg.Stride;
                }
            }

            return dest;
        }
    }
}
