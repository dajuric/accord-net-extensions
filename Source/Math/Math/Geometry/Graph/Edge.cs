using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class TaggedEdge<TVertex, TTag> : Edge<TVertex>
    {
        /// <summary>
        /// Creates a new instance from two vertices and a tag. 
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="destination">Destination vertex.</param>
        /// <param name="tag">USer specifed tag.</param>
        public TaggedEdge(TVertex source, TVertex destination, TTag tag)
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
