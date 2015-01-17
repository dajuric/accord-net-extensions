using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GOCO
{
    /// <summary>
    /// Generic object detector which uses user provided classifier to classify between object and non-object.
    /// The sliding window with scale change is used to cover all regions in image. 
    /// <para>Region classification is executed in parallel.</para>
    /// </summary>
    public static class DetectorExtensions
    {
        /// <summary>
        /// Detects object in an image using specified classifier. 
        /// Classifier is evaluated in each window.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="classficationFunc">Classifier function. It receives original image, prepared image, current window, and the classifier. Returns the classification confidence.</param>
        /// <returns>Search areas where the specified classifier function returned true.</returns>
        public static IEnumerable<KeyValuePair<Rectangle, float>> Detect(this IList<Rectangle> windows,
                                                                         Func<Rectangle, float> classficationFunc)
        {
            var detections = new ConcurrentBag<KeyValuePair<Rectangle, float>>();

            var windowPartitioner = Partitioner.Create<Rectangle>(windows, loadBalance: true);
            Parallel.ForEach(windowPartitioner, (window) =>
            //foreach(var window in windows)
            {
                var confidence = classficationFunc(window);

                if (confidence > 0)
                    detections.Add(new KeyValuePair<Rectangle, float>(window, confidence));
            });

            return detections;
        }
    }
}
