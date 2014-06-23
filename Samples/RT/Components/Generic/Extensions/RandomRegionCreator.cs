using Accord.Extensions;
using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT
{
    public static class RandomRegionCreatorExtensions
    {
        public static Rectangle CreateRandomRegion(this Size area, float widthScale)
        {
            var row = ParallelRandom.Next() % area.Height;
            var col = ParallelRandom.Next() % area.Width;
            var scale = ParallelRandom.Next() % (2 * Math.Min(Math.Min(row, area.Height - row), Math.Min(col, area.Width - col)) + 1);

            Rectangle window;
            if(widthScale >= 1)
                window = Rectangle.Round(fromElipsoid(col, row, scale, scale / widthScale));
            else
                window = Rectangle.Round(fromElipsoid(col, row, scale * widthScale, scale));

            return window;
        }

        public static Rectangle CreateRandomRegion(this Size area, float widthScale, Size minSize)
        {
            Rectangle region;
            while (true)
            {
                region = area.CreateRandomRegion(widthScale);

                if (region.Width >= minSize.Width && region.Height >= minSize.Height)
                    break;
            }

            return region;
        }

        private static RectangleF fromElipsoid(float centerX, float centerY, float width, float height)
        {
            return new RectangleF
            {
                X = centerX - width / 2,
                Y = centerY - height / 2,
                Width = width,
                Height = height
            };
        }
    }
}
