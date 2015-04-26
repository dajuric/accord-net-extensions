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

            testLazyMemCache();
        }

        /// <summary>
        /// Creates and tests lazy memory cache where elements are constructed on demand and evicted based on LRU strategy only if the RAM usage above specified limits.
        /// </summary>
        static void testLazyMemCache()
        {
            //********************* construction *********************************************************
            var memCache = new LazyMemoryCache<int, Bgr<byte>[,]>
              (
                //should we start evicting ?
                isCacheReachedCapacity,
                //get element size
                (img) => (ulong)(img.LongLength * img.ColorInfo().Size),
                //should we collect after each eviction ?
                forceCollectionOnRemoval: false
               );

            const int MAX_KEY = 100;
            //******************* adding elements *********************************************************
            populateCache(memCache, MAX_KEY + 1);

            //******************* accessing elements (run Task Manager to see memory allocation!) **************
            Console.WriteLine("Accessing elements (run Task Manager to see memory allocation!):");
            Random rand = new Random();
            while (!Console.KeyAvailable)
            {
                var key  = rand.Next(0, MAX_KEY + 1);
                ILazy<Bgr<byte>[,]> lazyVal;
                memCache.TryGetValue(key, out lazyVal);

                Console.ForegroundColor = lazyVal.IsValueCreated ? ConsoleColor.Green: ConsoleColor.Red;
                Bgr<byte>[,] val = null;
                var elapsed = Diagnostics.MeasureTime(() =>
                {
                    val = lazyVal.Value;
                });

                Console.Write("\r Accessing {0}. Access time: {1} ms.", key, elapsed);
            }
        }

        static ComputerInfo computerInfo = new ComputerInfo(); //reference to Microsoft.VisualBasic assembly.
        static bool isCacheReachedCapacity(ulong totalObjectSize)
        {
            var occupied = computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory;
            var occupiedPercentage = (float)occupied / computerInfo.TotalPhysicalMemory;
            
            //WATCH OUT! You can get OutOfMemoryException although the RAM is not full:
                //when creating fields with zeros I assume there are some OS optimizations like block sharing
                //if you compile this as 32-bit (when it consumes 2 GiB it will throw OutOfMemoryException)
            if (occupiedPercentage > 0.45)
                return true;
            
            return false;
        }

        static void populateCache(LazyMemoryCache<int, Bgr<byte>[,]> cache, int elementCount)
        {
            Console.WriteLine("Filling lazy cache (with constructors)...");
            //******************* adding elements *********************************************************
            for (int key = 0; key < elementCount; key++)
            {
                cache.AddOrUpdate(key, () =>
                {
                    //simulate getting image from a disc (slow operation)
                    var img = new Bgr<byte>[480, 640];
                    img.SetValue<Bgr<byte>>(new Bgr<byte>((byte)key, 0, 0));
                    Thread.Sleep(60);

                    return img;
                },
                    //we do not have destructor
                (img) => { });
            }
        }
    }
}
