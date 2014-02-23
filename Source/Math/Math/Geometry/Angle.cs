
namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Provides methods for manipulating angle calculation.
    /// </summary>
    public static class Angle
    {
        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="angleRad">Angle in radians.</param>
        /// <param name="normalizeDegrees">Returns angle in degrees in rangle [0..359].</param>
        /// <returns>Angle in degrees.</returns>
        public static double ToDegrees(double angleRad, bool normalizeDegrees = true)
        {
            var deg = angleRad / System.Math.PI * 180;

            if(normalizeDegrees)
                deg = NormalizeDegrees(deg);
            
            return deg;
        }

        /// <summary>
        /// Calculates angle in degrees in range [0..360].
        /// </summary>
        /// <param name="angleDeg">Angle in degrees.</param>
        /// <returns>Angle in degrees in range [0..360].</returns>
        public static double NormalizeDegrees(double angleDeg)
        {
            angleDeg %= 360;

            if (angleDeg < 0)
                angleDeg += 360;

            return angleDeg;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="angleDeg">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        public static double ToRadians(double angleDeg)
        {
            return angleDeg * System.Math.PI / 180;
        }

        /// <summary>
        /// Caclulates absolute distance in degrees for two angles in degrees.
        /// </summary>
        /// <param name="angleDegA">First angle in degrees.</param>
        /// <param name="angleDegB">Second angle in degrees.</param>
        /// <returns>Returns absolute distance in degrees for two angles in degrees.</returns>
        public static double DistanceDeg(double angleDegA, double angleDegB)
        {
            var dist = angleDegB - angleDegA;
            dist = (dist + 180) % 360 - 180;
            return dist;
        }
    }
}
