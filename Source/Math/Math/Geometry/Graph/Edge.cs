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
    /// Represents edge structure in a graph.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public class Edge<TVertex>
    {
        /// <summary>
        /// Creates new edge structure using two vertices.
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="destination">Destination vertex.</param>
        public Edge(TVertex source, TVertex destination)
        {
            this.Source = source;
            this.Destination = destination;
        }

        /// <summary>
        /// Gets source vertex.
        /// </summary>
        public TVertex Source { get; private set; }
        /// <summary>
        /// Gets destination (sink) vertex.
        /// </summary>
        public TVertex Destination { get; private set; }

        /// <summary>
        /// Compares two objects for equality.
        /// </summary>
        /// <param name="obj">Other object.</param>
        /// <returns>True if two objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Edge<TVertex> == false)
                return false;

            var edge = obj as Edge<TVertex>;

            if (edge.Source.Equals(this.Source) && edge.Destination.Equals(this.Destination))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calculates hash code of the object.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return this.Source.GetHashCode() ^ this.Destination.GetHashCode();
        }

        /// <summary>
        /// Gets object string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return String.Format("{0} -> {1}", Source, Destination);
        }
    }

    /// <summary>
    /// Represents edge structure in a graph and enables to contain user-defined edge information.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TTag">User tag type.</typeparam>
    public class Edge<TVertex, TTag> : Edge<TVertex>
    {
        /// <summary>
        /// Creates a new instance from two vertices and a tag. 
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="destination">Destination vertex.</param>
        /// <param name="tag">USer specifed tag.</param>
        public Edge(TVertex source, TVertex destination, TTag tag)
            : base(source, destination)
        {
            this.Tag = tag;
        }

        /// <summary>
        /// Gets tag (user specified information) for the edge.
        /// </summary>
        public TTag Tag { get; private set; }
    }
}
