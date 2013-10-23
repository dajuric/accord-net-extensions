using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Imaging.Helper;
using MethodCache = Accord.Core.MethodCache;
using System.Drawing;
using Accord.Core;

namespace Accord.Imaging.Converters
{
    public delegate void ConvertFunction(IImage srcImg, IImage destImg);

    /// <summary>
    /// Represents the main class for color and conversion.
    /// It uses search methods to find appropriate conversion paths.
    /// </summary>
    public class ColorConverter
    {
        /// <summary>
        /// Contains all information for color conversion.
        /// </summary>
        public class ConversionData
        {
            /// <summary>
            /// Creates and initializes structure.
            /// </summary>
            public ConversionData()
            {
                this.JustNeedsCast = false;
                this.ForceSequential = false;
            }

            /// <summary>
            /// Source color info.
            /// </summary>
            public ColorInfo SourceColorInfo { get; set; }
            /// <summary>
            /// Destination color info
            /// </summary>
            public ColorInfo DestColorInfo { get; set; }
            /// <summary>
            /// Does an image need only cast or not.
            /// Conversion between generic and non-generic color-space or between the same color family need only cast (depth is not included)
            /// If set to true data conversion function is omitted.
            /// See <seealso cref="ColorTypeRequiresOnlyCast"/> for more details.
            /// </summary>
            public bool JustNeedsCast { get; private set; }
            /// <summary>
            /// Function that does color conversion.
            /// </summary>
            public ConvertFunction DataConvertFunc { get; set; }
            /// <summary>
            /// Forces sequential processing.
            /// Useful if conversion operation is already parallel.
            /// </summary>
            public bool ForceSequential { get; set; }

            private static bool ColorTypeRequiresOnlyCast(ColorInfo source, ColorInfo dest)
            {
                bool sameFamily = source.ConversionCodename == dest.ConversionCodename;
                bool cast = (source.IsGenericColorSpace || dest.IsGenericColorSpace || sameFamily) && (source.NumberOfChannels == dest.NumberOfChannels);
                return cast;
            }

            /// <summary>
            /// Finds appropriate conversion data, or dynamically creates conversion path.
            /// Also used for building paths between color-spaces because it inserts appropriate depth conversion (input color-spaces contain the same colors, but different depths).
            /// </summary>
            /// <param name="data">Registered conversions.</param>
            /// <param name="source">Source color info.</param>
            /// <param name="dest">Destination color info.</param>
            /// <returns>Conversion path.</returns>
            internal static List<ConversionData> FindOrTryCreate(IEnumerable<ConversionData> data, ColorInfo source, ColorInfo dest)
            {
                ConversionData info = data.Where(x => x.SourceColorInfo.Equals(source) && x.DestColorInfo.Equals(dest)).FirstOrDefault();
                if (info != null)
                {
                    return new List<ConversionData> { info };
                }
                else //if conversion can not be found check if color is cast-able (generic)
                {
                    bool cast = ColorTypeRequiresOnlyCast(source, dest);

                    if (cast)
                    {
                        List<ConversionData> path = new List<ConversionData>();

                        /****************** cast color ****************/
                        var destInfo = ColorInfo.GetInfo(dest.ColorType, source.ChannelType);

                        info = new ConversionData
                        {
                            SourceColorInfo = source,
                            DestColorInfo = destInfo,
                            JustNeedsCast = true
                        };
                        path.Add(info);
                        /****************** cast color ****************/

                        /*********** convert depth if necessary *************/
                        var depthPath = InsertDepthConversion(destInfo, dest.ChannelType);
                        if (depthPath == null)
                            return null;

                        path.AddRange(depthPath);
                        /*********** convert depth if necessary *************/

                        return path;
                    }
                }

                return null;
            }

            private static List<ConversionData> InsertDepthConversion(ColorInfo srcInfo, Type destChannelType)
            {
                List<ConversionData> path = new List<ConversionData>();

                List<DepthConverter.DepthConversionInfo> depthConversion = DepthConverter.GetMostInexepnsiveConversionPath(srcInfo.ChannelType, destChannelType);
                if (depthConversion == null)
                    return null;

                ConversionData c = new ConversionData 
                { 
                    SourceColorInfo = srcInfo, 
                    DestColorInfo =  ColorInfo.GetInfo(srcInfo.ColorType, depthConversion.First().DestType),
                    DataConvertFunc = depthConversion.First().DataConvertFunc,
                    JustNeedsCast  = depthConversion.First().JustNeedsCast
                };
                path.Add(c);

                for (int i = 1; i < depthConversion.Count; i++)
                {
                    c = new ConversionData
                    {
                        SourceColorInfo = path[0].DestColorInfo,
                        DestColorInfo = ColorInfo.GetInfo(srcInfo.ColorType, depthConversion[i].DestType),
                        DataConvertFunc = depthConversion[i].DataConvertFunc,
                        JustNeedsCast = depthConversion[i].JustNeedsCast
                    };
                    path.Add(c);
                }

                return path;
            }

