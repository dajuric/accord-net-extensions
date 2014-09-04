#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Math
{

    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides matrix extension methods for 2D arrays.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Creates new array which has the same size as the source.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="matrix">Source matrix.</param>
        /// <returns>Blank array of the same size as source array.</returns>
        public static T[,] CopyBlank<T>(this T[,] matrix)
        {
            return new T[matrix.GetLength(0), matrix.GetLength(1)];
        }

        /// <summary>
        /// For a given matrix returns number of rows and columns in the form [rows, columns]
        /// </summary>
        public static int[] GetSize<T>(this T[,] matrix)
        {
            return new int[] { matrix.GetLength(0), matrix.GetLength(1) };
        }

        /// <summary>
        /// For a given matrix returns number of columns.
        /// </summary>
        public static int ColumnCount<T>(this T[,] matrix)
        {
            return matrix.GetLength(1);
        }

        /// <summary>
        /// For a given matrix returns number of rows.
        /// </summary>
        public static int RowCount<T>(this T[,] matrix)
        {
            return matrix.GetLength(0);
        }

        /// <summary>
        /// Checks whether the matrix has number of elements greater than zero.
        /// </summary>
        /// <returns>True if the matrix is empty, false otherwise.</returns>
        public static bool IsEmpty<T>(this T[,] matrix)
        {
            if (matrix.RowCount() == 0 || matrix.ColumnCount() == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if the matrix is empty and returns default value if the matrix is empty.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <param name="defaultValue">Default value to return if the matrix is empty.</param>
        /// <returns>Source matrix if the matrix is not empty, default value otherwise.</returns>
        public static T[,] DefaultIfEmpty<T>(this T[,] matrix, T[,] defaultValue)
        {
            if (matrix.IsEmpty())
                return defaultValue;
            else
                return matrix;
        }

        /// <summary>
        /// Checks if the matrix is empty and returns empty 2D array if the matrix is empty.
        /// </summary>
        /// <returns>Source matrix if the matrix is not empty, empty array otherwise.</returns>
        public static T[,] DefaultIfEmpty<T>(this T[,] matrix)
        {
            return matrix.DefaultIfEmpty(new T[0, 0]);
        }

        /// <summary>
        /// Gets the elements specified by indices.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="matrix">Matrix.</param>
        /// <param name="indices">Indices of requested elements (collection of (row col) pairs).</param>
        /// <returns>Elements specified by indices</returns>
        public static IEnumerable<T> GetAt<T>(this T[,] matrix, int[][] indices)
        {
            foreach (var idx in indices)
            {
                yield return matrix[idx[0], idx[1]];
            }
        }

        /// <summary>
        /// Sets the elements specified by indices.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="matrix">Matrix.</param>
        /// <param name="indices">Indices of requested elements (collection of (row col) pairs).</param>
        /// <param name="setter">
        /// Element setter.
        /// params: value of the element
        /// returns: new value
        /// </param>
        public static void SetAt<T>(this T[,] matrix, int[][] indices, Func<T, T> setter)
        {
            foreach (var idx in indices)
            {
                var row = idx[0];
                var col = idx[1];

                matrix[row, col] = setter(matrix[row, col]);
            }
        }

        /// <summary>
        /// For a given matrix returns if two matrices can be multiplied. 
        /// </summary>
        /// <param name="leftMatrix">Left matrix</param>
        /// <param name="rightMatrix">Right matrix</param>
        /// <returns>Whether can be multiplied or not.</returns>
        public static bool IsMultipliableBy(this double[,] leftMatrix, double[,] rightMatrix)
        {
            if (leftMatrix.ColumnCount() == rightMatrix.RowCount())
                return true;
            else
                return false;
        }

        //this method is taken from Accord.NET
        /// <summary>
        /// Gets the mean value for each column.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <returns>The mean value for each matrix column.</returns>
        public static double[] Mean(this double[,] matrix)
        {  
            double[] mean = new double[matrix.GetLength(1)];
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double N = matrix.GetLength(0);

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                    mean[j] += matrix[i, j];
                mean[j] /= N;
            }

            return mean;
        }

        //this method is taken from Accord.NET
        /// <summary>
        /// Gets the covariance of the matrix.
        /// Each matrix row represents a state. The result has the size of: [state x state].
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <param name="means">Mean value for each column.</param>
        /// <returns>The covariance matrix of size: [state x state].</returns>
        public static double[,] Covariance(this double[,] matrix, double[] means)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double divisor = rows - 1;

            double[,] cov = new double[cols, cols];
            for (int i = 0; i < cols; i++)
            {
                for (int j = i; j < cols; j++)
                {
                    double s = 0.0;
                    for (int k = 0; k < rows; k++)
                        s += (matrix[k, j] - means[j]) * (matrix[k, i] - means[i]);
                    s /= divisor;
                    cov[i, j] = s;
                    cov[j, i] = s;
                }
            }

            return cov;
        }

        /// <summary>
        /// Gets the covariance of the matrix (means are calculated internally) See function overloads.
        /// Each matrix row represents a state. The result has the size of: [state x state].
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <returns>The covariance matrix of size: [state x state].</returns>
        public static double[,] Covariance(this double[,] matrix)
        {
            var colMeans = matrix.Mean();
            return matrix.Covariance(colMeans);
        }

        /// <summary>
        /// Converts collection of matrix rows to 2D matrix.
        /// Each row must have the same length.
        /// </summary>
        /// <param name="matrixRows">Collection of matrix rows.</param>
        /// <returns>Matrix.</returns>
        public static T[,] ToMatrix<T>(this IEnumerable<T[]> matrixRows)
        {
            var nCols = matrixRows.First().Length;
            var nRows = matrixRows.Count();

            var mat = new T[nRows, nCols];

            for (int r = 0; r < nRows; r++)
            {
                var matRow = matrixRows.ElementAt(r);

                for (int c = 0; c < nCols; c++)
                {
                    mat[r, c] = matRow[c];
                }
            }

            return mat;
        }

        /// <summary>
        /// Creates a diagonal matrix from a supplied vector.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="vector">Vector to convert to diagonal matrix.</param>
        /// <returns>Diagonal matrix.</returns>
        public static T[,] ToDiagonalMatrix<T>(this T[] vector)
        {
            var nDim = vector.Length;
            var mat = new T[nDim, nDim];

            for (int i = 0; i < nDim; i++)
            {
                mat[i, i] = vector[i];
            }

            return mat;
        }

        /// <summary>
        /// Multiplies two vector (column  vector * row vector) resulting in 2D matrix. 
        /// </summary>
        /// <param name="columnVector">Column vector [n x 1].</param>
        /// <param name="rowVector">Row vector [1 x n].</param>
        /// <returns>Multiplication result.</returns>
        public static double[,] Multiply(this double[] columnVector, double[] rowVector)
        {
            if (columnVector.Length != rowVector.Length)
                throw new ArgumentException("Vector lengths must match!");

            int dim = columnVector.Length;
            var rez = new double[dim, dim];

            for (int r = 0; r < dim; r++)
            {
                for (int c = 0; c < dim; c++)
                {
                    rez[r, c] = columnVector[r] * rowVector[c];
                }
            }

            return rez;
        }

        #region Jagged matrix

        /// <summary>
        /// Gets jagged matrix column elements.
        /// </summary>
        /// <exception cref="System.ArgumentException">Column index is out of range.</exception>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="jaggedMatrix">Jagged matrix.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <returns>Column elements.</returns>
        public static IEnumerable<T> GetColumn<T>(this IEnumerable<IList<T>> jaggedMatrix, int columnIndex)
        {
            foreach (var matRow in jaggedMatrix)
            {
                yield return matRow[columnIndex];
            }
        }

        /// <summary>
        /// Converts vector to one-column jagged matrix representation.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="vector">Vector.</param>
        /// <returns>One-column jagged matrix.</returns>
        public static T[][] ToJaggedMatrix<T>(this IList<T> vector)
        {
            var mat = new T[vector.Count][];

            for (int i = 0; i < vector.Count; i++)
            {
                mat[i] = new T[] { vector[i] };
            }

            return mat;
        }

        #endregion
    }
}
