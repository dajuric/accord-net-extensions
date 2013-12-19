using Accord.Statistics.Distributions.Univariate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointF = AForge.Point;

namespace ParticleFilterModelFitting
{
    public class ParticleState: ICloneable
    {
        static NormalDistribution normalDistribution = new NormalDistribution(mean: 0, stdDev: 10);

        public Template HandTemplate { get; set; }
        public PointF Position { get; set; }
        public float Scale { get; set; }
        public int RotationZ { get; set; } //[0..180]

        public static void Drift(ref ParticleState state)
        {
            //we do not have velocity (or something else), so nothing :)
        }

        public static void Difuse(ref ParticleState state)
        {
            state.Position = new PointF
            {
                X = state.Position.X + 1.5f * (float)normalDistribution.Generate(),
                Y = state.Position.Y + 1.5f * (float)normalDistribution.Generate(),
            };

            //state.Scale = state.Scale + 1.5f * (float)normalDistribution.Generate();

            //state.RotationZ = state.RotationZ + (int)(1 * normalDistribution.Generate());

            state.HandTemplate = Template.Create(state.Position.X, state.Position.Y, state.Scale, state.Scale, 0, 0, state.RotationZ);
        }

        public object Clone()
        {
            return new ParticleState
            {
                HandTemplate = Template.Create(Position.X, Position.Y, Scale, Scale, 0,0,RotationZ),//this.HandTemplate,
                Position = this.Position,
                Scale = this.Scale,
                RotationZ = this.RotationZ
            };
        }

        public static ParticleState FromArray(double[] arr)
        {
            var ps =  new ParticleState
            {
                Position = new PointF((float)arr[0], (float)arr[1]),
                Scale = (float)arr[2],
                RotationZ = (int)arr[3]
            };

            ps.HandTemplate = Template.Create(ps.Position.X, ps.Position.Y,
                                              ps.Scale, ps.Scale,
                                              0, 0, ps.RotationZ);

            return ps;
        }
    }
}
