using Accord.Extensions;
using Accord.Extensions.Caching;
using Accord.Extensions.Imaging;
using Microsoft.VisualBasic.Devices;
using System;
using System.Threading;

namespace ElementCaching
{
    class Program
    {
        static void Main(string[] args)
        {
            runCacheTest();
        }

        static void runCacheTest()
        {
            ComputerInfo computerInfo = new ComputerInfo(); //reference to Microsoft.VisualBasic assembly.
            
            //construction
            var memCache = new LazyMemoryCache<int, Image<Gray, int>>
              (
               (currentSize) =>
                {
                    var occupied = computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory;
                    var occupiedPercentage = (float)occupied / computerInfo.TotalPhysicalMemory;
            
                    //WATCH OUT! You can get OutOfMemoryException although the RAM is not full:
                       //when creating fields with zeros I assume there are some OS optimizations like block sharing
                       //if you compile this as 32-bit (when it consumes 2 GiB it will throw OutOfMemoryException)
                    if (occupiedPercentage > 0.45)
                       return true;
            
                    return false;
                },
                
                (img) => (ulong)(img.Stride * img.Height),

                //set false to not to call GC.Collect() when an item is evicted => may fill more RAM than it has been set, but shorthens delays caused by GC
                forceCollectionOnRemoval: true
               );


            const int MAX_KEY = 100;
            //adding elements (you can also use stream as IEnumerable to populate cache)
            for (int key = 0; key <= MAX_KEY; key++)
            {
                memCache.AddOrUpdate(key, () =>
                {
                    //simulate getting image from a disc (slow operation)
                    var img = new Image<Gray, int>(640, 480);
                    img.SetValue(key);
                    Thread.Sleep(60);

                    return img;
                },
                (img) => { img.Dispose(); });
            }

            //accessing elements (run Task Manager to see memory allocation!)
            Random rand = new Random();
            while (!Console.KeyAvailable)
            {
                var key  = rand.Next(0, MAX_KEY + 1);
                ILazy<Image<Gray, int>> lazyVal;
                memCache.TryGetValue(key, out lazyVal);

                Console.ForegroundColor = lazyVal.IsValueCreated ? ConsoleColor.Green: ConsoleColor.Red;
                Image<Gray, int> val = null;
                var elapsed = Diagnostics.MeasureTime(() =>
                {
                    val = lazyVal.Value;
                });

                Console.Write("\r Accessing {0}. Access time: {1} ms.", key, elapsed);
            }

            //accessing elements (run Task Manager to see memory allocation!)
            /*foreach (var item in memCache)
            {
                 var lazyVal = item.Value;

                Console.WriteLine(lazyVal.Value);
                Console.WriteLine(memCache.HardFaults);
            }*/
        }
    }
}
