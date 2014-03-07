using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Extensions.Imaging.Helper;
using MethodCache = Accord.Extensions.MethodCache;
using System.Drawing;
using Accord.Extensions;
using Accord.Extensions.Math.Geometry;

namespace Accord.Extensions.Imaging.Converters //TODO - non-critical: rewrite the code (can be written much better)
{
    public delegate void ConvertDataFunc(IImage srcImg, IImage destImg);
    public delegate IImage CreateImageFunc(IImage srcImg, ColorInfo destColor);

    /// <summary>
    /// Represents the main class for color and conversion.
    /// It uses search methods to find appropriate conversion paths.
    /// </summary>
    public static class ColorDepthConverter
    {
        public enum ConversionCost: int
        {
            Cast = 0,
            DataConvert = 1,
            NotPossible = Int32.MaxValue
        }

        public class ConversionData<T> : Edge<T>
        {
            private ConversionCost cost;

            public ConversionData(T source, T destination, ConvertDataFunc convertFunc, bool forceSequential = false, CreateImageFunc createFunc = null, ConversionCost cost = ConversionCost.DataConvert)
                :base(source, destination)
            {
                this.CreateFunc = createFunc;
                this.ConvertFunc = convertFunc;
                this.ForceSequential = forceSequential;
                this.cost = cost;
            }

            public static ConversionData<ColorInfo> AsReturnSource(ColorInfo source)
            {
                return new ConversionData<ColorInfo>(source, 
                                                    source, 
                                                    (srcImg, dstImg) => { }, true, 
                                                    (srcImg, destColor) => srcImg,
                                                    ConversionCost.Cast);
            }

            public static ConversionData<ColorInfo> AsCast(ColorInfo source, ColorInfo destination)
            {
                if (source.Equals(destination, ColorInfo.ComparableParts.Castable) == false)
                    throw new Exception("The specified colors are not castable!");

                return new ConversionData<ColorInfo>(source, 
                                                     destination, 
                                                     (srcImg, dstImg) => { }, 
                                                     true, 
                                                     (srcImg, destColor) => Image.Create(destColor, srcImg.ImageData, srcImg.Width, srcImg.Height, srcImg.Stride, srcImg), 
                                                     ConversionCost.Cast);
            }

            public static ConversionData<ColorInfo> AsConvertData(ColorInfo source, ColorInfo destination, ConvertDataFunc convertFunc, bool forceSequential = false)
            {
                return new ConversionData<ColorInfo>(source, 
                                                     destination, 
                                                     convertFunc, 
                                                     forceSequential, 
                                                     (srcImg, destColor) => Image.Create(destColor, srcImg.ImageData, srcImg.Width, srcImg.Height, srcImg.Stride, srcImg),
                                                     ConversionCost.DataConvert);
            }

            public CreateImageFunc CreateFunc { get; set; }
            public ConvertDataFunc ConvertFunc { get; set; }
            public bool ForceSequential { get; set; }
            public ConversionCost Cost
            {
                get 
                {
                    if (ConvertFunc == null) return ConversionCost.NotPossible;
                    return cost;
                }
                set { cost = value; }
            }

            public bool CopiesData
            {
                get { return Cost != ConversionCost.Cast; }
            }

            public static ConversionData<ColorInfo> From(Type colorType, ConversionData<Type> depthConversionData)
            {
                var srcColor = ColorInfo.GetInfo(colorType, depthConversionData.Source);
                var dstColor = ColorInfo.GetInfo(colorType, depthConversionData.Destination);

                return new ConversionData<ColorInfo>
                       (
                          srcColor, 
                          dstColor, 
                          depthConversionData.ConvertFunc,
                          depthConversionData.ForceSequential,
                          depthConversionData.CreateFunc,
                          depthConversionData.Cost
                       );
            }
        }

        static List<IColor> genericColors;
        static List<ConversionData<Type>> depthConversions;
        static Dictionary<ColorInfo, Dictionary<ColorInfo, ConversionData<ColorInfo>>> graph;

        static Dictionary<ColorInfo, Dictionary<ColorInfo, List<ConversionData<ColorInfo>>>> shorthestPaths;
        static Dictionary<ColorInfo, Dictionary<ColorInfo, double>> costMatrix;

        static ColorDepthConverter()
        {
            genericColors = new List<IColor>();
            depthConversions = new List<ConversionData<Type>>();
            graph = new Dictionary<ColorInfo, Dictionary<ColorInfo, ConversionData<ColorInfo>>>();

            ColorDepthConverters.Initialize();
            Initialize();
        }

        public static void Add(ConversionData<ColorInfo> conversionData)
        {
            graph.AddEdge(conversionData);
        }

        public static void Add(IColor genericColor)
        {
            if (ColorInfo.GetInfo(genericColor.GetType()).IsGenericColorSpace == false)
                throw new Exception("The provided color is not generic color type!");

            genericColors.Add(genericColor);
        }

        public static void Add(ConversionData<Type> depthConversionData)
        {
            depthConversions.Add(depthConversionData);
        }

        public static void Initialize()
        {
            addGenericConversions();
            addDepthConversions();
            addSelfPaths();

            shorthestPaths = graph.FindAllShorthestPaths(x => (int)x.Cost, out costMatrix);
            forbidNoSensePaths();
        }

