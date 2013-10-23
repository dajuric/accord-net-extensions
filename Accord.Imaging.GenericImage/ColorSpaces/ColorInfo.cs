using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Accord.Imaging.Helper;
using Accord.Core;

namespace Accord.Imaging
{
    /// <summary>
    /// Gets color information from color type and depth type.
    /// </summary>
    public class ColorInfo: IEquatable<ColorInfo>
    {
        /// <summary>
        /// Color type (IColor).
        /// </summary>
        public Type ColorType { get; private set; }
        /// <summary>
        /// Conversion codename. Not used. May be used for compatibility with EmguCV.
        /// </summary>
        public string ConversionCodename { get; private set; }
        /// <summary>
        /// Returns if the color-space is generic (e.g. Color3, Color4...) or not.
        /// Conversion from non-generic and generic color-spaces may be casted (does not require data conversion).
        /// </summary>
        public bool IsGenericColorSpace { get; private set; }
        /// <summary>
        /// Number of channels that color has.
        /// </summary>
        public int NumberOfChannels { get; private set; }
        /// <summary>
        /// Number of bytes per channel.
        /// </summary>
        public int ChannelSize { get; private set; }
        /// <summary>
        /// Channel type. Only primitive types are supported.
        /// </summary>
        public Type ChannelType { get; private set; }
        /// <summary>
        /// Color size. Number of channels multiplied by channel size.
        /// </summary>
        public int Size { get { return this.ChannelSize * this.NumberOfChannels; } }

        /// <summary>
        /// Gets color info.
        /// </summary>
        /// <typeparam name="TColor">Member of <see cref="IColor"/></typeparam>
        /// <typeparam name="TDepth">Primitive type</typeparam>
        /// <returns>Color info</returns>
        public static ColorInfo GetInfo<TColor, TDepth>() 
            where TColor : IColor
            where TDepth: struct
        {
            return GetInfo(typeof(TColor), typeof(TDepth));
        }

       /// <summary>
       /// Gets color info.
       /// </summary>
       /// <param name="colorType">Color type. (member of IColor)</param>
       /// <param name="depthType">Primitive type.</param>
       /// <returns>Color info</returns>
        public static ColorInfo GetInfo(Type colorType, Type depthType)
        { 
            return MethodCache.Global.Invoke(getInfo, colorType, depthType);
        }

        private static ColorInfo getInfo(Type colorType, Type depthType)
        {
            //TODO: check if colorType is IColor ?

            ColorInfo ci = new ColorInfo();
            ci.ColorType = colorType;

            var attribVal = Attribute.GetCustomAttribute(colorType, typeof(ColorInfoAttribute)) as ColorInfoAttribute;
            ci.ConversionCodename = (attribVal != null) ? attribVal.ConversionCodename : new ColorInfoAttribute().ConversionCodename;
            ci.IsGenericColorSpace = (attribVal != null) ? attribVal.IsGenericColorSpace : new ColorInfoAttribute().IsGenericColorSpace;

            int numberOfChannels;
            getChannelInfo(colorType, depthType, out numberOfChannels);

            ci.NumberOfChannels = numberOfChannels;
            ci.ChannelType = depthType;
            ci.ChannelSize = Marshal.SizeOf(depthType);

            return ci;
        }

        private static void getChannelInfo(Type colorType, Type depthType, out int numberOfChannels)
        {
            numberOfChannels = 0;

            var channelTypes = colorType.GetFields().Select(x => x.FieldType).ToArray();

            //ensure that all types are the same
            var _depthType = channelTypes[0];
            if (channelTypes.Where(x => x.Equals(_depthType)).Count() != channelTypes.Length)
                throw new Exception("Public fields must have the same type!");

            if (channelTypes.Length == 0)
                throw new Exception("Color structure must have at least one public field!");

            if (!depthType.IsValueType)
                throw new Exception("Channel type must be a value type!");

            if (!depthType.IsPrimitive)
                throw new Exception("Channel type must be a primitive type!");

            numberOfChannels = channelTypes.Length;
        }

        public bool Equals(ColorInfo other)
        {
            return Equals(this, other, ComparableParts.Default);
        }

        /// <summary>
        /// Indicates what parts of color info should be compared.
        /// </summary>
        [Flags]
        public enum ComparableParts
        {
            /// <summary>
            /// Checks color depth type
            /// </summary>
            Depth = 0x1,
            /// <summary>
            /// Checks if one color can be casted to other 
            /// (one of the color infos must be Generic (or the same) 
            /// and have the same number of channels 
            /// and color channels must be the same type.
            /// </summary>
            Castable = 0x3, 
            /// <summary>
            /// Checks color type and depth type (if it is true all other properties are equal as well)
            /// </summary>
            Default =  0x4
        }

        /// <summary>
        /// Compares two color infos.
        /// </summary>
        /// <param name="c1">First color info.</param>
        /// <param name="c2">Second color info.</param>
        /// <param name="cParts">Indicates what to compare. Default is: ComparableParts.Default. </param>
        /// <returns></returns>
        public static bool Equals(ColorInfo c1, ColorInfo c2, ComparableParts cParts)
        {
            if(cParts == ComparableParts.Default)
            {
                return c1.ColorType == c2.ColorType && 
                       c1.ChannelType == c2.ChannelType;
            }

            if (cParts == ComparableParts.Castable)
            {
                bool sameChannels = (c1.NumberOfChannels == c2.NumberOfChannels) && (c1.ChannelType == c2.ChannelType);
                bool colorsAreCastable = c1.IsGenericColorSpace || c2.IsGenericColorSpace || c1.ColorType == c2.ColorType;
                var castable = sameChannels && colorsAreCastable;
                return castable;
            }

            if (cParts == ComparableParts.Depth)
            { 
                var depth = c1.ChannelType == c2.ChannelType;
                return depth;
            }

            throw new Exception("Unknown comparison!");
        }

        /// <summary>
        /// Get string representation.
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return String.Format("<{0}, {1}>", this.ColorType.Name, this.ChannelType.Name);
        }
    }
}
