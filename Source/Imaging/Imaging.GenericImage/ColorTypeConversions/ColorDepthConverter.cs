#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Extensions.Math.Geometry;

namespace Accord.Extensions.Imaging.Converters
{
    /// <summary>
    /// Function delegate for image data conversion.
    /// </summary>
    /// <param name="srcImg">Source image.</param>
    /// <param name="destImg">Destination image.</param>
    public delegate void ConvertDataFunc(IImage srcImg, IImage destImg);

    /// <summary>
    /// Function delegate for image creation (by casting source image, returning source or crating an empty image).
    /// </summary>
    /// <param name="srcImg">Source image.</param>
    /// <param name="destColor">Destination image color info.</param>
    /// <returns>Destination image.</returns>
    public delegate IImage CreateImageFunc(IImage srcImg, ColorInfo destColor);

    /// <summary>
    /// Represents the main class for color and conversion.
    /// It uses search methods to find appropriate conversion paths.
    /// <para>Methods can be used as extensions.</para>
    /// </summary>
    public static class ColorDepthConverter
    {
        /// <summary>
        /// To prevent paths like (Bgr, byte) => (Color3, byte) => (Hsv, byte), generic color conversions are weighted by arbitrary large weight.
        /// </summary>
        public static int GENERIC_COLOR_CAST_OFFSET = 500;

        /// <summary>
        /// Conversion costs.
        /// </summary>
        public enum ConversionCost: int
        {
            /// <summary>
            /// Source returning cost.
            /// </summary>
            ReturnSource = 0,
            /// <summary>
            /// Source casting cost.
            /// </summary>
            Cast = 1,
            /// <summary>
            /// Data conversion cost.
            /// </summary>
            DataConvert = 2,
            /// <summary>
            /// No possible conversion (max. cost).
            /// </summary>
            NotPossible = Int32.MaxValue
        }

        /// <summary>
        /// Contains information about the conversion for source to destination image.
        /// represents an edge in a built graph.
        /// </summary>
        /// <typeparam name="T">ColorInfo for color conversion or Type for depth conversion.</typeparam>
        public class ConversionData<T> : Edge<T>
        {
            private ConversionCost cost;

            /// <summary>
            /// Creates new conversion data object (an edge in a graph).
            /// </summary>
            /// <param name="source">Source vertex.</param>
            /// <param name="destination">Destination vertex.</param>
            /// <param name="convertFunc">Data conversion function.</param>
            /// <param name="forceSequential">Force conversion to execute sequentially.</param>
            /// <param name="createFunc">Destination image creation function.</param>
            /// <param name="cost">Conversion cost.</param>
            public ConversionData(T source, T destination, ConvertDataFunc convertFunc, bool forceSequential = false, CreateImageFunc createFunc = null, ConversionCost cost = ConversionCost.DataConvert)
                :base(source, destination)
            {
                this.CreateFunc = createFunc;
                this.ConvertFunc = convertFunc;
                this.ForceSequential = forceSequential;
                this.cost = cost;
            }

            /// <summary>
            /// Returns conversion data when source image has to be returned.
            /// </summary>
            /// <param name="source">Source's image color info.</param>
            /// <returns>Conversion info.</returns>
            public static ConversionData<ColorInfo> AsReturnSource(ColorInfo source)
            {
                return new ConversionData<ColorInfo>(source, 
                                                    source, 
                                                    (srcImg, dstImg) => { }, true, 
                                                    (srcImg, destColor) => srcImg,
                                                    ConversionCost.ReturnSource);
            }

            /// <summary>
            /// Returns conversion data when source image has to be casted.
            /// </summary>
            /// <param name="source">Source's image color info.</param>
            /// <param name="destination">Destination image color info.</param>
            /// <returns>Conversion info.</returns>
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

            /// <summary>
            /// Returns conversion data when source data has to be converted.
            /// </summary>
            /// <param name="source">Source's image color info.</param>
            /// <param name="destination">Destination image color info.</param>
            /// <param name="convertFunc">Data conversion function.</param>
            /// <param name="forceSequential">Force conversion to execute sequentially.</param>
            /// <returns>Conversion info.</returns>
            public static ConversionData<ColorInfo> AsConvertData(ColorInfo source, ColorInfo destination, ConvertDataFunc convertFunc, bool forceSequential = false)
            {
                return new ConversionData<ColorInfo>(source, 
                                                     destination, 
                                                     convertFunc, 
                                                     forceSequential, 
                                                     (srcImg, destColor) => Image.Create(destColor, srcImg.Width, srcImg.Height),
                                                     ConversionCost.DataConvert);
            }

