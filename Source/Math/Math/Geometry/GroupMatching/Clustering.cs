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
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Represents collections of elements in one group.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    public class Cluster<T>
    {
        public Cluster(IList<T> detections, IList<float> detectionWeights, T representative)
        {
            this.Detections = detections;
            this.Weights = detectionWeights;
            this.Representative = representative;
            this.CumulativeWeight = detectionWeights.Sum();
        }

        /// <summary>
        /// Gets the number of neighbors within a group.
        /// </summary>
        public int Neighbours { get { return this.Detections.Count; } }

        /// <summary>
        /// Gets the collection of elements within the group.
        /// </summary>
        public IList<T> Detections { get; set; }
        /// <summary>
        /// Detection importance weights.
        /// </summary>
        public IList<float> Weights { get; set; }

        /// <summary>
        /// Gets the group representative chosen by average function in group matching algorithm.
        /// </summary>
        public T Representative { get; private set; }
        /// <summary>
        /// Cumulative weight for the group.
        /// </summary>
        public float CumulativeWeight { get; private set; }
    }

    /// <summary>
    ///   Group matching algorithm for detection region averaging.
    /// </summary>
    /// <remarks>
    ///   This class can be seen as a post-processing filter. Its goal is to
    ///   group near or contained regions together in order to produce more
    ///   robust and smooth estimates of the detected regions.
    /// </remarks>
    public abstract class Clustering<T>
    {
        /// <summary>
        /// Creates a new group matcher.
        /// </summary>
        /// <param name="minGroupWeight">
        /// Minimum group weight threshold. Group with less than <paramref name="minGroupWeight"/> will be discarded.
        /// <para>If the all weights are equal to one, the <paramref name="minGroupWeight"/> represents the minimum number of neighbors.</para>
        /// </param>
        public Clustering(float minGroupWeight = 1f)
        {
            this.MinGroupWeight = minGroupWeight;
        }

        /// <summary>
        /// Minimum group weight threshold. Group with less than minimum group weight will be discarded.
        /// <para>If the all weights are equal to one, the minimum group weight represents the minimum number of neighbors.</para>
        /// </summary>
        public float MinGroupWeight { get; set; }

        /// <summary>
        /// Groups detections. All weights are set to one.
        /// </summary>
        /// <param name="detections">Detections.</param>
        public IList<Cluster<T>> Group(IList<T> detections)
        {
            var detectionWeights = Enumerable.Repeat(1f, detections.Count).ToArray();
            return Group(detections, detectionWeights);
        }

        /// <summary>
        /// Groups weighted detections.
        /// </summary>
        /// <param name="detections">Detections.</param>
        /// <param name="detectionWeights">Detection importance weights.</param>
        /// <returns>Groups.</returns>
        public IList<Cluster<T>> Group(IList<T> detections, IList<float> detectionWeights)
        {
            //get cluster labels
            int numberOfClusters;
            var clusterLabels = findClusters(detections, out numberOfClusters);

            var clusterDetections = new List<T>[numberOfClusters];
            var clusterDetectionWeights = new List<float>[numberOfClusters];

            //initialize arrays
            for (int clusterIdx = 0; clusterIdx < numberOfClusters; clusterIdx++)
            {
                clusterDetections[clusterIdx] = new List<T>();
                clusterDetectionWeights[clusterIdx] = new List<float>();
            }

            //get cluster members
            for (int i = 0; i < detections.Count; i++)
            {
                var clusterLabel = clusterLabels[i];

                clusterDetections[clusterLabel - 1].Add(detections[i]);
                clusterDetectionWeights[clusterLabel - 1].Add(detectionWeights[i]);
            }

            //make clusters
            var clusters = new List<Cluster<T>>();
            for (int clusterIdx = 0; clusterIdx < numberOfClusters; clusterIdx++)
            {
                var representative = GetRepresentative(clusterDetections[clusterIdx], clusterDetectionWeights[clusterIdx]);
                var cluster = new Cluster<T>(clusterDetections[clusterIdx], clusterDetectionWeights[clusterIdx], representative);

                if (cluster.CumulativeWeight >= MinGroupWeight)
                    clusters.Add(cluster);
            }

            return clusters;
        }

        static int UNVISITED_VERTEX_LABEL = 0;

        /// <summary>
        /// Marks visited vertices by applying depth first search.
        /// </summary>
        /// <param name="vertices">Vertices.</param>
        /// <param name="labels">Cluster labels.</param>
        /// <param name="startNodeIndex">Start node.</param>
        private void markNodes(IList<T> vertices, int[] labels, int startNodeIndex)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                var areAdjacent = AreDetectionsAdjacent(vertices[startNodeIndex], vertices[i]);
                if (labels[i] == UNVISITED_VERTEX_LABEL && areAdjacent)
                {
                    labels[i] = labels[startNodeIndex];
                    markNodes(vertices, labels, i);
                }
            }
        }

        private int[] findClusters(IList<T> vertices, out int numberOfClusters)
        {
            var labels = new int[vertices.Count]; //marked as UNVISITED_VERTEX_LABEL

            var clusterLabel = 1;

            for (int i = 0; i < vertices.Count; i++)
            {
                if (labels[i] == UNVISITED_VERTEX_LABEL)
                {
                    labels[i] = clusterLabel;
                    markNodes(vertices, labels, i);

                    clusterLabel++;
                }
            }

            numberOfClusters = clusterLabel - 1;
            return labels;
        }

        /// <summary>
        /// The function for getting representative of the group.
        /// </summary>
        /// <param name="samples">Detections.</param>
        /// <param name="weights">Detection weights.</param>
        /// <returns>The group representative.</returns>
        protected abstract T GetRepresentative(IList<T> samples, IList<float> weights);

        /// <summary>
        /// Function that checks whether two detections are adjacent.
        /// </summary>
        /// <param name="sampleA">First sample.</param>
        /// <param name="sampleB">Second sample.</param>
        /// <returns>True if two detections are adjacent, false otherwise.</returns>
        protected abstract bool AreDetectionsAdjacent(T sampleA, T sampleB);
    }
}
