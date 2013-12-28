using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using Accord.Imaging.Filters;
using System.Linq;

namespace LINE2D
{
    public class ImageTemplatePyramid<T>: ITemplatePyramid, ITemplatePyramid<T>
        where T: ImageTemplate, new()
    {
        public T[] Templates { get; private set; }

        public ImageTemplatePyramid() { }

        public void Initialize(T[] templates)
        { 
            this.Templates = templates;
        }

        public static ImageTemplatePyramid<T> CreatePyramid(Image<Bgr, Byte> sourceImage, string classLabel)
        {
            T[] templates = new T[GlobalParameters.MAX_FEATURES_PER_LEVEL.Length];
            Image<Bgr, Byte> image = sourceImage;

            bool isValid = true;
            for (int pyrLevel = 0; pyrLevel < GlobalParameters.MAX_FEATURES_PER_LEVEL.Length; pyrLevel++)
            {
                if (pyrLevel > 0)
                    image = image.PyrDown();

                var newTemplate = new T();
                newTemplate.Initialize(image, GlobalParameters.MAX_FEATURES_PER_LEVEL[pyrLevel], classLabel);
                templates[pyrLevel] = newTemplate;

                if (templates[pyrLevel].Features.Length == 0) //if there is no enough features mark Pyramid as invalid
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

        public static ImageTemplatePyramid<T> CreatePyramidFromPreparedBWImage(Image<Gray, Byte> sourceImage, string classLabel)
        {
            T[] templates = new T[GlobalParameters.MAX_FEATURES_PER_LEVEL.Length];
            Image<Gray, Byte> image = sourceImage;

            for (int pyrLevel = 0; pyrLevel < GlobalParameters.MAX_FEATURES_PER_LEVEL.Length; pyrLevel++)
            {
                if (pyrLevel > 0)
                    image = image.PyrDown();

                var newTemplate = new T();
                newTemplate.Initialize(image, GlobalParameters.MAX_FEATURES_PER_LEVEL[pyrLevel], classLabel);
                templates[pyrLevel] = newTemplate;
            }

            var pyr = new ImageTemplatePyramid<T>(); pyr.Initialize(templates);
            return pyr;
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
