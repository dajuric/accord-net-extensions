
namespace Accord.Extensions.Math
{

    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides matrix extension methods for 2D arrays.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// For a given matrix returns number of rows and columns in the form [rows, columns]
        /// </summary>
        public static int[] GetSize(this double[,] matrix)
        {
            return new int[] { matrix.GetLength(0), matrix.GetLength(1) };
        }

        /// <summary>
        /// For a given matrix returns number of columns.
        /// </summary>
        public static int ColumnCount(this double[,] matrix)
        {
            return matrix.GetLength(1);
        }

        /// <summary>
        /// For a given matrix returns number of rows.
        /// </summary>
        public static int RowCount(this double[,] matrix)
        {
            return matrix.GetLength(0);
        }

        /// <summary>
        /// For a given matrix returns if two matrices can be multiplied. 
        /// </summary>
        /// <param name="leftMatrix">Left matrix</param>
        /// <param name="rightMatrix">Right matrix</param>
        /// <returns>Wheather can be multiplied or not.</returns>
        public static bool IsMultipliableBy(this double[,] leftMatrix, double[,] rightMatrix)
        {
            if (leftMatrix.ColumnCount() == rightMatrix.RowCount())
                return true;
            else
                return false;
        }
    }
}
