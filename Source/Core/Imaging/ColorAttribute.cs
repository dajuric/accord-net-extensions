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

namespace Accord.Extensions
{
    /// <summary>
    /// Color info attribute. 
    /// Contains informations about conversion name and specifies whether the structure is generic color space or not.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class ColorInfoAttribute : Attribute
    {
        /// <summary>
        /// Gets the default conversion name.
        /// </summary>
        public const string DEFAULT_CONVERSION_CODENAME = null;

        /// <summary>
        /// Creates the new instance of an <see cref="ColorInfoAttribute"/>.
        /// </summary>
        public ColorInfoAttribute()
        {
            this.ConversionCodename = DEFAULT_CONVERSION_CODENAME;
            this.IsGenericColorSpace = false;
        }

        /// <summary>
        /// Gets or sets conversion codename. Not used. (May be used for EmguCV compatibility)
        /// </summary>
        public string ConversionCodename { get; set; }
        /// <summary>
        /// Gets or sets whether the color space is generic (does not require data conversion).
        /// </summary>
        public bool IsGenericColorSpace { get; set; }
    }
}
