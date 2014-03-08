using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RT
{
    public class Detector<TClassifier>
    {
        TClassifier classifier;

        public Detector(TClassifier classifier)
        {
            this.classifier = classifier;

            this.Scale = 1.1f;
            this.StepFunc = (window, scaleFactor) =>
            {
                var step = (int)Math.Max(0.5f * scaleFactor, 1);
                return new Size(step, step);
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

        public float Scale { get; set; }

        public Size StartSize { get; set; }
        public Size EndSize { get; set; }

        public List<Rectangle> Detect<TImage>(TImage image,
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
        /// <param name="classficationFunc">Classifier function. It receives prepared image, current window, and the classifier. 
        /// If an object is found within the provided window it returns <see cref="TOutput"/>, null otherwise.
        /// </param>
        /// <param name="imagePreparationFunc">If an image needs some kind of preparation (e.g. calculating integral image) use this function overload.</param>
        /// <returns>List of the classifier detections.</returns>
        public List<Rectangle> Detect<TImage, TPreparedImage>(TImage image,
                                                            Func<TImage, TPreparedImage, Rectangle, TClassifier, bool> classficationFunc,
                                                            Func<TImage, TPreparedImage> imagePreparationFunc)
            where TImage : IImage
            where TPreparedImage : IImage
        {
            validateProperties();

            var preparedIm = imagePreparationFunc(image);
            var windows = generateWindows(preparedIm.Size).ToArray();

            var detections = new ConcurrentBag<Rectangle>();

            //foreach (var window in windows)
            Parallel.ForEach(windows, (window) =>
            {
                var success = classficationFunc(image, preparedIm, window, classifier);

                if (success)
                    detections.Add(window);
            });

            return detections.ToList();
        }

        private List<Rectangle> generateWindows(Size imageSize)
        {
            var windows = new List<Rectangle>();
            Rectangle window = new Rectangle(0, 0, StartSize.Width, StartSize.Height);

            foreach (var factor in generateScales(imageSize))
            {
                window.Width = (int)Math.Floor(StartSize.Width * factor);
                window.Height = (int)Math.Floor(StartSize.Height * factor);

                var step = StepFunc(window, factor);

                while(window.Bottom < imageSize.Height)
                {
                    while(window.Right < imageSize.Width)
                    {
                        windows.Add(window);

                        window.X += step.Width;
                    }

                    window.X = 0;
                    window.Y += step.Height;
                }
            }

            return windows;
        }

        private IEnumerable<float> generateScales(Size imageSize)
        {
            var maxSize = new Size
            {
                Width = Math.Min(imageSize.Width, EndSize.Width),
                Height = Math.Min(imageSize.Height, EndSize.Height)
            };

            float start = 1f;
            float maxFactor = Math.Min(maxSize.Width / StartSize.Width, maxSize.Height / StartSize.Height);

            for (float f = start; f < maxFactor; f *= Scale)
                yield return f;
        }

        private void validateProperties()
        {
            if (this.Scale != 1 && this.StartSize.Equals(this.EndSize))
                throw new Exception("Scale should be different than 1, if the start size is not equal to destination size.");

            if(this.Scale >= 1 && 
               ((this.StartSize.Width > this.EndSize.Width) || (this.StartSize.Height > this.EndSize.Height)))
            {
                throw new Exception("StartSize is bigger than EndSize, but the scale factor is bigger or equal to 1!");
            }

            if(this.Scale <= 1 && 
               ((this.StartSize.Width < this.EndSize.Width) || (this.StartSize.Height < this.EndSize.Height)))
            {
                throw new Exception("StartSize is smaller than EndSize, but the scale factor is smaller or equal to 1!");
            }
        }
    }
}
