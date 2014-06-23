using Accord.Extensions;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Statistics.Filters;
using Accord.Statistics.Distributions.Univariate;
using System;
using System.Collections.Generic;
using System.Linq;
using PointF = AForge.Point;

namespace JPDAF_Demo
{
    public class MotionParticle : IParticle<ConstantVelocity2DModel>
    {
        static double[,] transitionMatrix = ConstantVelocity2DModel.GetTransitionMatrix();
        static NormalDistribution normalDistribution = new NormalDistribution();

        public double Weight { get; set; }
        public ConstantVelocity2DModel State { get; set; }

        public void Drift()
        {
            this.State = ConstantVelocity2DModel.Evaluate(this.State, transitionMatrix);
        }

        public void Difuse()
        {
            this.State.Velocity = new PointF
            {
                X = this.State.Velocity.X + 1f * (float)normalDistribution.Generate(),
                Y = this.State.Velocity.Y + 1f * (float)normalDistribution.Generate(),
            };
        }

        object ICloneable.Clone()
        {
            return new MotionParticle
            {
                State = (ConstantVelocity2DModel)this.State.Clone(),
                Weight = this.Weight
            };
        }
    }

    public static class MotionParticleCollectionExtensions
    {
        public static RectangleF BoundingBox(this IEnumerable<MotionParticle> particles)
        {
            return particles.Select(x => x.State.Position).BoundingRect();
        }
    }
}
