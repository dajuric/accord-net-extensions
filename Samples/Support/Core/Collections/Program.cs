#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
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
using Accord.Extensions;

namespace Collections
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Sparse matrix example:"); Console.ResetColor();
            testSparseMatrix();

            Console.ReadLine();
        }

        struct MatrixElement<TKey, TVal>
        {
            public TKey Row;
            public TKey Column;
            public TVal Value;

            public static MatrixElement<TKey, TVal> Create(TKey row, TKey column, TVal value)
            {
                return new MatrixElement<TKey, TVal>
                {
                    Row = row,
                    Column = column,
                    Value = value
                };
            }
        }

        private static void testSparseMatrix()
        {
            //sparse matrix structure is implemented as an extension method for IDictionary<Pair<TKey>, TValue>
            //the structure is used for methods that operates on graph (see Accord.Extension.Math) but it can be useful for many other problems
            //(e.g. when dense matrix takes a large amount of memory, but does not contain many elements)

            var elements = new[] 
            {
                MatrixElement<int, string>.Create(row:   0, column:   5, value:"0_5"    ),
                MatrixElement<int, string>.Create(row:  55, column: 243, value:"55_243" ),
                MatrixElement<int, string>.Create(row: -22, column:  10, value:"-22_10" )
            };

            var sparseMat = elements.ToSparseMatrix(x => x.Row, x => x.Column, x => x.Value);
            sparseMat.AddOrUpdate(0, 5, "0_5_changed");
            bool isRemoved = sparseMat.Remove(55, 243);

            string value;
            bool exist = sparseMat.TryGetValue(0, 5, out value);
            Console.WriteLine("<0, 5> => {0}", value);

            exist = sparseMat.TryGetValue(1, 1, out value);
            Console.WriteLine("<1, 1> => {0}", value);

            Console.WriteLine();
            Console.WriteLine("All values:");
            foreach (string val in sparseMat.AsEnumerable())
            {
                Console.Write(val + " ");
            }
            Console.WriteLine();

            //discover more extension methods!!!
        }
    }
}
