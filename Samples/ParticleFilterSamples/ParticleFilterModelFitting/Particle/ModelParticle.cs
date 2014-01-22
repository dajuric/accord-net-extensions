using Accord.Math.Geometry;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Filters;
using LINE2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointF = AForge.Point;
using Match = LINE2D.Match;
using Accord.Imaging;
using Range = AForge.IntRange;
using System.Text.RegularExpressions;

namespace ParticleFilterModelFitting
{
    public class ModelParticle: IParticle, ITemplate
    {
        //static NormalDistribution normalDistribution = new NormalDistribution(mean: 0, stdDev: 3);
        static Random rand = new Random();

        public ModelParams ModelParameters { get; set; }
        public Match MetaData { get; set; }

        public void Drift()    
        {}

        public void Difuse()
        {
            var angle = ModelParameters.Angle + rand.Next(-15, +15 + 1);
            ModelParameters.Angle = (short)angle;

            var scale = ModelParameters.Scale + rand.Next(-15, +15 + 1);
            ModelParameters.Scale = (short)scale;

            ModelParameters = ModelRepository.GetClosestTo(ModelParameters);
            var template = ModelParameters.TryGetTemplate();
            this.updateTemplateData(template);
        }

        public static ModelParticle FromParameters(params double[] arr)
        {
            var mParams = new ModelParams(modelTypeIndex: (int)arr[0], 
                                         scale:         (short)arr[1], 
                                         angle:         (short)arr[2]);

            mParams = ModelRepository.GetClosestTo(mParams);

            var p = new ModelParticle
            {
                ModelParameters = mParams,
                Weight = 0
            };

            var template = mParams.TryGetTemplate();
            p.updateTemplateData(template);

            return p;
        }

        private void updateTemplateData(ITemplate template)
        {
            this.Size = template.Size;
            this.Features = template.Features;
            this.ClassLabel = template.ClassLabel;
        }

        public override bool Equals(object obj)
        {
            var p = obj as ModelParticle;
            if (p == null)
                return false;

            return p.ModelParameters.Equals(this.ModelParameters);
        }

        public override int GetHashCode()
        {
            return this.ModelParameters.GetHashCode();
        }

        #region IParticle Interface

        public double Weight { get; set; }
       
        public object Clone()
        {
            return new ModelParticle
            {
                //user defined
                ModelParameters = (ModelParams)this.ModelParameters.Clone(),
                //IParticle
                Weight = this.Weight,
                //ITemplate
                Size = this.Size,
                Features = this.Features,
                ClassLabel = this.ClassLabel,
                //meta-data
                MetaData = this.MetaData
            };
        }

        #endregion

        #region ITemplate

        public Feature[] Features
        {
            get;
            private set;
        }

        public System.Drawing.Size Size
        {
            get;
            private set;
        }

        public string ClassLabel
        {
            get;
            private set;
        }

        void ITemplate.Initialize(Feature[] features, System.Drawing.Size size, string classLabel)
        {
            this.Features = features;
            this.Size = size;
            this.ClassLabel = classLabel;
        }

        #endregion
    }
}
