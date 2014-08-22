using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions
{
    /// <summary>
    /// Contains extension methods for projecting a collection.
    /// <para>All methods can be used as extensions.</para>
    /// </summary>
    public static class SelectExtensions
    {
        /// <summary>
        /// Projects each element of a sequence into a new form by giving the progress expressed as percentage.
        /// </summary>
        /// <typeparam name="TSrc">Source type.</typeparam>
        /// <typeparam name="TDest">Projected type.</typeparam>
        /// <param name="collection">Collection.</param>
        /// <param name="selector">Selector function.</param>
        /// <param name="percentageAction">Executed prior every selection. Parameter represents percentage in range [0..1].</param>
        /// <returns>Projected collection.</returns>
        public static IEnumerable<TDest> Select<TSrc, TDest>(this IEnumerable<TSrc> collection, Func<TSrc, TDest> selector, Action<float> percentageAction)
        {
            var count = collection.Count();
            return collection.Select((item, index) => 
            {
                percentageAction((float)index / count);
                return selector(item);
            });              
        }
    }
}
