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
    public class ModelParams: ICloneable
    {
        public ModelParams(int modelTypeIndex, short scale, short angle)
        {
            this.ModelTypeIndex = modelTypeIndex;
            this.Scale = scale;
            this.Angle = angle;
        }

        public int ModelTypeIndex { get; set; }
        public short Scale { get; set; }
        /// <summary>
        /// Gets or sets angle offset, not the absoulte value.
        /// </summary>
        public short Angle { get; set; }

        public ITemplate TryGetTemplate()
        {
            ITemplate val;
            ModelRepository.Repository.TryGetValue(this, out val);
            return val;
        }

        public override bool Equals(object obj)
        {
            if (obj is ModelParams == false)
                return false;

            var m = obj as ModelParams;

            if (this.ModelTypeIndex == m.ModelTypeIndex &&
                this.Angle == m.Angle &&
                this.Scale == m.Scale)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (this.Angle << (sizeof(int) / 2)) | (ushort)this.Scale | this.ModelTypeIndex; //can be better
        }

        public object Clone()
        {
            return new ModelParams(this.ModelTypeIndex, this.Scale, this.Angle);
        }
    }

    public static class ModelRepository
    {
        private static short[][] sortedScales;
        private static short[][] sortedAngles;

        public static Dictionary<ModelParams, ITemplate> Repository { get; private set; }

        public static void Initialize(Dictionary<ModelParams, ITemplate> repository)
        {
            Repository = repository;

            var modelGroups = from m in repository.Keys
                              group m by m.ModelTypeIndex into mGroup
                              orderby mGroup.Key
                              select mGroup;

            sortedScales = (
                              from mGroup in modelGroups
                                select (
                                       from m in mGroup
                                       orderby m.Scale ascending
                                       select m.Scale
                                       )
                                       .Distinct().ToArray()
                            )
                            .ToArray();   

            sortedAngles = (
                              from mGroup in modelGroups
                               select (
                                       from m in mGroup
                                       orderby m.Angle ascending
                                       select m.Angle
                                      )
                                      .Distinct().ToArray()
                            )
                            .ToArray();


            ScaleRange = new Range(sortedScales.Select(x => x.First()).Min(), sortedScales.Select(x => x.Last()).Max());
            AngleRange = new Range(sortedAngles.Select(x => x.First()).Min(), sortedAngles.Select(x => x.Last()).Max());
        }

        public static ModelParams GetClosestTo(ModelParams model)
        {
            Func<short, short, float> scaleDistFunc = (a, b) => (float)Math.Abs(a - b);
            Func<short, short, float> angleDistFunc = (a, b) => (float)Math.Abs(a - b);//(float)Angle.DistanceDeg(a, b);

            var templateIdx = model.ModelTypeIndex;
            var closestScale = getClosestValue(sortedScales[templateIdx], model.Scale, scaleDistFunc);
            var closestAngle = getClosestValue(sortedAngles[templateIdx], model.Angle, angleDistFunc);
        
            return new ModelParams(templateIdx, closestScale, closestAngle);
        }

        private static T getClosestValue<T>(T[] sortedCollection, T param, Func<T, T, float> distanceFunc)
        {
            int index = Array.BinarySearch(sortedCollection, param);

            if (index < 0)
            {
                // If the index is negative, it represents the bitwise 
                // complement of the next larger element in the array. 
                index = ~index;
                var indexMax = Math.Max(0, Math.Min(index, sortedCollection.Length - 1));
                var indexMin = Math.Max(0, Math.Min(index - 1, sortedCollection.Length - 1));

                if (distanceFunc(sortedCollection[indexMin], param) < distanceFunc(sortedCollection[indexMax], param))
                {
                    return sortedCollection[indexMin];
                }
                else
                {
                    return sortedCollection[indexMax];
                }
            }

            return sortedCollection[index];
        }

        public static Range ScaleRange { get; private set; }
        public static Range AngleRange { get; private set; }
    }

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
                ClassLabel = this.ClassLabel
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
