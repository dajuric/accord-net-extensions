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

        public static implicit operator Tuple<T, T>(Pair<T> pair)
        {
            return new Tuple<T, T>(pair.First, pair.Second);
        }

        public static implicit operator Pair<T>(Tuple<T, T> tuple)
        {
            return new Pair<T>(tuple.Item1, tuple.Item2);
        }
    }
}
