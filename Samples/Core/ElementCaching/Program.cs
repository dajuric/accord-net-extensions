using Accord.Extensions;
using Accord.Extensions.Caching;
using Accord.Extensions.Imaging;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ElementCaching
{
    class Program
    {
        static void Main(string[] args)
        {
            //testLRUCache();
            testLazyMemCache();
        }

        /// <summary>
        /// Creates and tests lazy memory cache where elements are constructed on demand and evicted based on LRU strategy only if the RAM usage above specified limits.
        /// </summary>
        static void testLazyMemCache()
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

        /// <summary>
        /// Creates a new instance of LRU cache where elements are evicted based on least frequently usage.
        /// </summary>
        static void testLRUCache()
        {
            ComputerInfo computerInfo = new ComputerInfo(); //reference to Microsoft.VisualBasic assembly.

            var lru = new LRUCache<int, Image<Gray, byte>>(
                                                    (currentSize) =>
                                                    {
                                                        var occupied = computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory;
                                                        var occupiedPercentage = (float)occupied / computerInfo.TotalPhysicalMemory;

                                                        if (occupiedPercentage > 0.85)
                                                            return true;

                                                        return false;
                                                    },
                                                    (img) => (ulong)(img.Stride * img.Height));

            lru.OnRemoveItem += lru_OnRemoveItem;

            /***************** add some elements ****************/
            var image = new Image<Gray, byte>(5 * 1024 * 1024, 1, 0);
            image.SetValue(5 % 256);
            lru.Add(1, image);

            image = new Image<Gray, byte>(5 * 1024 * 1024, 1, 0);
            image.SetValue(5 % 256);
            lru.Add(1, image);
            /***************** add some elements ****************/

            List<Image<Gray, byte>> a = new List<Image<Gray, byte>>();

            Random rand = new Random();

            int i = 0;
            while (i < 10000)
            {
                image = new Image<Gray, byte>(1024 * 1024, 1, 0);
                image.SetValue(i % 256);
                lru.Add(i, image);

                //Thread.Sleep(1);
                Console.WriteLine(computerInfo.AvailablePhysicalMemory / 1024 / 1024);
                i++;
            }

            //discover more properties and methods!
        }

        static void lru_OnRemoveItem(LRUCache<int, Image<Gray, byte>> sender, KeyValuePair<int, Image<Gray, byte>> item, bool userRequested)
        {
            //return;

            sender.Oldest.Value.Dispose();
            GC.Collect();

            Console.WriteLine("Kicked out!");
        }
    }
}
