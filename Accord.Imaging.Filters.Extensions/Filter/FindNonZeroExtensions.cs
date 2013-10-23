using Accord.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
    public static class FindNonZeroExtensions
    {
        delegate void FindNonZeroFunc(IImage img, out List<Point> locations, out IList values);
        static Dictionary<Type, FindNonZeroFunc> findNonZeroFuncs;

        static FindNonZeroExtensions()
        {
            findNonZeroFuncs = new Dictionary<Type, FindNonZeroFunc>();

            findNonZeroFuncs.Add(typeof(float), findNonZero_Float);
        }

        public static List<Point> FindNonZero<TDepth>(this Image<Gray, TDepth> img)
            where TDepth : struct
        {
            List<TDepth> values;
            return FindNonZero(img, out values);
        }

        public static List<Point> FindNonZero<TDepth>(this Image<Gray, TDepth> img, out List<TDepth> values)
            where TDepth: struct
        {
            FindNonZeroFunc findNonZeroFunc = null;
            if (findNonZeroFuncs.TryGetValue(img.ColorInfo.ChannelType, out findNonZeroFunc) == false)
                throw new NotSupportedException(string.Format("Can not perform FindNonZero on an image of type {0}", img.ColorInfo.ChannelType.Name));

            var locations = new List<Point>();
            var _values = new List<TDepth>();

            var proc = new ParallelProcessor<IImage, bool>(img.Size, 
                                                          () => true, 
                                                          (_src, _, area) => 
                                                          {
                                                              List<Point> locationsPatch;
                                                              IList valuesPatch;
                                                              findNonZeroFunc(img.GetSubRect(area), out locationsPatch, out valuesPatch);

                                                              lock(locations)
                                                              lock (_values)
                                                              {
                                                                  locationsPatch.ForEach(x => 
                                                                  {
                                                                      x.Offset(area.Location);
                                                                      locations.Add(x);
                                                                  });

                                                                  _values.AddRange(valuesPatch  as IList<TDepth>);
                                                              }
                                                          });

            proc.Process(img);

            values = _values;
            return locations;
        }

        private unsafe static void findNonZero_Float(IImage img, out List<Point> locations, out IList values)
        {
            locations = new List<Point>();
            var _values = new List<float>();

            float* ptr = (float*)img.ImageData;
            int stride = img.Stride;

            int width = img.Width;
            int height = img.Height;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var val = ptr[col];
                    
                    if (val != 0)
                    {
                        locations.Add(new Point(col, row));
                        _values.Add(val);
                    }
                }

                ptr = (float*)((byte*)ptr + stride);
            }

            values = _values;
        }
    }
}
