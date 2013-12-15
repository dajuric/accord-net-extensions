using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Range = AForge.IntRange;

namespace Accord.Math.Geometry
{
    public static class RangeExtensions
    {
        public static IEnumerable<bool> IsInside(this Range range, IEnumerable<int> values)
        {
            foreach (var val in values)
            {
                yield return range.IsInside(val);
            }
        }
    }
}
