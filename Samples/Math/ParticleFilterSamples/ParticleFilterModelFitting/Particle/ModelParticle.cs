#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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
using Accord.Extensions;
using Accord.Extensions.Statistics.Filters;
using Accord.Extensions.Imaging.Algorithms.LINE2D;

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

        public void Diffuse()
        {
            var id = (ModelParameters.ModelTypeIndex + rand.Next(ModelRepository.PrototypeCount)) % ModelRepository.PrototypeCount;
            ModelParameters.ModelTypeIndex = id;

            var angle = ModelParameters.Angle + rand.Next(-15, +15 + 1);
            ModelParameters.Angle = (short)angle;

            var scale = ModelParameters.Scale + rand.Next(-15, +15 + 1);
            ModelParameters.Scale = (short)scale;

            ModelParameters = ModelRepository.GetMostSimilarTo(ModelParameters);
            var template = ModelParameters.TryGetTemplate();
            this.updateTemplateData(template);
        }

        public static ModelParticle FromParameters(params double[] arr)
        {
            var mParams = new ModelParams(modelTypeIndex: (int)arr[0], 
                                         scale:         (short)arr[1], 
                                         angle:         (short)arr[2]);

            mParams = ModelRepository.GetMostSimilarTo(mParams);

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
