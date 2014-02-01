using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using Accord.Extensions.Imaging.Filters;
using System.Linq;
using Accord.Extensions;

namespace LINE2D
{
    public class ImageTemplatePyramid<T>: ITemplatePyramid, ITemplatePyramid<T>
        where T: ImageTemplate, new()
    {
        static int DEFAULT_MIN_FEATURES = 30;
        static int[] DEFAULT_MAX_FEATURES_PER_LEVEL = new int[] { 100/*, 100 / 2*/ }; //bigger image towards smaller one   

        public T[] Templates { get; private set; }

        public ImageTemplatePyramid() { }

        public void Initialize(T[] templates)
        { 
            this.Templates = templates;
        }

        public static ImageTemplatePyramid<T> CreatePyramid(Image<Bgr, Byte> sourceImage, string classLabel, int minFeatureStrength = 40, int minNumberOfFeatures = 30, int[] maxNumberOfFeaturesPerLevel = null)
        {
            return CreatePyramid<Bgr, byte>(sourceImage, classLabel,
                                            (image, minFeatures, maxFeatures, label) => 
                                            {
                                                var t = new T();
                                                t.Initialize(sourceImage, minFeatureStrength, maxFeatures, label);
                                                return t;
                                            },
                                            minNumberOfFeatures, maxNumberOfFeaturesPerLevel);
        }

        public static ImageTemplatePyramid<T> CreatePyramidFromPreparedBWImage(Image<Gray, Byte> sourceImage, string classLabel, int minFeatureStrength = 40, int minNumberOfFeatures = 30, int[] maxNumberOfFeaturesPerLevel = null)
        {
            return CreatePyramid<Gray, byte>(sourceImage, classLabel,
                                            (image, minFeatures, maxFeatures, label) =>
                                            {
                                                var t = new T();
                                                t.Initialize(sourceImage, minFeatureStrength, maxFeatures, label);
                                                return t;
                                            },
                                            minNumberOfFeatures, maxNumberOfFeaturesPerLevel);
        }

        private static ImageTemplatePyramid<T> CreatePyramid<TColor, TDepth>(Image<TColor, TDepth> sourceImage, string classLabel, 
                                                                             Func<Image<TColor, TDepth>, int, int, string, T> templateCreationFunc,
                                                                             int minNumberOfFeatures, int[] maxNumberOfFeaturesPerLevel = null)
            where TColor: IColor
            where TDepth: struct
        {
            maxNumberOfFeaturesPerLevel = maxNumberOfFeaturesPerLevel ?? DEFAULT_MAX_FEATURES_PER_LEVEL;

            int nPyramids = maxNumberOfFeaturesPerLevel.Length;
            T[] templates = new T[nPyramids];
            var image = sourceImage;

            bool isValid = true;
            for (int pyrLevel = 0; pyrLevel < nPyramids; pyrLevel++)
            {
                if (pyrLevel > 0)
                    image = image.PyrDown();

                var newTemplate = templateCreationFunc(image, minNumberOfFeatures, maxNumberOfFeaturesPerLevel[pyrLevel], classLabel);
                templates[pyrLevel] = newTemplate;

                if (templates[pyrLevel].Features.Length < minNumberOfFeatures) //if there is no enough features mark Pyramid as invalid
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                var pyr = new ImageTemplatePyramid<T>(); pyr.Initialize(templates);
                return pyr;
            }
            else
                return null;
        }

        #region ITemplatePyramid Interface

        ITemplate[] ITemplatePyramid.Templates
        {
            get
            {
                return this.Templates;
            }
        }

        #endregion
    }
}