        private static void addDepthConversions()
        {
            var colors = graph.Keys.ToArray();

            foreach (var color in colors)
            {
                foreach (var depthConversion in depthConversions)
                {
                    var colorConversion = ConversionData<ColorInfo>.From(color.ColorType, depthConversion);
                    graph.AddEdge(colorConversion);
                }
            }
        }

        private static void addGenericConversions()
        {
            var colors = graph.Keys.ToArray();

            foreach (var genericColor in genericColors)
            {
                foreach (var color in colors)
                {
                    var genericColorInfo = ColorInfo.GetInfo(genericColor.GetType(), color.ChannelType);

                    if (color.NumberOfChannels != genericColorInfo.NumberOfChannels)
                        continue;

                    Add(ConversionData<ColorInfo>.AsCast
                        (
                        source: genericColorInfo,
                        destination: color
                        ));

                    Add(ConversionData<ColorInfo>.AsCast
                       (
                       source: color,
                       destination: genericColorInfo
                       ));
                }
            }
        }

        private static void addSelfPaths()
        {
            var colors = graph.Keys.ToArray();

            foreach (var color in colors)
            {
                Add(ConversionData<ColorInfo>.AsReturnSource(color));
            }
        }

        private static void forbidNoSensePaths()
        { 
            //generic color conversions must be without data copy

            foreach (var v1 in shorthestPaths.Keys)
            {
                foreach (var v2 in shorthestPaths.Keys)
                {
                    if (!v1.IsGenericColorSpace && !v2.IsGenericColorSpace)
                        continue;

                    if (!shorthestPaths[v1][v2].CopiesData())
                        continue;

                    shorthestPaths[v1][v2] = new List<ConversionData<ColorInfo>>();
                    costMatrix[v1][v2] = Double.PositiveInfinity;
                }
            }

        }

        public static IImage Convert(this ConversionData<ColorInfo> conversionData, IImage img)
        {
            var proc = new ParallelProcessor<IImage, IImage>
                (
                  img.Size,
                  () => conversionData.CreateFunc(img, conversionData.Destination),
                  (src, dest, area) => conversionData.ConvertFunc(src.GetSubRect(area), dest.GetSubRect(area))
#if DEBUG
                  ,new ParallelOptions { ForceSequential = true }
#else
                  ,new ParallelOptions { ForceSequential = conversionData.ForceSequential }
#endif
                );

            return proc.Process(img);
        }

        /// <summary>
        /// Converts and image using conversion path.
        /// </summary>
        /// <param name="srcImage">Input image.</param>
        /// <param name="conversionPath">Conversion path. If zero-length source is returned. If null null is returned.</param>
        /// <param name="copyAlways">Forces to copy data even if the casting is enough</param>
        /// <returns>Converted image. (may share data with input image if casting is used)</returns>
        public static IImage Convert(IImage srcImage, ConversionData<ColorInfo>[] conversionPath, bool copyAlways = false)
        {
            if (conversionPath == null)
                return null;

            IImage convertedIm = srcImage;
            bool isImageCopied = false;
            foreach (ConversionData<ColorInfo> conversion in conversionPath)
            {
                convertedIm = conversion.Convert(convertedIm);
                isImageCopied |= conversion.CopiesData;
            }

            if (copyAlways && !isImageCopied)
            {
                var castedIm = convertedIm;
                convertedIm = castedIm.Clone();
                //castedIm.Dispose(); //TODO. should I dispose it ? (I would dispose srcImage -> not good)
            }

            return convertedIm;
        }

        public static List<ConversionData<ColorInfo>> GetPath(ColorInfo src, ColorInfo dst)
        {
            List<ConversionData<ColorInfo>> path;
            shorthestPaths.TryGetValue(src, dst, out path);

            return path ?? new List<ConversionData<ColorInfo>>();
        }

        /// <summary>
        /// Finds the most inexpensive conversion path.
        /// </summary>
        /// <param name="srcInfo">Source color info.</param>
        /// <param name="preferedDestFormats">Allowed destination formats.</param>
        /// <returns>The most inexpensive path.</returns>
        public static List<ConversionData<ColorInfo>> GetPath(ColorInfo srcInfo, params ColorInfo[] preferedDestFormats)
        {
            List<ConversionData<ColorInfo>> minPath = new List<ConversionData<ColorInfo>>();
            var minCost = Double.MaxValue;

            foreach (var destInfo in preferedDestFormats)
            {
                var path = GetPath(srcInfo, destInfo);
                if (path.Count == 0)
                    continue;

                var cost = costMatrix[srcInfo][destInfo];

                if (cost < minCost)
                {
                    minCost = cost;
                    minPath = path;
                }
            }

            return minPath;
        }

        /// <summary>
        /// Returns if an conversion path requires data copy or not.
        /// </summary>
        /// <param name="conversionPath">Conversion path</param>
        public static bool CopiesData(this IEnumerable<ConversionData<ColorInfo>> conversionPath)
        {
            if (conversionPath == null)
                throw new Exception("Conversion path can not be null!");

            var isImageCopied = conversionPath.Where(x => x.CopiesData).Count() != 0;
            return isImageCopied;
        }
    }
}
