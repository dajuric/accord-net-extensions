using Accord.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
    public unsafe partial class DenseHistogram
    {
        delegate void BackpropagateFunc(DenseHistogram hist, IImage[] channels, IImage projImg);

        static Dictionary<Type, BackpropagateFunc> backpropagateFuncs;

        private static void initializeBackpropagate()
        {
            backpropagateFuncs = new Dictionary<Type, BackpropagateFunc>();
            backpropagateFuncs.Add(typeof(byte), backProjectByte);
        }

        /// <summary>
        /// Back-projects (creates probability map) from histogram values.
        /// </summary>
        /// <param name="srcs">Image channels.</param>
        /// <returns>Back-projection image (probability image) </returns>
        public Image<Gray, byte> BackProject(Image<Gray, byte>[] srcs)
        {
            return BackProject<byte>(srcs);
        }

        private Image<Gray, TDepth> BackProject<TDepth>(Image<Gray, TDepth>[] srcs) 
            where TDepth : struct
        {
            var destColor = ColorInfo.GetInfo(typeof(Gray), typeof(TDepth));

            BackpropagateFunc backpropagateFunc = null;
            if (backpropagateFuncs.TryGetValue(destColor.ChannelType, out backpropagateFunc) == false)
                throw new Exception(string.Format("Back-propagate function does not support an image of type {0}", destColor.ChannelType));

            var imgSize = srcs[0].Size;

            var proc = new ParallelProcessor<IImage[], IImage>(imgSize, 
                                                            ()=> //executed once
                                                            {
                                                                return GenericImageBase.Create(destColor, imgSize.Width, imgSize.Height);
                                                            }, 

                                                            (IImage[] srcImgs, IImage destImg, Rectangle area) => //executed for each thread
                                                            {
                                                                var channelPatches = new IImage[srcImgs.Length];
                                                                for (int i = 0; i < channelPatches.Length; i++)
			                                                    {
                                                                    channelPatches[i] = srcImgs[i].GetSubRect(area);
			                                                    }

                                                                var projPatch = destImg.GetSubRect(area);

                                                                backpropagateFunc(this, channelPatches, projPatch);
                                                            }
                                                            /*,new ParallelOptions { ForceSequential = true}*/);

            return proc.Process(srcs) as Image<Gray, TDepth>;
        }

        #region Backproject Byte

        private static void backProjectByte(DenseHistogram hist, IImage[] channels, IImage projImg)
        {
            /******************************* prepare data *****************************/
            float* bins = (float*)hist.HistogramData;
            float[][] valueToIndexMultipliers = hist.ValueToIndexMultipliers;
            int[] strides = hist.Strides;

            int width = channels[0].Width;
            int height = channels[0].Height;
            int channelStride = channels[0].Stride;
            int numDimensions = channels.Length;

            byte*[] channelPtrs = new byte*[channels.Length];
            for (int i = 0; i < channelPtrs.Length; i++)
            {
                channelPtrs[i] = (byte*)channels[i].ImageData;
            }

            byte* projImgPtr = (byte*)projImg.ImageData;
            int projImgStride = projImg.Stride;
            /******************************* prepare data *****************************/

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    float* binsTemp = bins;
                    for (int d = 0; d < numDimensions; d++)
                    {
                        var idxDecimal = channelPtrs[d][col] * valueToIndexMultipliers[d][0] + valueToIndexMultipliers[d][1];
                        int idx = (int)(idxDecimal);

                        binsTemp += idx * strides[d];
                    }

                    projImgPtr[col] = (byte)binsTemp[0];
                }

                for (int d = 0; d < numDimensions; d++)
                    channelPtrs[d] += channelStride;

                projImgPtr += projImgStride;
            }
        }

        #endregion

    }
}
