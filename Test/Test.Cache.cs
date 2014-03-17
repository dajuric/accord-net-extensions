using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Extensions;
using Accord.Extensions.Caching;
using Accord.Extensions.Imaging;
using System.Threading;
using Microsoft.VisualBasic.Devices;

namespace Test
{
    public partial class Test
    {
        public void TestMemCache()
        {
            ComputerInfo computerInfo = new ComputerInfo();

            var memCache = new LazyMemoryCache<int, Image<Gray, byte>>
                (
                (currentSize) =>
                {
                    var occupied = computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory;
                    var occupiedPercentage = (float)occupied / computerInfo.TotalPhysicalMemory;

                    if (occupiedPercentage > 0.55)
                        return true;

                    return false;
                },
                (image) => (ulong)(image.Stride * image.Height * image.ColorInfo.Size));


            Random rand = new Random();

            int i = 0;
            while (i < 10000)
            {
                memCache.AddOrUpdate(i, () =>   
                {
                    var image = new Image<Gray, byte>(1 * 1024 * 1024, 1, 0);
                    image.SetValue(252);
                    return image;
                }, 
                (img) => img.Dispose());

                //Thread.Sleep(1);
                Console.WriteLine(computerInfo.AvailablePhysicalMemory / 1024 / 1024);
                i++;
            }

            for (int j = 0; j < 10; j++)
            {
                int b = 0;
                foreach (var item in memCache)
                {
                    Console.WriteLine(item.Value.Value);
                    Console.WriteLine(memCache.HardFaults);
                    //Console.WriteLine(computerInfo.AvailablePhysicalMemory / 1024 / 1024);
                    b++;

                    //GC.Collect();
                }
            }
            
        }

        public void TestLRU()
        {
            ComputerInfo computerInfo = new ComputerInfo();

            var lru = new LRUCache<int, Image<Gray, byte>>(
                                                    (currentSize) => 
                                                        {
                                                            var occupied = computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory;
                                                            var occupiedPercentage = (float)occupied / computerInfo.TotalPhysicalMemory;

                                                            if (occupiedPercentage > 0.85)
                                                                return true;

                                                            return false;
                                                        }, 
                                                    (image) => (ulong)(image.Stride * image.Height * image.ColorInfo.Size));

            lru.OnRemoveItem += lru_OnRemoveItem;

            var image1 = new Image<Gray, byte>(5 * 1024 * 1024, 1, 0);
            image1.SetValue(5 % 256);
            lru.Add(1, image1);

            image1 = new Image<Gray, byte>(5 * 1024 * 1024, 1, 0);
            image1.SetValue(5 % 256);
            lru.Add(1, image1);


            List<Image<Gray, byte>> a = new List<Image<Gray, byte>>();

            Random rand = new Random();

            int i = 0;
            while (i < 10000)
            {
                var image = new Image<Gray, byte>(rand.Next(1, 10) * 1024 * 1024, 1,  0);
                image.SetValue(i % 256);

                /*a.Add(image);
                i++;
                Thread.Sleep(500);*/

                lru.Add(i, image);
                //Thread.Sleep(1);
                Console.WriteLine(computerInfo.AvailablePhysicalMemory / 1024 / 1024);
                i++;
            }
        }

        void lru_OnRemoveItem(LRUCache<int, Image<Gray, byte>> sender, KeyValuePair<int, Image<Gray, byte>> item, bool userRequested)
        {
            //return;

            sender.Oldest.Value.Dispose();
            GC.Collect();

            Console.WriteLine("Kicked out!");
        }
    }
}
