using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accord.Extensions;
using System.IO;

namespace Misc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n********** Path extensions ************");
            PathExtensionsTest.Test();

            Console.WriteLine("\n********** Platform ************");
            PlatformTest.Test();    
        }
    }
}
