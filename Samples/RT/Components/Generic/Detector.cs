using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RT
{
    /// <summary>
    /// Generic object detector which uses user provided classifier <see cref="TClassifier"/> to classify between object and non-object.
    /// The sliding window with scale change is used to cover all regions in image. 
    /// <para>Region classification is executed in parallel.</para>
    /// </summary>
    /// <typeparam name="TClassifier">Classifier type.</typeparam>
    public class Detector<TClassifier>
    {
        TClassifier classifier;

        /// <summary>
        /// Creates new object detector from provided classifier.
        /// </summary>
        /// <param name="classifier">User provided classifier which classifies between object and non-object region.</param>
        public Detector(TClassifier classifier)
        {
            this.classifier = classifier;

            this.Scale = 1.2f;
            this.StepFunc = (window, scaleFactor) =>
            {
                var stepX = (int)Math.Max(0.1f * scaleFactor * StartSize.Width, 1);
                var stepY = (int)Math.Max(0.1f * scaleFactor * StartSize.Height, 1);
                return new Size(stepX, stepY);
            };

            this.StartSize = new Size(50, 50);
            this.EndSize = new Size(500, 500);
        }

        /// <summary>
        /// Gets or sets the stepping function.
        /// Stepping function receives the last window, current scale factor (multiplies <see cref="StartSize"/>). 
        /// Function outputs the steps in horizontal and vertical direction.
        /// </summary>
        public Func<Rectangle, float, Size> StepFunc { get; set; }

        /// <summary>
        /// Scale factor for window rescaling.
        /// <para>
        /// If the scale is less than 1:   every component of<see cref="StartSize"/> must be larger  than <see cref="EndSize"/>.
        /// If the scale is bigger than 1: every component of<see cref="StartSize"/> must be smaller than <see cref="EndSize"/>.
        /// IF the scale is 1:             every component of<see cref="StartSize"/> must be equal   to   <see cref="EndSize"/>.  
        /// </para>
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Start window size. <seealso cref="Scale"/>.
        /// </summary>
        public SizeF StartSize { get; set; }
        /// <summary>
        /// End window size. <seealso cref="Scale"/>.
        /// </summary>
        public SizeF EndSize { get; set; }

        /// <summary>
        /// Detects object in an image using specified classifier. 
        /// Classifier is evaluated in each window.
        /// </summary>
        /// <typeparam name="TImage">Image type.</typeparam>
        /// <param name="image">Input image.</param>
        /// <param name="classficationFunc">Classifier function. It receives original image, prepared image, current window, and the classifier. </param>
        /// <returns>Search areas where the specified classifier function returned true.</returns>
        public Rectangle[] Detect<TImage>(TImage image,
                                              Func<TImage, Rectangle, TClassifier, bool> classficationFunc)
            where TImage : class, IImage
        {
            return Detect(image, (im, _, window, stage) => classficationFunc(im, window, stage), im => im);
        }

        /// <summary>
        /// Detects object in an image using specified classifier. 
        /// Classifier is evaluated in each window.
        /// </summary>
        /// <typeparam name="TImage">Image type.</typeparam>
        /// <typeparam name="TPreparedImage">Prepared image type.</typeparam>
        /// <typeparam name="TOutput">Output classifier type.</typeparam>
        /// <param name="image">Input image.</param>
        /// <param name="classficationFunc">Classifier function. It receives original image, prepared image, current window, and the classifier.</param>
        /// <param name="imagePreparationFunc">If an image needs some kind of preparation (e.g. calculating integral image) use this function overload.</param>
        /// <returns>Search areas where the specified classifier function returned true.</returns>
        public Rectangle[] Detect<TImage, TPreparedImage>(TImage image,
                                                            Func<TImage, TPreparedImage, Rectangle, TClassifier, bool> classficationFunc,
                                                            Func<TImage, TPreparedImage> imagePreparationFunc)
            where TImage : IImage
            where TPreparedImage : IImage
        {
            var preparedIm = imagePreparationFunc(image);

            var detections = new ConcurrentBag<Rectangle>();

            var windows = preparedIm.Size.GenerateRegions(StartSize, EndSize, Scale, StepFunc);
            
            var windowPartitioner = Partitioner.Create<Rectangle>(windows, loadBalance: true);
            Parallel.ForEach(windowPartitioner, (window) =>
            //foreach(var window in windows)
            {
                var success = classficationFunc(image, preparedIm, window, classifier);

                if (success)
                    detections.Add(window);
            });

            return detections.ToArray();
        }
    }
}
