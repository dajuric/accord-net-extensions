#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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

using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    static class WhitePatchExtensionsBase
    {
        /// <summary>
        /// Applies White Patch filter for color normalization (Accord.NET function)
        /// </summary>
        /// <param name="img">image.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        internal static Image<TColor, TDepth> WhitePatch<TColor, TDepth>(this Image<TColor, TDepth> img, bool inPlace = true)
            where TColor: IColor
            where TDepth : struct
        {
            WhitePatch wp = new WhitePatch();
            return img.ApplyFilter(wp, inPlace);
        }
    }

    /// <summary>
    /// Contains extensions for White-patch algorithm.
    /// </summary>
    public static class WhitePatchExtensionsIColor3
    {
        /// <summary>
        /// Applies White Patch filter for color normalization (Accord.NET function)
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static Image<Bgr, byte> WhitePatch(this Image<Bgr, byte> img, bool inPlace = true)
        {
            return WhitePatchExtensionsBase.WhitePatch<Bgr, byte>(img, inPlace);
        }
    }

    /// <summary>
    /// Contains extensions for White-patch algorithm.
    /// </summary>
    public static class WhitePatchExtensionsIColor4
    {
        /// <summary>
        /// Applies White Patch filter for color normalization (Accord.NET function)
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="inPlace">Apply in place or not. If it is set to true return value can be omitted.</param>
        /// <returns>Processed image.</returns>
        public static Image<Bgra, byte> WhitePatch(this Image<Bgra, byte> img, bool inPlace = true)
        {
            return WhitePatchExtensionsBase.WhitePatch<Bgra, byte>(img, inPlace);
        }
    }
}
