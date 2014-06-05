using System;
using PointF = AForge.Point;

namespace Accord.Extensions.Statistics.Filters
{
    [Obsolete] //not used: use for motion models
    public interface I2DConstantVelocityModel
    {
        PointF Position { get; set; }
        PointF Velocity { get; set; }

        double[] ToArray();

        I2DConstantVelocityModel FromArray(double[] arr);
    }
}