            /// <summary>
            /// Compacts path - removes path fragments between the same color-spaces.
            /// Can produce zero-length path in case if source and destination color are the same.
            /// </summary>
            /// <param name="path">Conversion path.</param>
            /// <returns>Compacted conversion path.</returns>
            public static List<ConversionData> TryCompactPath(List<ConversionData> path)
            {
                List<ConversionData> compactedPath = new List<ConversionData>();

                for (int i = 0; i < path.Count; i++)
                {
                    var data = path[i];

                    if (data.JustNeedsCast && data.SourceColorInfo.Equals(data.DestColorInfo))
                    {
                        continue;
                    }

                    compactedPath.Add(data);
                }

                /*if (compactedPath.Count < path.Count)
                    return TryCompactPath(compactedPath);
                else*/
                    return compactedPath;
            }

            /// <summary>
            /// Converts an image using information from this structure.
            /// </summary>
            /// <param name="img">Input image.</param>
            /// <returns>Converted image.</returns>
            public IImage Convert(IImage img)
            {
                if (this.JustNeedsCast)
                {
                    return GenericImageBase.Create(this.DestColorInfo, img.ImageData, img.Width, img.Height, img.Stride, img);
                }
                else
                {
                    /*var dest = GenericImageBase.Create(this.DestColorInfo, img.Width, img.Height);
                    this.DataConvertFunc(img.GetSubRect(new Rectangle(0,0, img.Width, img.Height)), dest);
                    return dest;*/

                    var proc = new ParallelProcessor<IImage, IImage>(
                           img.Size,
                           () => GenericImageBase.Create(this.DestColorInfo, img.Width, img.Height),
                           (src, dest, area) => 
                           {
                               this.DataConvertFunc(src.GetSubRect(area), dest.GetSubRect(area));
                           }
#if DEBUG
                           ,new ParallelOptions { ForceSequential = true }
#else
                           ,new ParallelOptions { ForceSequential = this.ForceSequential }
#endif
                           );

                    return proc.Process(img);
                }
            }
        }

        static List<ConversionData> registeredData;
        static ColorConverter()
        {
            registeredData = new List<ConversionData>();
            ColorDepthConverterFactory.Initialize();
        }

        /// <summary>
        /// Registers conversion data.
        /// </summary>
        /// <param name="conversionData"></param>
        public static void Register(ConversionData conversionData)
        {
            if(registeredData.Contains(conversionData) == false)
                registeredData.Add(conversionData);
        }

        /// <summary>
        /// Finds the most inexpensive conversion path in term of <see cref="getPathCostFunc"/>.
        /// </summary>
        /// <param name="srcInfo">Source color info.</param>
        /// <param name="preferedDestFormats">Allowed destination formats.</param>
        /// <returns>The most inexpensive path.</returns>
        public static List<ConversionData> GetMostInexepnsiveConversionPath(ColorInfo srcInfo, params ColorInfo[] preferedDestFormats)
        {
            var paths = new List<List<ConversionData>>();
            var costFunc = getPathCostFunc();

            foreach (var destInfo in preferedDestFormats)
            {
                var path = GetMostInexepnsiveConversionPath(srcInfo, destInfo);

                if (path != null)
                    paths.Add(path);
            }

            if (paths.Count > 0)
                return paths.MinBy(costFunc);
            else
                return null;
        }

        /// <summary>
        /// Finds the most inexpensive conversion path in term of <see cref="getPathCostFunc"/>.
        /// Calculates path or gets the cached one.
        /// </summary>
        /// <param name="srcInfo">Source color info.</param>
        /// <param name="destInfo">Destination color info.</param>
        /// <returns>The most inexpensive path.</returns>
        public static List<ConversionData> GetMostInexepnsiveConversionPath(ColorInfo srcInfo, ColorInfo destInfo)
        {
#if DEBUG
            return getMostInexepnsiveConversionPath(srcInfo, destInfo);
#else
            return MethodCache.Global.Invoke(getMostInexepnsiveConversionPath, srcInfo, destInfo);
#endif
        }

