using Accord.Extensions;
using Accord.Extensions.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCO
{
    public static class RandomRegionCreatorExtensions
    {
        public static Rectangle CreateRandomRegion(this Size area, float widthScaleFactor)
        {
            return CreateRandomRegion(area, widthScaleFactor, 0);
        }

        public static Rectangle CreateRandomRegion(this Size area, float widthScaleFactor, int minScale)
        {
            var maxScale = (int)Math.Min(area.Width / widthScaleFactor, area.Height);

            if (minScale > maxScale)
                throw new ArgumentException("Minimal size is greater than valid area!");

            var scale = ParallelRandom.Next(minScale, maxScale);
            var xLoc = ParallelRandom.Next(0, area.Width - (int)Math.Ceiling(scale * widthScaleFactor));
            var yLoc = ParallelRandom.Next(0, area.Height - scale);

            var window = new Rectangle(xLoc, yLoc, (int)Math.Round(scale * widthScaleFactor), scale);
            return window;
        }

        public static Rectangle CreateRandomRegion(this Size area, SizeF minSize)
        {
            var widthScaleFactor = minSize.Width / minSize.Height;
            var minScale = (int)Math.Ceiling(minSize.Height);

            return area.CreateRandomRegion(widthScaleFactor, minScale);
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
