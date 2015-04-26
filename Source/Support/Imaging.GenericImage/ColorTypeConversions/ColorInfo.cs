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

using Accord.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Imaging
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
        public int ChannelCount { get; private set; }
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
        public int Size { get { return this.ChannelSize * this.ChannelCount; } }

        /// <summary>
        /// Gets color info (depth is taken from color).
        /// </summary>
        /// <typeparam name="TColor">Member of <see cref="IColor"/></typeparam>
        /// <returns>Color info</returns>
        public static ColorInfo GetInfo<TColor>()
            //where TColor : IColor<T>
        {
            return GetInfo(typeof(TColor));
        }

        /// <summary>
        /// Gets color info (depth is taken from color).
        /// </summary>
        /// <param name="colorType">Color type. (member of IColor)</param>
        /// <returns>Color info</returns>
        public static ColorInfo GetInfo(Type colorType)
        {
            return MethodCache.Global.Invoke(getInfo, colorType);
        }

        private static ColorInfo getInfo(Type colorType)
        {
            var channelTypes = colorType.GetFields().Select(x => x.FieldType).ToArray();
            return getInfo(colorType, channelTypes.FirstOrDefault());
        }

        private static ColorInfo getInfo(Type colorType, Type depthType)
        {
            ColorInfo ci = new ColorInfo();
            ci.ColorType = colorType;

            var attribVal = Attribute.GetCustomAttribute(colorType, typeof(ColorInfoAttribute)) as ColorInfoAttribute;
            ci.ConversionCodename = (attribVal != null) ? attribVal.ConversionCodename : new ColorInfoAttribute().ConversionCodename;
            ci.IsGenericColorSpace = (attribVal != null) ? attribVal.IsGenericColorSpace : new ColorInfoAttribute().IsGenericColorSpace;

            int numberOfChannels;
            getChannelInfo(colorType, depthType, out numberOfChannels);

            ci.ChannelCount = numberOfChannels;
            ci.ChannelType = depthType;
            ci.ChannelSize = Marshal.SizeOf(depthType);

            return ci;
        }

        private static void getChannelInfo(Type colorType, Type depthType, out int numberOfChannels)
        {
            numberOfChannels = 0;

            var channelTypes = colorType
                               .GetFields(BindingFlags.Public | ~BindingFlags.Static)
                               .Select(x => x.FieldType)
                               .ToArray();

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

        /// <summary>
        /// Determines whether the object is equal compared to the specified object. 
        /// A default comparison is used. Please see overloads.
        /// </summary>
        /// <param name="other">Other object.</param>
        /// <returns>True if two objects are equal, false otherwise.</returns>
        public bool Equals(ColorInfo other)
        {
            return Equals(other, ComparableParts.Default);
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
            /// Checks if one color can be casted to other (if colors are binary compatible).
            /// <para>
            /// One of the color infos must be Generic (or the same) 
            /// and have the same number of channels 
            /// and color channels must be the same type.
            /// </para>
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
        /// <param name="other">Other color info.</param>
        /// <param name="cParts">Indicates what to compare. Default is: ComparableParts.Default. </param>
        /// <returns></returns>
        public bool Equals(ColorInfo other, ComparableParts cParts)
        {
            if(cParts == ComparableParts.Default)
            {
                return this.ColorType == other.ColorType && 
                       this.ChannelType == other.ChannelType;
            }

            if (cParts == ComparableParts.Castable)
            {
                bool sameChannels = (this.ChannelCount == other.ChannelCount) && (this.ChannelType == other.ChannelType);
                bool colorsAreCastable = this.IsGenericColorSpace || other.IsGenericColorSpace || this.ColorType == other.ColorType;
                var castable = sameChannels && colorsAreCastable;
                return castable;
            }

            if (cParts == ComparableParts.Depth)
            { 
                var depth = this.ChannelType == other.ChannelType;
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

    /// <summary>
    /// Provides extensions for color to array conversion.
    /// </summary>
    public static class ColorToArrayExtensions
    {
        /// <summary>
        /// Converts color to array of type <typeparamref name="TDepth"/>.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <typeparam name="TDepth">Channel type.</typeparam>
        /// <param name="color">Color</param>
        /// <returns>Array whose length is the same as color's number of channels.</returns>
        public static TDepth[] ColorToArray<TColor, TDepth>(this TColor color)
            where TColor : IColor
            where TDepth : struct
        {
            var fields = typeof(TColor).GetFields(BindingFlags.Public | ~BindingFlags.Static);

            TDepth[] arr = new TDepth[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                var rawVal = fields[i].GetValue(color);
                arr[i] = (TDepth)Convert.ChangeType(rawVal, typeof(TDepth));
            }

            return arr;
        }
    }
}