        /// <summary>
        /// Finds the most inexpensive conversion path in term of <see cref="getPathCostFunc"/>.
        /// Calculates path.
        /// </summary>
        /// <param name="srcInfo">Source color info.</param>
        /// <param name="destInfo">Destination color info.</param>
        /// <returns>The most inexpensive path.</returns>
        private static List<ConversionData> getMostInexepnsiveConversionPath(ColorInfo srcInfo, ColorInfo destInfo)
        {
            var costFunc = getPathCostFunc();

            var possiblePaths = getPossibleConversionPaths(srcInfo, destInfo);

            if (possiblePaths.Count > 0)
                return possiblePaths.MinBy(costFunc);
            else
                return null;
        }

        /// <summary>
        /// Gets cost function for path.
        /// The higher the number the more expensive the path.
        /// </summary>
        private static Func<List<ConversionData>, int> getPathCostFunc()
        {
            return (path) => path.Where(x => x.JustNeedsCast == false).Count();
        }


        /// <summary>
        /// Searches for a possible paths to convert source color to destination one by looking at user registered conversions.
        /// Uses <see cref="ConversionData.FindOrTryCreate"/> to build paths (insert depth conversion or create casting).
        /// </summary>
        /// <returns>All possible paths.</returns>
        private static List<List<ConversionData>> getPossibleConversionPaths(ColorInfo srcInfo, ColorInfo destInfo)
        {
            var types = new List<ColorInfo>();

            types.Add(srcInfo);
            types.Add(destInfo);

            foreach (var registerdConversion in registeredData)
            {
                types.Add(registerdConversion.SourceColorInfo);
                types.Add(registerdConversion.DestColorInfo);
            }

            var foundTypePairs = types.BreadthFirstSearch(srcInfo, destInfo,
                                            (a, b) =>
                                                {
                                                    return ConversionData.FindOrTryCreate(registeredData, a, b) != null;
                                                });

            List<List<ConversionData>> paths = new List<List<ConversionData>>();
            foreach (var foundTypePair in foundTypePairs)
            { 
                var pathSegments = foundTypePair.Pairwise((a, b) => ConversionData.FindOrTryCreate(registeredData, a, b));

                var path = new List<ConversionData>();
                pathSegments.ForEach(x => path.AddRange(x));
                path = ConversionData.TryCompactPath(path);

                paths.Add(path);
            }

            return paths;
        }
   
        /// <summary>
        /// Converts and image using conversion path.
        /// </summary>
        /// <param name="srcImage">Input image.</param>
        /// <param name="conversionPath">Conversion path. If zero-length source is returned. If null null is returned.</param>
        /// <param name="copyAlways">Forces to copy data even if the casting is enough</param>
        /// <returns>Converted image. (may share data with input image if casting is used)</returns>
        public static IImage Convert(IImage srcImage, ColorConverter.ConversionData[] conversionPath, bool copyAlways = false)
        {
            if (conversionPath == null)
                return null;

            IImage convertedIm = srcImage;
            foreach (ColorConverter.ConversionData conversion in conversionPath)
            { 
                convertedIm = conversion.Convert(convertedIm);
            }

            var isImageCopied = ConversionPathCopiesData(conversionPath).Value;
            if (copyAlways && !isImageCopied)
            {
                var castedIm = convertedIm;
                convertedIm = castedIm.Clone();
                //castedIm.Dispose(); //TODO. should I dispose it ? (I would dispose srcImage -> not good)
            }

            return convertedIm;
        }

        /// <summary>
        /// Returns if an conversion path requires data copy or not.
        /// Null is returned if an conversion path is null.
        /// </summary>
        /// <param name="conversionPath">Conversion path</param>
        public static bool? ConversionPathCopiesData(IEnumerable<ColorConverter.ConversionData> conversionPath)
        {
            if (conversionPath == null)
                return null; //there is no path

            var isImageCopied = conversionPath.Where(x => x.JustNeedsCast == false).Count() != 0;
            return isImageCopied;
        }
    }

    /// <summary>
    /// Depth converter. It is used internally from <see cref="ColorConverter"/>
    /// </summary>
    static class DepthConverter
    {
        /// <summary>
        /// Contains all information for depth conversion.
        /// </summary>
        public class DepthConversionInfo
        {
            /// <summary>
            /// Creates and initializes structure.
            /// </summary>
            public DepthConversionInfo()
            {
                this.JustNeedsCast = false;
            }

