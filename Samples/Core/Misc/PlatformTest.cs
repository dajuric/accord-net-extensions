using Accord.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc
{
    class PlatformTest
    {
        public static void Test()
        {
            //in order to use some unmanaged library that has platform versions and is put into specific dir call the following (before the first call to the lib):
            Console.WriteLine(@"Adding specific dir for unmanged dll loading.... (see \UnamangedLibraries\Readme.txt)");
            Platform.AddDllSearchPath(); //or with user-specified dir

            Console.WriteLine("I am running: {0}", Platform.RunningPlatform);
        }

        
    }
}
