using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Accord.Extensions.Math.Geometry;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using RangeF = AForge.Range;

namespace Accord.Extensions.Imaging
{
    public static class AnnotationDatabaseExtensions
    {
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
        /// <returns>Annotations from all images.</returns>
        public static IEnumerable<Annotation> GetAnnotations(this Database data, Func<string, bool> imgKeySelector, Func<Annotation, bool> annotationSelector)
        {
            var annotations = from pair in data
                              where imgKeySelector(pair.Key)
                              from ann in pair.Value
                              where annotationSelector(ann)
                              select ann;

            return annotations;
        }

        /// <summary>
        /// Gets annotations from all images (flattens the database).
        /// </summary>
        /// <param name="data">Database.</param>
        public static IEnumerable<Annotation> GetAnnotations(this Database data)
        {
            return GetAnnotations(data, (_) => true, (_) => true);
        }

        /// <summary>
        /// Processes annotated samples in the following way:
        /// <para>   1) Inflates sample bounding rectangle.</para>
        /// <para>   2) Randomizes rectangle and creates <see cref="nRandsPerSample"/> rectangles by using specified parameters: <see cref="locationRand"/>, <see cref="scaleRand"/>.</para>
        /// <para>   3) Rescales rectangle so that its scale satisfies specified <see cref="widthHeightRatio"/> ratio.</para>
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
                    randRects.ForEach(x => newData[imgAnns.Key].Add(new Annotation { Label = imgAnn.Label, Tag = imgAnn.Tag, Polygon = Rectangle.Round(x).Vertices() }));
                }
            }

            return newData;
        }

        /// <summary>
        /// Exports annotation database to list of images and list of annotation bounding rectangles.
        /// <para>Returns list of images and bounding rectangles have the same length. Images are stored as references.</para>
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
        public static void Export<TImage>(this Database data, Func<string, TImage> imageGrabberFunc, out List<TImage> images, out List<List<Rectangle>> boundingRects)
            where TImage : IImage
        {
            images = new List<TImage>();
            boundingRects = new List<List<Rectangle>>();

            foreach (var pair in data)
            {
                var im = imageGrabberFunc(pair.Key);
                var imAnns = pair.Value.Select(x => x.Polygon.BoundingRect()).ToList();

                images.Add(im);
                boundingRects.Add(imAnns);

                foreach (var imgAnn in imAnns)
                {
                    if (imgAnn.X < 0 || imgAnn.Y < 0)
                        Console.WriteLine();
                }
            }
        }


    }
}
