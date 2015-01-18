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

using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Extensions.Math.Geometry;
using MoreLinq;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using RangeF = AForge.Range;
using System.IO;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for the annotation database type.
    /// </summary>
    public static class AnnotationDatabaseExtensions
    {
        /// <summary>
        /// Clones the database (tag is cloned only if implements <see cref="ICloneable"/> interface).
        /// </summary>
        /// <param name="data">Database to clone.</param>
        /// <returns>Cloned database.</returns>
        public static Database Clone(this Database data)
        {
            Database clonedDb = new Database();

            foreach (var d in data)
            {
                clonedDb.Add(d.Key, new List<Annotation>());

                foreach (var imgAnn in d.Value)
                {
                    clonedDb[d.Key].Add((Annotation)imgAnn.Clone());
                }
            }

            return clonedDb;
        }

        /// <summary>
        /// Gets the annotations of the specified type with the same label.
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="type">Annotation type.</param>
        /// <returns>Annotations per label for the specified annotation type.</returns>
        public static Dictionary<string, List<KeyValuePair<string, Annotation>>> GetAnnotationsByLabels(this Database data, AnnotationType type)
        {
            var annotations = new Dictionary<string, List<KeyValuePair<string, Annotation>>>();

            foreach (var pair in data.GetAnnotations())
            {
                if (pair.Value.Type != type) continue;

                var annLabel = pair.Value.Label;

                if (!annotations.ContainsKey(annLabel))
                    annotations.Add(annLabel, new List<KeyValuePair<string, Annotation>>());

                annotations[annLabel].Add(pair);
            }

            return annotations;
        }

        /// <summary>
        /// Gets annotations from all images (flattens the database).
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="imgKeySelector">Image key selector function. Returns true if the provided image key should be selected.</param>
        /// <param name="annotationSelector">Annotation selector function. Returns true if the provided annotation should be selected.</param>
        /// <returns>Annotations from all images.</returns>
        public static IEnumerable<KeyValuePair<string, Annotation>> GetAnnotations(this Database data, Func<string, bool> imgKeySelector, Func<Annotation, bool> annotationSelector)
        {
            var annotations = from pair in data
                              where imgKeySelector(pair.Key)
                              from ann in pair.Value
                              where annotationSelector(ann)
                              select new KeyValuePair<string, Annotation>(pair.Key, ann);

            return annotations;
        }

        /// <summary>
        /// Gets annotations from all images (flattens the database).
        /// </summary>
        /// <param name="data">Database.</param>
        public static IEnumerable<KeyValuePair<string, Annotation>> GetAnnotations(this Database data)
        {
            return GetAnnotations(data, (_) => true, (_) => true);
        }

        /// <summary>
        /// Count the number of annotations of the specified type with the same label.
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="type">Annotation type.</param>
        /// <returns>The number of annotations per label for the specified annotation type.</returns>
        public static Dictionary<string, int> AnnotationCountByLabels(this Database data, AnnotationType type)
        {
            Dictionary<string, int> labelCounts = new Dictionary<string, int>();

            foreach (var pair in data.GetAnnotationsByLabels(type))
            {
                labelCounts[pair.Key] = pair.Value.Count;
            }

            return labelCounts;
        }

        /// <summary>
        /// Gets the number of annotations which have label that matches specified predicate.
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="labelPredicate">A label matching predicate.</param>
        /// <returns>Number of annotations.</returns>
        /// <example>
        /// var nAnnotations = database.NumberOfAnnotations((label) => label.Contains("Bus"));
        /// Console.WriteLine(nAnnotations);
        /// </example>
        public static int AnnotationCount(this Database data, Func<string, bool> labelPredicate)
        {
            var nAnnotations = data.GetAnnotations((_) => true, (ann) => labelPredicate(ann.Label))
                                   .Count();

            return nAnnotations;
        }

        /// <summary>
        /// Gets the number of annotations in the database.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AnnotationCount(this Database data)
        {
            return AnnotationCount(data, (_) => true);
        }    
    }
}
