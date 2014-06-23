using Accord.Extensions;
using Accord.Extensions.Statistics.Filters;
using LINE2D;
using System;
using Match = LINE2D.Match;

namespace ParticleFilterModelFitting
{
    public class ModelParticle: IParticle, ITemplate
    {
        //static NormalDistribution normalDistribution = new NormalDistribution(mean: 0, stdDev: 3);
        static Random rand = new Random();

        public ModelParams ModelParameters { get; set; }
        public WeakReference<Match> MetaDataRef { get; set; }

        public void Drift()    
        {}

        public void Difuse()
        {
            var id = (ModelParameters.ModelTypeIndex + rand.Next(ModelRepository.PrototypeCount)) % ModelRepository.PrototypeCount;
            ModelParameters.ModelTypeIndex = id;

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
                MetaDataRef = this.MetaDataRef
            };
        }

        #endregion

        #region ITemplate

        public Feature[] Features
        {
            get;
            private set;
        }

        public Size Size
        {
            get;
            private set;
        }

        public string ClassLabel
        {
            get;
            private set;
        }

        void ITemplate.Initialize(Feature[] features, Size size, string classLabel)
        {
            this.Features = features;
            this.Size = size;
            this.ClassLabel = classLabel;
        }

        #endregion

        public void OverwriteWith(ModelParticle particle)
        {
            this.ModelParameters = (ModelParams)particle.ModelParameters.Clone();
            this.Weight = particle.Weight;

            this.Size = particle.Size;
            this.Features = particle.Features;
            this.ClassLabel = particle.ClassLabel;

            this.MetaDataRef = particle.MetaDataRef;
        }
    }
}
