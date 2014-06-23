using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RT
{
    /// <summary>
    /// pseudo-random number generator is the multiply-with-carry method invented by George Marsaglia. 
    /// It is computationally fast and has good (albeit not cryptographically strong) randomness properties.
    /// For details see: <see cref="http://en.wikipedia.org/wiki/Random_number_generation?action=render"/>.
    /// <para>This class is thread-safe.</para>
    /// </summary>
    public class MyRandom
    {
        uint m_z, m_w;

        public MyRandom()
            :this((uint)DateTime.Now.Ticks)
        { }

        public MyRandom(uint seed)
        {
            Thread.BeginCriticalRegion();

            m_z = 0x12345678 * seed;
            if (m_z == 0)
                m_z = 0xAAAA;

            m_w = 0x000fffff * seed;
            if (m_w == 0)
                m_w = 0xBBBB;

            Thread.EndCriticalRegion();
        }

        public UInt32 Next()
        {
            Thread.BeginCriticalRegion();

            m_z = 36969 * (m_z & 65535) + (m_z >> 16);
            m_w = 18000 * (m_w & 65535) + (m_w >> 16);
            var rand = (m_z << 16) + m_w;  /* 32-bit result */

            Thread.EndCriticalRegion();

            return rand;
        }
    }
}
