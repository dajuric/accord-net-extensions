using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public static class ImagePatchProcessor
    {
        private static void makePatches(Int32Size imgSize, int minPatchHeight, out List<Int32Rect> patches)
        {
            int patchHeight, verticalPatches;
            getPatchInfo(imgSize, out patchHeight, out verticalPatches);
            minPatchHeight = System.Math.Max(minPatchHeight, patchHeight);

            patches = new List<Int32Rect>();

            for (int y = 0; y < imgSize.Height; )
            {
                int h = System.Math.Min(patchHeight, imgSize.Height - y);

                Int32Rect patch = new Int32Rect(0, y, imgSize.Width, h);
                patches.Add(patch);

                y += h;
            }

            //ensure minPatchSize (merge last two patches if necessary)
            if (patches.Last().Height < minPatchHeight)
            {
                var penultimate = patches[patches.Count - 1 - 1];
                var last = patches[patches.Count - 1];

                var mergedPatch = new Int32Rect
                {
                    X = penultimate.X,
                    Y = penultimate.Y,
                    Width = penultimate.Width,
                    Height = penultimate.Height + last.Height
                };

                patches.RemoveRange(patches.Count - 1 - 1, 2);
                patches.Add(mergedPatch);
            }
        }

        private static void getPatchInfo(Int32Size imgSize, out int patchHeight, out int verticalPatches)
        {
            int numOfCores = System.Environment.ProcessorCount;
            int minNumOfPatches = numOfCores * 2;

            float avgNumPatchElements = (float)(imgSize.Width * imgSize.Height) / minNumOfPatches;

            //make patch look like a long stripe (it is probably more efficient to process than a square patch)
            patchHeight = (int)System.Math.Floor(avgNumPatchElements / imgSize.Width);

            //get number of patches
            verticalPatches = (int)System.Math.Ceiling((float)imgSize.Height / patchHeight);
        }

        public static unsafe Image<TColor, TDepthDst> ProcessPatch<TColor, TDepthSrc, TDepthDst>(this Image<TColor, TDepthSrc> image, Action<Int32, Int32> action)
            where TColor: IColor
            where TDepthSrc: struct
            where TDepthDst: struct
        {
            List<Int32Rect> patches;
            makePatches(image.Size, 0, out patches);

            int nChannels = image.ColorInfo.NumberOfChannels;

            var dst = new Image<TColor, TDepthDst>(image.Size);
            int dstOffset = dst.Stride - nChannels * dst.Width;

            int srcOffset = image.Stride - nChannels * image.Width;

            int width = image.Width;
            int height = image.Height;
       

            Parallel.ForEach(patches, (rect) => 
            {
                int patchHeight = rect.Height;

                byte* srcPtr = (byte*)image.GetData(rect.Y);
                byte* dstPtr = (byte*)dst.GetData(rect.Y);

                while (patchHeight-- > 0)
                {
                    int _width = width;
                    while(_width-- > 0)
                    {
                        action((Int32)(srcPtr), (Int32)(dstPtr));
                        srcPtr += nChannels;
                        dstPtr += nChannels;
                    }

                    srcPtr = (byte*)((byte*)srcPtr + srcOffset);
                    dstPtr = (byte*)((byte*)dstPtr + dstOffset);
                }

                /*for (int r = 0; r < patchHeight; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        action((Int32)(srcPtr), (Int32)(dstPtr));
                        srcPtr += nChannels;
                        dstPtr += nChannels;
                    }

                    srcPtr = (byte*)((byte*)srcPtr + srcOffset);
                    dstPtr = (byte*)((byte*)dstPtr + dstOffset);
                }*/
            });

            return dst;
        }



    }

    public unsafe struct IntPtr<T>
    {
        void* value;

        public IntPtr(void* value)
        {
            this.value = value;
        }

        public static implicit operator int(IntPtr<T> ptr)
        {
            return (int)ptr.value;
        }

        public unsafe static implicit operator void*(IntPtr<T> ptr)
        {
            return (void*)ptr.value;
        }

        public unsafe static implicit operator IntPtr<T>(void* value)
        {
            return new IntPtr<T>(value);
        }
    }
}
