using Accord.Math;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// Contains methods for geometric transformations.
    /// </summary>
    public static class Transforms2D
    {
        #region Rotation

        /// <summary>
        /// Gets the 3x3 rotation matrix.
        /// </summary>
        /// <param name="angleRad">Rotation angle in radians.</param>
        /// <returns>Rotation matrix.</returns>
        public static float[,] Rotation(float angleRad)
        {
            var cos = (float)System.Math.Cos(angleRad);
            var sin = (float)System.Math.Sin(angleRad);

            return new float[,] 
            {
                {cos, -sin, 0},
                {sin, cos,  0},
                {0,   0,    1}
            };
        }

        #endregion

        #region Translation

        /// <summary>
        /// Gets the 3x3 translation matrix.
        /// </summary>
        /// <param name="x">Horizontal offset.</param>
        /// <param name="y">Vertical offset.</param>
        /// <returns>Translation matrix.</returns>
        public static float[,] Translation(float x, float y)
        {
            return new float[,] 
            {
                {1, 0, x},
                {0, 1, y},
                {0, 0, 1}
            };
        }

        #endregion

        #region Scale

        /// <summary>
        /// Gets the 3x3 scale matrix.
        /// </summary>
        /// <param name="x">Horizontal scale.</param>
        /// <param name="y">Vertical scale.</param>
        /// <param name="z">Depth scale.</param>
        /// <returns>Scale matrix.</returns>
        public static float[,] Scale(float x, float y, float z = 1)
        {
            return new float[,] 
            {
                {x, 0, 0},
                {0, y, 0},
                {0, 0, z}
            };
        }

        #endregion

        /// <summary>
        /// Multiplies transforms starting from the last one to the first one (stack).
        /// Transforms are given by priority.
        /// </summary>
        /// <param name="transforms">Transfrom matrices.</param>
        /// <returns>Combined transfrom matrix.</returns>
        public static float[,] Combine(params float[][,] transforms)
        {
            float[,] res = Matrix.Identity(3).ToSingle();

            for (int i = transforms.Length - 1; i >= 0; i--)
            {
                res = res.Multiply(transforms[i]);
            }

            return res;
        }

    }

}
