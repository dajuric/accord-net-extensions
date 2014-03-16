using Accord.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Extensions.Imaging;
using Accord.Extensions;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using Point = AForge.IntPoint;
using System.IO;
using System.Runtime.Caching;


namespace RT
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           

            /*Bla();
            return;*/

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RTDemo());
        }

        private static void Bla()
        {
            MemoryCache memCache = new MemoryCache("ll");

            var cip = new CacheItemPolicy();
         

            ImageDirectoryReader reader = new ImageDirectoryReader("C:/imagesConverted", "*.jpg");
            IEnumerable<IImage> images = reader.AsEnumerable();

            var count = images.Count();
            Console.WriteLine(count);

            //var el = images.ElementAt(840);

            var maxWidth = images.Max(x => x.Width);
            Console.WriteLine(maxWidth);

            for (int i = 0; i < 800; i++)
            {
                using (var e = images.ElementAt(i))
                {
                    Console.WriteLine(e);
                }
            }

            return;

            foreach (var im in reader.AsEnumerable())
            {
                Console.WriteLine(im);
                GC.Collect();
            }

            return;

            while (true)
            {       
                var im = reader.Read();
                Console.WriteLine(im);

                //task.Result.Convert<Gray, byte>().Save("C:/imagesConverted/" + new FileInfo(reader.CurrentImageName).Name);
                GC.Collect();
            }

            return;
        }
    }
}
