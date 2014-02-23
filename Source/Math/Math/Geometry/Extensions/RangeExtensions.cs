using System.Collections.Generic;
using Range = AForge.IntRange;

namespace Accord.Extensions.Math.Geometry
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides extension methods for <see cref="IntRange"/>.
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// Determines whether the specified values are inside the range.
        /// </summary>
        /// <param name="range">Range.</param>
        /// <param name="values">Values.</param>
        /// <returns>Collection where each element is set to true if the corresponding value is in the range, otherwise is set to false.</returns>
        public static IEnumerable<bool> IsInside(this Range range, IEnumerable<int> values)
        {
            foreach (var val in values)
            {
                yield return range.IsInside(val);
            }
        }
    }
}
