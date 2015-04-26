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

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Circular list example:"); Console.ResetColor();
            testCircularList(); //TODO: ispraviti ?

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Map example:");           Console.ResetColor();
            testMap();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("History example:");       Console.ResetColor();
            testHistory();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Pinned array example:");  Console.ResetColor();
            testPinnedArray();

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

        private static void testCircularList()
        {
            //the common problem when working with contours is to select previous and next point
            //the list can be the solution but the special boundary cases must be handled separately
            //that is the reason why the circular list structure is created

            List<int> numbers = new int[] { 2, 4, 6, 8, 10, 12, 14 }.ToList();

            var circularList = numbers.ToCircularList();

            Console.WriteLine("Last element: {0}", circularList[-1]);
            Console.WriteLine("ELement with large index: {0}", circularList[500]);

            var range = circularList.GetRange(5, circularList.Count - 1);
            foreach (var elem in range)
	        {
                Console.Write(elem + " ");
	        }

            List<int> someLst = circularList; //back to list
            Console.WriteLine();

            //discover more properties and extensions!!!
        }

        private static void testMap()
        {
            //.NET does not have structure that supports pair values where each part can be key (two keys and no value)
            //Map<,> provide this behaviour

            var map = new Map<int, string>();
            map.Add(42, "Hello_1");
            map.Add(52, "Hello_2");
            
            Console.WriteLine(map.Forward[42]);
            Console.WriteLine(map.Reverse["Hello_1"]);

            //write forward keys
            Console.WriteLine();
            Console.Write("Forward keys: ");
            foreach (var keyA in map.Forward)
            {
                Console.Write(keyA + " ");
            }

            Console.WriteLine();
            Console.Write("Reverse keys: ");
            foreach (var keyB in map.Reverse)
            {
                Console.Write(keyB + " ");
            }
            Console.WriteLine();

            //discover more properties and extensions!!!
        }

        private static void testHistory()
        {
            //to log a history a list could do the job. 
            //However if the user wants to remember only few elements (e.g. during object tracking) then History<> is the easiest to use.

            History<int> hist = new History<int>(maxNumOfElems: 5);

            hist.Add(1); hist.Add(2); hist.Add(3); hist.Add(4); hist.Add(5); hist.Add(6);

            foreach (var elem in hist)
            {
                Console.Write(elem + " ");
            }
            Console.WriteLine();

            //discover more properties and extensions!!!
        }

        private unsafe static void testPinnedArray()
        { 
            //to enable fast access to the elements of the array (without bound checking) an PinnedArray<> is created.
            //you can emulate it by using GCHandle, but this is more convenient way :)

            int[] arr = new int[] { 1, 2, 3, 4, 5 };

            var pinnedArr = new PinnedArray<int>(arr);

            var data = (int*)pinnedArr.Data;
            data[1] = 500;

            pinnedArr.Dispose();

            Console.WriteLine("Changed value: " + arr[1]);

            //discover more properties and extensions!!!
        }
    }
}
