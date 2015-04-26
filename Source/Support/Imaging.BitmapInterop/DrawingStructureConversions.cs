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

using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for System.Drawing namespace and Accord.NET extensions structure conversion.
    /// </summary>
    public static class DrawingStructureConversions
    {
        #region Point conversions

        /// <summary>
        /// Converts <see cref="System.Drawing.Point"/> to the <see cref="AForge.IntPoint"/>.
        /// </summary>
        /// <param name="point"><see cref="System.Drawing.Point"/></param>
        /// <returns><see cref="AForge.IntPoint"/></returns>
        public static Point ToPt(this System.Drawing.Point point)
        {
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// Converts <see cref="System.Drawing.PointF"/> to the <see cref="AForge.Point"/>.
        /// </summary>
        /// <param name="point"><see cref="System.Drawing.PointF"/></param>
        /// <returns><see cref="AForge.Point"/></returns>
        public static PointF ToPt(this System.Drawing.PointF point)
        {
            return new PointF(point.X, point.Y);
        }

        /// <summary>
        /// Converts to <see cref="AForge.IntPoint"/> to the <see cref="System.Drawing.Point"/>.
        /// </summary>
        /// <param name="point"><see cref="AForge.IntPoint"/></param>
        /// <returns><see cref="System.Drawing.Point"/></returns>
        public static System.Drawing.Point ToPt(this Point point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }

        /// <summary>
        /// Converts to <see cref="AForge.Point"/> to the <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// <param name="point"><see cref="AForge.Point"/></param>
        /// <returns><see cref="System.Drawing.PointF"/></returns>
        public static System.Drawing.PointF ToPt(this PointF point)
        {
            return new System.Drawing.PointF(point.X, point.Y);
        }

        #endregion

        #region Rectangle conversions

        /// <summary>
        /// Converts the <see cref="System.Drawing.Rectangle"/> to the <see cref="Accord.Extensions.Rectangle"/>.
        /// </summary>
        /// <param name="rect"><see cref="System.Drawing.Rectangle"/></param>
        /// <returns><see cref="Accord.Extensions.Rectangle"/></returns>
        public static Rectangle ToRect(this System.Drawing.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Converts the <see cref="System.Drawing.RectangleF"/> to the <see cref="Accord.Extensions.RectangleF"/>.
        /// </summary>
        /// <param name="rect"><see cref="System.Drawing.RectangleF"/></param>
        /// <returns><see cref="Accord.Extensions.RectangleF"/></returns>
        public static RectangleF ToRect(this System.Drawing.RectangleF rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Converts the <see cref="Accord.Extensions.Rectangle"/> to the <see cref="System.Drawing.Rectangle"/>.
        /// </summary>
        /// <param name="rect"><see cref="Accord.Extensions.Rectangle"/></param>
        /// <returns><see cref="System.Drawing.Rectangle"/></returns>
        public static System.Drawing.Rectangle ToRect(this Rectangle rect)
        {
            return new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Converts the <see cref="Accord.Extensions.RectangleF"/> to the <see cref="System.Drawing.RectangleF"/>.
        /// </summary>
        /// <param name="rect"><see cref="Accord.Extensions.RectangleF"/></param>
        /// <returns><see cref="System.Drawing.RectangleF"/></returns>
        public static System.Drawing.RectangleF ToRect(this RectangleF rect)
        {
            return new System.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion

        #region Size conversions

        /// <summary>
        /// Converts the <see cref="System.Drawing.Size"/> to the <see cref="Accord.Extensions.Size"/>.
        /// </summary>
        /// <param name="size"><see cref="System.Drawing.Size"/></param>
        /// <returns><see cref="Accord.Extensions.Size"/></returns>
        public static Size ToSize(this System.Drawing.Size size)
        {
            return new Size(size.Width, size.Height);
        }

        /// <summary>
        /// Converts the <see cref="System.Drawing.SizeF"/> to the <see cref="Accord.Extensions.SizeF"/>.
        /// </summary>
        /// <param name="size"><see cref="System.Drawing.SizeF"/></param>
        /// <returns><see cref="Accord.Extensions.SizeF"/></returns>
        public static SizeF ToSize(this System.Drawing.SizeF size)
        {
            return new SizeF(size.Width, size.Height);
        }

        /// <summary>
        /// Converts the <see cref="Accord.Extensions.Size"/> to the <see cref="System.Drawing.Size"/>.
        /// </summary>
        /// <param name="size"><see cref="Accord.Extensions.Size"/></param>
        /// <returns><see cref="System.Drawing.Size"/></returns>
        public static System.Drawing.Size ToSize(this Size size)
        {
            return new System.Drawing.Size(size.Width, size.Height);
        }

        /// <summary>
        /// Converts the <see cref="Accord.Extensions.SizeF"/> to the <see cref="System.Drawing.SizeF"/>.
        /// </summary>
        /// <param name="size"><see cref="Accord.Extensions.SizeF"/></param>
        /// <returns><see cref="System.Drawing.SizeF"/></returns>
        public static System.Drawing.SizeF ToSize(this SizeF size)
        {
            return new System.Drawing.SizeF(size.Width, size.Height);
        }

        #endregion

    }
}
