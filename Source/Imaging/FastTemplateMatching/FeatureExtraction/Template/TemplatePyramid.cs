using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using Accord.Imaging.Filters;

namespace LINE2D.TemplateMatching
{
    public class TemplatePyramid
    {
        public Template[] Templates = null;

        internal TemplatePyramid(Template[] templates)
        {
            this.Templates = templates;
        }

        public static TemplatePyramid CreatePyramid(Image<Bgr, Byte> sourceImage, string classLabel)
        {
            Template[] templates = new Template[GlobalParameters.MAX_FEATURES_PER_LEVEL.Length];
            Image<Bgr, Byte> image = sourceImage;

            bool isValid = true;
            for (int pyrLevel = 0; pyrLevel < GlobalParameters.MAX_FEATURES_PER_LEVEL.Length; pyrLevel++)
            {
                if (pyrLevel > 0)
                    image = image.PyrDown();

                Template newTemplate = CreateNew(classLabel);
                newTemplate.Initialize(image, GlobalParameters.MAX_FEATURES_PER_LEVEL[pyrLevel], classLabel);
                templates[pyrLevel] = newTemplate;

                if (templates[pyrLevel].Features.Length == 0) //if there is no enough features mark Pyramid as invalid
                {
                    isValid = false; 
                    break;
                }
            }


            if(isValid)
                return new TemplatePyramid(templates);
            else
                return null;
        }

        public static TemplatePyramid CreatePyramidFromPreparedBWImage(Image<Gray, Byte> sourceImage, string classLabel)
        {
            Template[] templates = new Template[GlobalParameters.MAX_FEATURES_PER_LEVEL.Length];
            Image<Gray, Byte> image = sourceImage;

            for (int pyrLevel = 0; pyrLevel < GlobalParameters.MAX_FEATURES_PER_LEVEL.Length; pyrLevel++)
            {
                if (pyrLevel > 0)
                    image = image.PyrDown();

                Template newTemplate = CreateNew(classLabel);
                newTemplate.Initialize(image, GlobalParameters.MAX_FEATURES_PER_LEVEL[pyrLevel], classLabel);
                templates[pyrLevel] = newTemplate;
            }

            return new TemplatePyramid(templates);
        }


        static Dictionary<string, Type> specificTemplateClasses;
        static Type defaultType = typeof(Template);

        static TemplatePyramid()
        {
            specificTemplateClasses = new Dictionary<string, Type>();
        }

        internal static Template CreateNew(string classLabel)
        { 
            if (specificTemplateClasses.ContainsKey(classLabel))
                return (Template)Activator.CreateInstance(specificTemplateClasses[classLabel]);
            else
                return (Template)Activator.CreateInstance(defaultType);
        }

        /// <summary>
        /// Binds specific template to class label so the metadata can be loaded or generated (serialized)
        /// </summary>
        /// <typeparam name="T"> Specific template inherited from Template</typeparam>
        /// <param name="classLabel">assigned class label</param>
        public static void BindSpecificTemplateToClass<T>(string classLabel) where T : Template, new()
        { 
            specificTemplateClasses.Add(classLabel, typeof(T));
        }

    }
}
