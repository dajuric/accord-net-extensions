using Accord.Math.Geometry;
using Accord.Statistics.Distributions.Univariate;
using LINE2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointF = AForge.Point;

namespace ParticleFilterModelFitting
{
    public class ParticleState: ICloneable, ITemplate
    {
        static NormalDistribution normalDistribution = new NormalDistribution(mean: 0, stdDev: 3);
        static Random rand = new Random();

        public float Scale { get; set; }
        public int RotationZ { get; set; } //[0..180]

        public static void Drift(ref ParticleState state)
        {
            //we do not have velocity (or something else), so nothing :)
        }

        public static void Difuse(ref ParticleState state)
        {
            return;

            state.Scale = Math.Max(80, state.Scale + 5.5f * (float)(rand.NextDouble() - 0.5) * 2 /*(float)normalDistribution.Generate()*/);
            //state.RotationZ = state.RotationZ + (int)(5 * normalDistribution.Generate());

            var template = OpenHandTemplate.Create(state.Scale, state.Scale/*, state.RotationZ*/, 10);
            state.Features = template.Features.ToArray();
            state.Size = System.Drawing.Size.Round(template.BoundingBox.Size);
            state.ClassLabel = "";
        }

        public object Clone()
        {
            return new ParticleState
            {
                Scale = this.Scale,
                RotationZ = this.RotationZ,
                Features = (Feature[])this.Features.Clone(), //needs to be deep cloning
                Size = this.Size,
                MetaData = this.MetaData,
                ClassLabel = ""
            };
        }

        public static ParticleState FromArray(params double[] arr)
        {
            var ps =  new ParticleState
            {
                Scale = (float)arr[0],
                RotationZ = (int)arr[1] + 10
            };

            var template = OpenHandTemplate.Create(ps.Scale, ps.Scale, ps.RotationZ);
            ps.Features = template.Features.ToArray();
            ps.Size = System.Drawing.Size.Round(template.BoundingBox.Size);
            ps.ClassLabel = "";

            return ps;
        }

        public override bool Equals(object obj)
        {
             var state = obj as ParticleState;

            if (state == null)
                return false;

            if (state.Scale == this.Scale && state.RotationZ == this.RotationZ)
            {
                return true;
            }

            return false;
        }

        public GroupMatch<Match> MetaData { get; set; }

        #region LINE 2D template Members

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

        public void Initialize(Feature[] features, System.Drawing.Size size, string classLabel) //needed for ITemplate serialization
        {
            this.Features = features;
            this.Size = size;
            this.ClassLabel = classLabel;
        }

        #endregion

        #region ISerializable

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {}

        public void WriteXml(System.Xml.XmlWriter writer)
        { }

        #endregion
    }
}
