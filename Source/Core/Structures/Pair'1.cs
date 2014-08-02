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
    /// Represents pair of <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public class Pair<T>
    {
        /// <summary>
        /// Gets or sets the first element.
        /// </summary>
        public T First;
        /// <summary>
        /// Gets or sets the second element.
        /// </summary>
        public T Second;

        /// <summary>
        /// Constructs the empty pair. 
        /// Properties are initialized to the default type values.
        /// </summary>
        public Pair()
        {
            this.First = default(T);
            this.Second = default(T);
        }

        /// <summary>
        /// Constructs the pair structure.
        /// </summary>
        /// <param name="first">First value.</param>
        /// <param name="second">Second value.</param>
        public Pair(T first, T second)
        {
            this.First = first;
            this.Second = second;
        }

        /// <summary>
        /// Determines whether the current object is equal to the specified one.
        /// </summary>
        /// <param name="obj">Other object to compare with.</param>
        /// <returns>True if the current object is equal to the specified one, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj is Pair<T> == false)
                return false;

            var pair = obj as Pair<T>;

            if (pair.First.Equals(this.First) &&
                pair.Second.Equals(this.Second))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets the object's hash code.
        /// </summary>
        /// <returns>Object's has code.</returns>
        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        /// <summary>
        /// Gets the string representation of the object.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString()
        {
            return String.Format("<{0}, {1}>", First, Second);
        }

        /// <summary>
        /// Converts the structure into System.Tuple{T, T}.
        /// </summary>
        /// <param name="pair">Value pair.</param>
        /// <returns>Tuple.</returns>
        public static implicit operator Tuple<T, T>(Pair<T> pair)
        {
            return new Tuple<T, T>(pair.First, pair.Second);
        }

        /// <summary>
        /// Converts the structure into Accord.Extensions.Pair{T}.
        /// </summary>
        /// <param name="tuple">Tuple.</param>
        /// <returns>Pair.</returns>
        public static implicit operator Pair<T>(Tuple<T, T> tuple)
        {
            return new Pair<T>(tuple.Item1, tuple.Item2);
        }
    }
}
