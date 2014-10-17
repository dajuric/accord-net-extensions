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

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Represents generic point
    /// </summary>
    /// <typeparam name="T">Blittable type.</typeparam>
    [Serializable]
    public struct Point<T>
    {
        /// <summary>
        /// Gets or sets X coordinate.
        /// </summary>
        public T X;
        /// <summary>
        /// Gets or sets Y coordinate.
        /// </summary>
        public T Y;

        /// <summary>
        /// Determines whether the provided object is equal to the current object.
        /// </summary>
        /// <param name="obj">Other object to compare with.</param>
        /// <returns>True if the two objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Point<T> == false)
                return false;

            var pt = (Point<T>)obj;

            if (this.X.Equals(pt.X) && this.Y.Equals(pt.Y))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        /// <summary>
        /// Gets the string representation of the object.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return String.Format("({0}, {1}", this.X, this.Y);
        }
    }
}
