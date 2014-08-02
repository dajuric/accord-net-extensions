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
using System.Threading;
using Accord.Extensions;
using Accord.Extensions.Caching;
using Accord.Extensions.Imaging;
using Microsoft.VisualBasic.Devices;

namespace ElementCaching
{
    class Program
    {
        static void Main(string[] args)
        {
            if (IntPtr.Size == 4)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Warning: the application is 32-bit which may cause OutOfMemoryException due to 2GiB limit.");
                Console.ResetColor();
            }

            //testLRUCache();  //uncomment to run
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

                //set false to not to call GC.Collect() when an item is evicted => may fill more RAM than it has been set, but shortens delays caused by GC
                forceCollectionOnRemoval: true
               );

            Console.WriteLine("Filling lazy cache (with constructors)...");
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
            Console.WriteLine("Accessing elements (run Task Manager to see memory allocation!):");
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
            sender.Oldest.Value.Dispose();
            GC.Collect();

            Console.WriteLine("Kicked out!");
        }
    }
}