            /// <summary>
            /// Returns conversion data for color conversion with specified channel depth conversion data.
            /// </summary>
            /// <param name="colorType">Destination color type.</param>
            /// <param name="depthConversionData">Channel type conversion data.</param>
            /// <returns>Conversion info.</returns>
            public static ConversionData<ColorInfo> AsConvertDepth(Type colorType, ConversionData<Type> depthConversionData)
            {
                CreateImageFunc creationFunc = depthConversionData.CreateFunc ??
                                                   ((srcImg, destColor) => Image.Create(destColor, srcImg.Width, srcImg.Height));

                var srcColor = ColorInfo.GetInfo(colorType, depthConversionData.Source);
                var dstColor = ColorInfo.GetInfo(colorType, depthConversionData.Destination);

                return new ConversionData<ColorInfo>
                       (
                          srcColor,
                          dstColor,
                          depthConversionData.ConvertFunc,
                          depthConversionData.ForceSequential,
                          creationFunc,
                          depthConversionData.Cost
                       );
            }

            /// <summary>
            /// Gets or sets destination image creation function.
            /// </summary>
            public CreateImageFunc CreateFunc { get; set; }
            /// <summary>
            /// Gets or sets data conversion function.
            /// </summary>
            public ConvertDataFunc ConvertFunc { get; set; }
            /// <summary>
            /// Gets or sets whether data conversion has to be executed sequentially.
            /// </summary>
            public bool ForceSequential { get; set; }

            /// <summary>
            /// Gets or sets conversion cost (edge cost).
            /// </summary>
            public ConversionCost Cost
            {
                get 
                {
                    if (ConvertFunc == null) return ConversionCost.NotPossible;
                    return cost;
                }
                set { cost = value; }
            }

            /// <summary>
            /// Returns true is the specified conversion copies data, false otherwise.
            /// </summary>
            public bool CopiesData
            {
                get { return !(Cost == ConversionCost.Cast || Cost == ConversionCost.ReturnSource); }
            }
        }

        static List<IColor> genericColors;
        static List<ConversionData<Type>> depthConversions;
        static Dictionary<ColorInfo, Dictionary<ColorInfo, ConversionData<ColorInfo>>> graph;
        static Dictionary<ColorInfo, Dictionary<ColorInfo, List<ConversionData<ColorInfo>>>> shorthestPaths;

        #region Initialization

        static ColorDepthConverter()
        {
            genericColors = new List<IColor>();
            depthConversions = new List<ConversionData<Type>>();
            graph = new Dictionary<ColorInfo, Dictionary<ColorInfo, ConversionData<ColorInfo>>>();

            ColorDepthConverters.Initialize();
            Initialize();
        }

        /// <summary>
        /// Builds a graph for added conversions (built-in conversion are added by default) and determines conversion paths by using Floyd-Warshall algorithm.
        /// </summary>
        public static void Initialize()
        {
            addGenericConversions();
            addDepthConversions();
            addSelfPaths();

            Dictionary<ColorInfo, Dictionary<ColorInfo, double>> costMatrix;
            shorthestPaths = graph.FindAllPaths(x =>
            {
                var cost = (int)x.Cost;
                if (x.Cost == ConversionCost.Cast) cost += GENERIC_COLOR_CAST_OFFSET;
                return cost;
            },
            out costMatrix);

            Remove((srcColor, dstColor, path) =>
            {
                //allow only immediate generic-color conversions (depth conversion)
                if (srcColor.IsGenericColorSpace && dstColor.IsGenericColorSpace)
                {
                    if (path.Count > 1)
                        return true;
                }

                //allow only cast between generic and non-generic color type
                if ((srcColor.IsGenericColorSpace && !dstColor.IsGenericColorSpace) || (!srcColor.IsGenericColorSpace && dstColor.IsGenericColorSpace))
                {
                    if (path.CopiesData())
                        return true;
                }

                return false; //return false if the path is valid (does not need to be removed)
            });
        }

        private static void addDepthConversions()
        {
            var colors = graph.GetVertices<ColorInfo, ConversionData<ColorInfo>>().ToList();

            foreach (var color in colors)
            {
                foreach (var depthConversion in depthConversions)
                {
                    var colorConversion = ConversionData<ColorInfo>.AsConvertDepth(color.ColorType, depthConversion);
                    graph.AddEdge(colorConversion);
                }
            }
        }

        private static void addGenericConversions()
        {
            var colors = graph.GetVertices<ColorInfo, ConversionData<ColorInfo>>().ToList();

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
            var colors = graph.GetVertices<ColorInfo, ConversionData<ColorInfo>>().ToList();

            foreach (var color in colors)
            {
                Add(ConversionData<ColorInfo>.AsReturnSource(color));
            }
        }

        #endregion

        #region Graph manipulation

        /// <summary>
        /// Adds conversion info (edge) to the graph.
        /// </summary>
        /// <param name="conversionData"></param>
        public static void Add(ConversionData<ColorInfo> conversionData)
        {
            graph.AddEdge(conversionData);
        }

