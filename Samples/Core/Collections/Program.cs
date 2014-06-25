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
            testSparseMatrix();
            //testCircularList(); //TODO: ispraviti ?
            //testMap();
            //testHistory();
            //testPinnedArray();
        }

        struct MatElement<TKey, TVal>
        {
            public TKey Row;
            public TKey Col;
            public TVal Value;
        }

        private static void testSparseMatrix()
        {
            //sparse matrix structure is implemented as an extension method for Dictionary<,Dictionary<,>>
            //the structure is used for methods that operates on graph (see Accord.Extension.Math) but it can be useful for many other problems
            //(e.g. when dense matrix takes a large amount of memory, but does not contain many elements)

            var elements = new MatElement<int, string>[] 
            {
                new MatElement<int, string>{Row=0, Col=5, Value="0_5" },
                new MatElement<int, string>{Row=55, Col=243, Value="55_243" },
                new MatElement<int, string>{Row=-22, Col=10, Value="-22_10" }
            };

            var sparseMat = elements.ToMatrix(x => x.Row, x => x.Col, x => x.Value);
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

            //discover more extension methods!!!
        }

        private static void testCircularList()
        {
            //the common problem when working with contours is to select previous and next point
            //the list can be the solution but the special boder cases must be handeled separetly
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

            //discover more properties and extensions!!!
        }

        private static void testHistory()
        {
            //to log a history a list could serve. 
            //However if the user wants to remember only few elements (e.g. during object tracking) then History<> is the easiest to use.

            History<int> hist = new History<int>(maxNumOfElems: 5);

            hist.Add(1); hist.Add(2); hist.Add(3); hist.Add(4); hist.Add(5); hist.Add(6);

            foreach (var elem in hist)
            {
                Console.Write(elem + " ");
            }

            //discover more properties and extensions!!!
        }

        private unsafe static void testPinnedArray()
        { 
            //to enable fast access to the elements of the array (without bound checking) an PinnedArray<> is created.
            //you can emulate it with GCHandle, but I think it looks better this way :)

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
