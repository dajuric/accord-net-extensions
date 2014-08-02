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
using Accord.Extensions.Math.Geometry;
using MoreLinq;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using RangeF = AForge.Range;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Contains extension methods for the annotation database type.
    /// </summary>
    public static class AnnotationDatabaseExtensions
    {
        /// <summary>
        /// Clones the database (tag is not cloned).
        /// </summary>
        /// <param name="data">Database to clone.</param>
        /// <returns>Cloned database.</returns>
        public static Database Clone(this Database data)
        {
            return data.ModifyAnnotations(x => (Annotation)x.Clone());
        }

        /// <summary>
        /// Gets the number of annotations in the database.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int NumberOfAnnotations(this Database data)
        {
            return NumberOfAnnotations(data, (_) => true);
        }

        /// <summary>
        /// Gets the number of annotations which have label that matches specified predicate.
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="annotationLabelFunc">A label matching predicate.</param>
        /// <returns>Number of annotations.</returns>
        /// <example>
        /// var nAnnotations = database.NumberOfAnnotations((label) => label.Contains("Bus"));
        /// Console.WriteLine(nAnnotations);
        /// </example>
        public static int NumberOfAnnotations(this Database data, Func<string, bool> annotationLabelFunc)
        {
            var nAnnotations = data.GetAnnotations((_) => true, (ann) => annotationLabelFunc(ann.Label))
                                   .Count();

            return nAnnotations;
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
        /// Modifies databse annotations and creates new database.
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="modifierFunc">Annotation modifier function.</param>
        /// <returns>New database with modified annotations.</returns>
        public static Database ModifyAnnotations(this Database data, Func<Annotation, Annotation> modifierFunc)
        {
            var newData = new Database();

            foreach (var imgAnns in data)
            {
                newData[imgAnns.Key] = new List<Annotation>();

                foreach (var imgAnn in imgAnns.Value)
                {
                    var modifiedAnn = modifierFunc(imgAnn);
                    newData[imgAnns.Key].Add(modifiedAnn);
                }
            }

            return newData;
        }

        //TODO: enable support for arbitrary polygons.
        /// <summary>
        /// Randomizes locations of the annotations. The annotations are assumed to be rectangles. (support for arbitrary polygons will be enabled in one of the future releases).
        /// </summary>
        /// <param name="data">Database.</param>
        /// <param name="locationRand">Location randomization range.</param>
        /// <param name="scaleRand">Scale randomization range.</param>
        /// <param name="nRandsPerSample">The number of generated (randomized) samples per sample.</param>
        /// <returns>New database with generated randomized annotations.</returns>
        public static Database Randomize(this Database data, Pair<RangeF> locationRand, Pair<RangeF> scaleRand, int nRandsPerSample)
        {
            var newData = new Database();

            foreach (var imgAnns in data)
            {
                newData[imgAnns.Key] = new List<Annotation>();

                foreach (var imgAnn in imgAnns.Value)
                {
                    var rect = (RectangleF)imgAnn.Polygon.BoundingRect();

                    //randomize
                    var randRects = rect.Randomize(locationRand, scaleRand, nRandsPerSample);

                    //create annotations from a original annotation
                    randRects.ForEach(x => newData[imgAnns.Key].Add(new Annotation
                    {
                        Label = imgAnn.Label,
                        Tag = imgAnn.Tag,
                        Polygon = Rectangle.Round(x).Vertices()
                    }));
                }
            }

            return newData;
        }

        /// <summary>
        /// Processes annotated samples in the following way:
        /// <para>   1) Inflates sample bounding rectangle.</para>
        /// <para>   2) Randomizes rectangle and creates <paramref name="nRandsPerSample"/> rectangles by using specified parameters: <paramref name="locationRand"/>, <paramref name="scaleRand"/>.</para>
        /// <para>   3) Rescales rectangle so that its scale satisfies specified <paramref name="widthHeightRatio"/> ratio.</para>
        /// <para>Please note that polygons are treated like rectangles (a bounding rectangle is taken).</para>
        /// </summary>
        /// <param name="data">Database</param>
        /// <param name="widthHeightRatio">Width / height ratio that every rectangle must satisfy.</param>
        /// <param name="locationRand">The location offset boundaries used in randomization.</param>
        /// <param name="scaleRand">The scale offset boundaries used in randomization.</param>
        /// <param name="nRandsPerSample">Number of random samples to create (per sample).</param>
        /// <param name="sampleInfalteFactor">Inflate factor for sample.</param>
        /// <returns>Returns new database with processed samples.</returns>
        public static Database ProcessSamples(this Database data, float widthHeightRatio, Pair<RangeF> locationRand, Pair<RangeF> scaleRand, int nRandsPerSample, float sampleInfalteFactor = 0)
        {
            var newData = new Database();

            foreach (var imgAnns in data)
            {
                newData[imgAnns.Key] = new List<Annotation>();

                foreach (var imgAnn in imgAnns.Value)
                {
                    var rect = imgAnn.Polygon.BoundingRect();

                    //1) inflate if requested
                    var infaltedRect = (RectangleF)rect.Inflate(sampleInfalteFactor, sampleInfalteFactor);

                    //2) randomize
                    var randRects = infaltedRect.Randomize(locationRand, scaleRand, nRandsPerSample);

                    //3) rescale
                    randRects = randRects.Select(x => x.ScaleTo(widthHeightRatio, correctLocation: true));

                    //create annotations from a original annotation
                    randRects.ForEach(x => newData[imgAnns.Key].Add(new Annotation 
                                                        { 
                                                            Label = imgAnn.Label, 
                                                            Tag = imgAnn.Tag, 
                                                            Polygon = Rectangle.Round(x).Vertices() 
                                                        })
                                     );
                }
            }

            return newData;
        }

        /// <summary>
        /// Exports annotation database to list of images and list of annotation bounding rectangles.
        /// <para>Returns list of images and bounding rectangles. The two lists have the same length. Images are stored as references.</para>
        /// </summary>
        /// <typeparam name="TImage">Image type.</typeparam>
        /// <param name="data">Database.</param>
        /// <param name="imageGrabberFunc">
        /// Image grabbing function.
        /// Parameters: image key
        /// Returns: image
        /// </param>
        /// <param name="images">List of images.</param>
        /// <param name="boundingRects">list of annotations' bounding rectangles.</param>
        public static void Export<TImage>(this Database data, Func<string, TImage> imageGrabberFunc, out List<TImage> images, out List<Rectangle> boundingRects)
            where TImage: IImage
        {
            images = new List<TImage>();
            boundingRects = new List<Rectangle>();

            foreach (var pair in data)
            {
                var im = imageGrabberFunc(pair.Key);

                foreach (var imageAnn in pair.Value)
                {
                    images.Add(im);
                    boundingRects.Add(imageAnn.Polygon.BoundingRect());
                }
            }
        }

        /// <summary>
        /// Exports annotation database to list of images and list of annotation bounding rectangles.
        /// <para>Returns list of images and bounding rectangles for each image. Images are stored as references.</para>
        /// </summary>
        /// <typeparam name="TImage">Image type.</typeparam>
        /// <param name="data">Database.</param>
        /// <param name="imageGrabberFunc">
        /// Image grabbing function.
        /// Parameters: image key
        /// Returns: image
        /// </param>
        /// <param name="images">List of images.</param>
        /// <param name="boundingRects">list of annotations' bounding rectangles.</param>
        /// <param name="onDoneSampleLoad">Action that is executed on each image load. Parameter is progress [0..1].</param>
        public static void Export<TImage>(this Database data, Func<string, TImage> imageGrabberFunc, out List<TImage> images, out List<List<Rectangle>> boundingRects, Action<float> onDoneSampleLoad = null)
            where TImage : IImage
        {
            images = new List<TImage>();
            boundingRects = new List<List<Rectangle>>();

            var idx = 0;
            foreach (var pair in data)
            {
                var im = imageGrabberFunc(pair.Key);
                var imAnns = pair.Value.Select(x => x.Polygon.BoundingRect()).ToList();

                images.Add(im);
                boundingRects.Add(imAnns);
                idx++;

                /*foreach (var imgAnn in imAnns)
                {
                    if (imgAnn.X < 0 || imgAnn.Y < 0)
                        Console.WriteLine();
                }*/

                if (onDoneSampleLoad != null)
                    onDoneSampleLoad((float)idx / data.Keys.Count);
            }
        }


    }
}
