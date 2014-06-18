using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Represents generic point
    /// </summary>
    /// <typeparam name="T">Blittable type.</typeparam>
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