            /// <summary>
            /// Source type.
            /// </summary>
            public Type SourceType { get; set; }
            /// <summary>
            /// Destination type.
            /// </summary>
            public Type DestType { get; set; }
            /// <summary>
            /// Does an image need only cast or not.
            /// Conversion between the same depths need only cast.
            /// If set to true data conversion function is omitted.
            /// Path is compacted later in <see cref="ColorConverter"/>.
            /// </summary>
            public bool JustNeedsCast { get; private set; }
            /// <summary>
            /// Conversion function.
            /// </summary>
            public ConvertFunction DataConvertFunc { get; set; }

            /// <summary>
            /// Finds appropriate conversion data, or dynamically creates conversion path.
            /// </summary>
            /// <param name="data">Registered depth converters.</param>
            /// <param name="source">Source type.</param>
            /// <param name="dest">Destination type.</param>
            /// <returns>Depth conversion structure.</returns>
            public static DepthConversionInfo FindOrTryCreate(IEnumerable<DepthConversionInfo> data, Type source, Type dest)
            {
                DepthConversionInfo info = data.Where(x => x.SourceType == source && x.DestType == dest).FirstOrDefault();

                if (info == null)
                {
                    if (source.Equals(dest))
                    {
                        info = new DepthConversionInfo
                        {
                            SourceType = source,
                            DestType = dest,
                            JustNeedsCast = true
                        };
                    }
                }

                return info;
            }
        }

        static List<DepthConversionInfo> registeredData;

        static DepthConverter()
        {
            registeredData = new List<DepthConversionInfo>();
        }

        /// <summary>
        /// Registers depth converter.
        /// </summary>
        /// <param name="conversionData"></param>
        public static void Register(DepthConversionInfo conversionData)
        {
            if (registeredData.Contains(conversionData) == false)
                registeredData.Add(conversionData);
        }

        /// <summary>
        /// Find the most inexpensive path in terms of <see cref=" getPathCostFunc"/>.
        /// Calculates the path or returns it from the cache.
        /// </summary>
        /// <param name="src">Source type.</param>
        /// <param name="dest">Destination type.</param>
        /// <returns>The most inexpensive path.</returns>
        public static List<DepthConversionInfo> GetMostInexepnsiveConversionPath(Type src, Type dest)
        {
#if DEBUG
            return getMostInexepnsiveConversionPath(src, dest);
#else
            return MethodCache.Global.Invoke(getMostInexepnsiveConversionPath, src, dest);
#endif
        }

        /// <summary>
        /// Find the most inexpensive path in terms of <see cref=" getPathCostFunc"/>.
        /// Calculates the path..
        /// </summary>
        /// <param name="src">Source type.</param>
        /// <param name="dest">Destination type.</param>
        /// <returns>The most inexpensive path.</returns>
        private static List<DepthConversionInfo> getMostInexepnsiveConversionPath(Type src, Type dest)
        {
            var costFunc = getPathCostFunc();

            var possiblePaths = getPossibleConversionPaths(src, dest);

            if (possiblePaths.Count > 0)
                return possiblePaths.MinBy(costFunc);
            else
                return null;
        }

        /// <summary>
        /// Gets the cost path function.
        /// The greater the number the more expensive the path.
        /// </summary>
        private static Func<List<DepthConversionInfo>, int> getPathCostFunc()
        {
            return (path) => path.Count;
        }

        /// <summary>
        /// Searches for a possible paths to convert source depth to destination one by looking at user registered conversions.
        /// Uses <see cref="ConversionData.FindOrTryCreate"/> to build paths (insert depth conversion or create casting).
        /// </summary>
        /// <returns>All possible paths.</returns>
        private static List<List<DepthConversionInfo>> getPossibleConversionPaths(Type src, Type dest)
        {
            var types = new List<Type>();

            foreach (var registerdConversion in registeredData)
            {
                types.Add(registerdConversion.SourceType);
                types.Add(registerdConversion.DestType);
            }

            var foundTypePairs = types.BreadthFirstSearch(src, dest,
                                            (a, b) => registeredData.Exists(x => x.SourceType.Equals(a) && x.DestType.Equals(b)));

            List<List<DepthConversionInfo>> paths = new List<List<DepthConversionInfo>>();
            foreach (var foundTypePair in foundTypePairs)
            {
                var path = foundTypePair.Pairwise((a, b) => DepthConversionInfo.FindOrTryCreate(registeredData, a, b));
                paths.Add(path.ToList());
            }

            return paths;
        }
    }

}