        /// <summary>
        /// Registers generic color.
        /// </summary>
        /// <param name="genericColor">Generic color.</param>
        public static void Add(IColor genericColor)
        {
            if (ColorInfo.GetInfo(genericColor.GetType()).IsGenericColorSpace == false)
                throw new Exception("The provided color is not generic color type!");

            genericColors.Add(genericColor);
        }

        /// <summary>
        /// Adds depth conversion info (edge) to channel conversion graph.
        /// </summary>
        /// <param name="depthConversionData">Depth conversion data.</param>
        public static void Add(ConversionData<Type> depthConversionData)
        {
            depthConversions.Add(depthConversionData);
        }

        /// <summary>
        /// Removes the paths forbidden by the specified rules.
        /// </summary>
        /// <param name="forbidRule"></param>
        public static void Remove(Func<ColorInfo, ColorInfo, List<ConversionData<ColorInfo>>, bool> forbidRule)
        {
            foreach (var v1 in shorthestPaths.Keys)
            {
                foreach (var v2 in shorthestPaths.Keys)
                {
                    if (forbidRule(v1, v2, shorthestPaths[v1][v2]))
                        shorthestPaths[v1][v2] = new List<ConversionData<ColorInfo>>();
                }
            }

        }

        #endregion

        /// <summary>
        /// Gets the conversion path.
        /// <para>If the conversion is not allowed or does not exist an empty list is returned.</para>
        /// </summary>
        /// <param name="src">Source color.</param>
        /// <param name="dst">Destination color.</param>
        /// <returns>Conversion path.</returns>
        public static List<ConversionData<ColorInfo>> GetPath(this ColorInfo src, ColorInfo dst)
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
        public static List<ConversionData<ColorInfo>> GetPath(this ColorInfo srcInfo, params ColorInfo[] preferedDestFormats)
        {
            List<ConversionData<ColorInfo>> minPath = new List<ConversionData<ColorInfo>>();
            var minCost = Double.MaxValue;

            foreach (var destInfo in preferedDestFormats)
            {
                var path = GetPath(srcInfo, destInfo);
                if (path.Count == 0)
                    continue;

                var cost = path.PathCost();

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
        public static bool CopiesData(this IList<ConversionData<ColorInfo>> conversionPath)
        {
            if (conversionPath == null)
                throw new Exception("Conversion path can not be null!");

            var isImageCopied = conversionPath.Where(x => x.CopiesData).Count() != 0;
            return isImageCopied;
        }

        /// <summary>
        /// Gets the cost of the specified conversion path.
        /// </summary>
        /// <param name="conversionPath">Conversion path.</param>
        /// <returns>The cost of the path.</returns>
        public static int PathCost(this IList<ConversionData<ColorInfo>> conversionPath)
        {
            if (conversionPath.Count == 0)
                return (int)ConversionCost.NotPossible;

            int cost = 0;
            foreach (var conversion in conversionPath)
            {
                cost += (int)conversion.Cost;
            }

            return cost;
        }

        #region Image extensions
        /// <summary>
        /// Converts an image by using specified conversion path.
        /// </summary>
        /// <param name="img">Image to convert.</param>
        /// <param name="conversionData">Conversion path.</param>
        /// <returns>Converted image.</returns>
        public static IImage Convert(this IImage img, ConversionData<ColorInfo> conversionData)
        {
            var proc = new ParallelProcessor<IImage, IImage>
                (
                  img.Size,
                  () => conversionData.CreateFunc(img, conversionData.Destination),
                  (src, dest, area) => conversionData.ConvertFunc(src.GetSubRect(area), dest.GetSubRect(area))
#if DEBUG
                  ,new ParallelOptions2D { ForceSequential = true }
#else
                  ,new ParallelOptions2D { ForceSequential = conversionData.ForceSequential }
#endif
);

            return proc.Process(img);
        }

        /// <summary>
        /// Converts and image using conversion path.
        /// </summary>
        /// <param name="srcImage">Input image.</param>
        /// <param name="conversionPath">Conversion path. If zero-length source is returned. If null, null value is returned.</param>
        /// <param name="copyAlways">Forces to copy data even if the casting is enough</param>
        /// <returns>Converted image. (may share data with input image if casting is used)</returns>
        public static IImage Convert(this IImage srcImage, IList<ConversionData<ColorInfo>> conversionPath, bool copyAlways = false)
        {
            if (conversionPath == null)
                return null;

            IImage convertedIm = srcImage;
            bool isImageCopied = false;
            foreach (ConversionData<ColorInfo> conversion in conversionPath)
            {
                convertedIm = convertedIm.Convert(conversion);
                isImageCopied |= conversion.CopiesData;
            }

            if (copyAlways && !isImageCopied)
            {
                var castedIm = convertedIm;
                convertedIm = castedIm.Clone();
                //castedIm.Dispose(); //TODO. should I dispose it ? (It would dispose srcImage -> not good)
            }

            return convertedIm;
        }

        #endregion
    }
}
