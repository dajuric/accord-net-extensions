using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accord.Extensions.Math
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides additional math functions.
    /// </summary>
    public static class MathFunctions
    {
        #region ATan2 Approximation

        /// <summary>
        /// Calculates an octant and brings x and y to the first octant
        /// </summary>
        /// <returns> Octant [0..7]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateOctant(ref int x, ref int y)
        { 
            int o = 0, t;

            if (y < 0)
            {
                x = -x;
                y = -y;
                o += 4;
            }
            if (x <= 0)
            {
                t = x; x = y;
                y = -t;
                o += 2;
            }
            if (x <= y)
            {
                t = y - x; x = x + y;
                y = t;
                o += 1;
            }

            return o;
        }

        const int PI_DEG = 180;

        const int Y_MUL_CONST = 64;
        public static float DEG_RESOULTION = (float)(PI_DEG / 4) / Y_MUL_CONST;


        static int[] angleTable;

        static MathFunctions()
        {
            angleTable = CalculateAngleTable();
        }

        private static int[] CalculateAngleTable()
        {
            int[] angleTable = new int[Y_MUL_CONST + 1];

            for (int i = 0; i < angleTable.Length; i++)
            {
                double angle = System.Math.Atan((double)i / Y_MUL_CONST) * 180 / System.Math.PI;
                angleTable[i] = (int)System.Math.Round(angle);
            }

            return angleTable;
        }

        //TODO - medium: check maxium error!
        /// <summary>
        /// Approximates Atan2 function. Maximum error is 90 / Y_MUL_CONST (1.4 degrees). 
        /// </summary>
        /// <param name="dY">Vertical offset.</param>
        /// <param name="dX">Horizontal offset.</param>
        /// <returns>Angle in degrees.</returns>
        public static int Atan2Aprox(int dY, int dX)
        {
            if (dY == 0) return (dX >= 0 ? 0 : PI_DEG);

            int octant = CalculateOctant(ref dX, ref dY);

            int tableIdx = dY * Y_MUL_CONST / dX;
            int degFirstOctant = angleTable[tableIdx];

            return (PI_DEG / 4) * octant + degFirstOctant;
        }

        #endregion

        #region Sqrt Approximation

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public int tmp;
        }

        /// <summary>
        /// Approximates Sqrt function.
        /// see: http://blog.wouldbetheologian.com/2011/11/fast-approximate-sqrt-method-in-c.html
        /// </summary>
        /// <param name="z">Input number</param>
        /// <returns>Square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float z)
        {
            if (z == 0) return 0;

            FloatIntUnion u;
            u.tmp = 0;
            float xhalf = 0.5f * z;
            u.f = z;
            u.tmp = 0x5f375a86 - (u.tmp >> 1);
            u.f = u.f * (1.5f - xhalf * u.f * u.f);
            return u.f * z;
        }

        #endregion
    }
}
