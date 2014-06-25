using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace LINE2D
{
    public class Match<T>
        where T: ITemplate
    {
        public int X;
        public int Y;
        public float Score;
        public T Template;

        public Rectangle BoundingRect
        {
            get { return GetBoundingRect(this as Match<ITemplate>); }
        }

        public IEnumerable<AForge.Point> Points
        {
            get 
            {
                return Template.Features
                       .Select(x => new AForge.Point(X + x.X, Y + x.Y));
            }
        }

        public static Rectangle GetBoundingRect(Match<ITemplate> m)
        {
            Size size = m.Template.Size;
            return new Rectangle(m.X, m.Y, size.Width, size.Height);
        }

        public static Point GetCenter(Match<ITemplate> m)
        {
            Rectangle matchRect = Match.GetBoundingRect(m);
            Point matchCenter = new Point(matchRect.X + matchRect.Width / 2, matchRect.Y + matchRect.Height / 2);
            return matchCenter;
        }
    }

    public class Match: Match<ITemplate>
    {}

    public static class ImageMatchExtensions
    {
        public static void Draw<TColor, TTemplate>(this Image<TColor, Byte> image, Match<TTemplate> match, TColor color, int thickness = 3,
                                                   bool drawOrientations = false, TColor orientationColor = default(TColor))
            where TColor: IColor3
            where TTemplate: ITemplate
        {
            PointF offset = new PointF(match.X, match.Y);
            image.Draw(match.Template, offset, color, thickness, drawOrientations, orientationColor);
        }

        public static void Draw<TColor>(this Image<TColor, Byte> image, Match match, TColor color, int thickness = 3,
                                        bool drawOrientations = false, TColor orientationColor = default(TColor))
            where TColor : IColor3
        {
            image.Draw<TColor, ITemplate>((Match<ITemplate>)match, color, thickness, drawOrientations, orientationColor);
        }
    }
}
