using AForge;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Accord.Imaging
{
    /// <summary>
    /// Image histogram.
    /// </summary>
    public unsafe partial class DenseHistogram: IDisposable
    {
        int[] binSizes;
        IntRange[] ranges;

        float[][] valueToIndexMultipliers = null;
        int[] strides = null;

        float[] histogram = null;
        GCHandle histogramHandle;
        float* histPtr = null;

        static DenseHistogram()
        {
            initializeCaclculateHist();
            initializeBackpropagate();
        }

        private DenseHistogram()
        { }

        /// <summary>
        /// Constructs histogram.
        /// </summary>
        /// <param name="binSize">Bin size.</param>
        /// <param name="range">Image values range.</param>
        public DenseHistogram(int binSize, IntRange range)
            : this(new int[] { binSize }, new IntRange[] { range })
        { }

        /// <summary>
        /// Constructs histogram.
        /// </summary>
        /// <param name="binSizes">Bin sizes.</param>
        /// <param name="ranges">Image values ranges.</param>
        public DenseHistogram(int[] binSizes, IntRange[] ranges)
        {
            initalize(this, binSizes, ranges);      
        }

        private static void initalize(DenseHistogram hist, int[] binSizes, IntRange[] ranges)
        {
            hist.binSizes = binSizes;
            hist.ranges = ranges;

            var valueToIndexMultipliers = new float[binSizes.Length][];

            /***********************************************************************************/
            //                                          binSizes[i]
            //idx = (val - range[i].Min) * -----------------------------------
            //                              range[i].Max - range[i].Min + 1
            for (int bin = 0; bin < binSizes.Length; bin++)
            {
                valueToIndexMultipliers[bin] = new float[2];
                valueToIndexMultipliers[bin][0] = (float)binSizes[bin] / (ranges[bin].Max - ranges[bin].Min + 1);
                valueToIndexMultipliers[bin][1] = -ranges[bin].Min * valueToIndexMultipliers[bin][0];
            }
            hist.valueToIndexMultipliers = valueToIndexMultipliers;
            /***********************************************************************************/

            /***********************************************************************************/
            var strides = new int[binSizes.Length];
            strides[binSizes.Length - 1] = 1;
            for (int bin = (binSizes.Length - 1) - 1; bin >= 0; bin--)
            {
                strides[bin] = strides[bin + 1] * (binSizes[bin + 1]);
            }
            hist.strides = strides;
            /***********************************************************************************/

            hist.NumberOfElements = binSizes.Aggregate((a, b) => a * b);

            hist.histogram = new float[hist.NumberOfElements];
            hist.histogramHandle = GCHandle.Alloc(hist.histogram, GCHandleType.Pinned);
            hist.histPtr = (float*)hist.histogramHandle.AddrOfPinnedObject();
        }

        /// <summary>
        /// Gets total number of elements.
        /// </summary>
        public int NumberOfElements { get; private set; }

        /// <summary>
        /// Internal histogram structure. Use <see cref="Stride"/> and <see cref="ValueToIndexMultipliers"/> to access elements properly.
        /// </summary>
        public float[] HistogramArray { get { return this.histogram; } }


        /// <summary>
        /// Histogram data. Unmanaged representation of <see cref="HistogramArray"/>.
        /// </summary>
        internal IntPtr HistogramData { get { return (IntPtr)this.histPtr; } }

        /// <summary>
        /// Value is multiplied with those values to obtain appropriate histogram index.
        ///                                              binSizes[i]
        ///    idx = (val - range[i].Min) * -----------------------------------
        ///                                  range[i].Max - range[i].Min + 1
        /// </summary>
        internal float[][] ValueToIndexMultipliers { get { return this.valueToIndexMultipliers; } }
        /// <summary>
        /// Strides fro each dimensions. To access 3rd dimension use pinter + Strides[2].
        /// </summary>
        internal int[] Strides { get { return this.strides; } }

        /// <summary>
        /// Disposes histogram.
        /// </summary>
        public void Dispose()
        {
            if (histogramHandle != null && histogramHandle.IsAllocated)
            {
                histogramHandle.Free();
                histPtr = null;
                histogram = null;
            }
        }

        ~DenseHistogram()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the same histogram representation but without data. (data is default =  zero)
        /// </summary>
        /// <returns></returns>
        public DenseHistogram CopyBlank()
        {
            DenseHistogram hist = new DenseHistogram();
            initalize(hist, (int[])this.binSizes.Clone(), (IntRange[])this.ranges.Clone());
            return hist;
        }

        /// <summary>
        /// Creates ratio histogram [0.. <see cref="histogramsNormalizationFactor"/>].
        /// </summary>
        /// <param name="hist2">Second histogram. This histogram will be element-wise dived with it. Both histograms must be normalized to the same value!</param>
        /// <returns>Normalized ratio histogram</returns>
        public unsafe DenseHistogram CreateRatioHistogram(DenseHistogram hist2, float histogramsNormalizationFactor = 1, float hist2Gain = 1)
        { 
            //histograms must be normalized!
            DenseHistogram ratioHist = this.CopyBlank();

            float* hist1Ptr = (float*)this.HistogramData;
            float* hist2Ptr = (float*)hist2.HistogramData;
            float* ratioHistPtr = (float*)ratioHist.HistogramData;
            int numOfElems = this.NumberOfElements;

            for (int i = 0; i < numOfElems; i++)
            {
                if (hist2Ptr[i] != 0)
                    ratioHistPtr[i] = (hist1Ptr[i] / (hist2Gain * hist2Ptr[i])) * histogramsNormalizationFactor;
                else
                    //ratioHistPtr[i] = 0; //in original Accord's implementation. Why ?
                    ratioHistPtr[i] = hist1Ptr[i];

                ratioHistPtr[i] = System.Math.Min(histogramsNormalizationFactor, ratioHistPtr[i]);
            }

            return ratioHist;
        }
    }
}
