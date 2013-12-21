using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Core
{
    public static class EnumerableMethods
    {
        public static IEnumerable<float> GetRange(float start, float end, float step)
        {
            for (float i = start; i < end; i += step)
            {
                yield return i;
            }
        }
    }
}
