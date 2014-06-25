using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Accord.Extensions
{
    /// <summary>
    /// Provides functions for natural string comparison.
    /// </summary>
    public class NaturalSortComparer: IComparer<string>
    {
        private bool isAscending;

        /// <summary>
        /// Creates a new instance of <see cref="NaturalSortComparer"/>.
        /// </summary>
        /// <param name="inAscendingOrder">Sorts in ascending order, otherwise descending.</param>
        public NaturalSortComparer(bool inAscendingOrder = true)
        {
            this.isAscending = inAscendingOrder;
        }

        #region IComparer<string> Members

        /// <summary>
        /// Compares two strings.
        /// </summary>
        /// <param name="x">First string.</param>
        /// <param name="y">Second string.</param>
        /// <returns>0 - the same objects, -1, +1 otherwise depending whether the first string preceds the second one or not.</returns>
        public int Compare(string x, string y)
        { 
            throw new NotImplementedException();
        }

        #endregion

        #region IComparer<string> Members

        int IComparer<string>.Compare(string x, string y)
        {
            if (x == y)
                return 0;

            string[] x1, y1;

            if (!table.TryGetValue(x, out x1))
            {
                x1 = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
                table.Add(x, x1);
            }

            if (!table.TryGetValue(y, out y1))
            {
                y1 = Regex.Split(y.Replace(" ", ""), "([0-9]+)");
                table.Add(y, y1);
            }

            int returnVal;

            for (int i = 0; i < x1.Length && i < y1.Length; i++)
            {
                if (x1[i] != y1[i])
                {
                    returnVal = PartCompare(x1[i], y1[i]);
                    return isAscending ? returnVal : -returnVal;
                }
            }

            if (y1.Length > x1.Length)
            {
                returnVal = 1;
            }
            else if (x1.Length > y1.Length)
            {
                returnVal = -1;
            }
            else
            {
                returnVal = 0;
            }

            return isAscending ? returnVal : -returnVal;
        }

        private static int PartCompare(string left, string right)
        {
            int x, y;
            if (!int.TryParse(left, out x))
                return left.CompareTo(right);

            if (!int.TryParse(right, out y))
                return left.CompareTo(right);

            return x.CompareTo(y);
        }

        #endregion

        private Dictionary<string, string[]> table = new Dictionary<string, string[]>();
    }
}
