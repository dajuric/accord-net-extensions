using System.Drawing;

namespace LINE2D
{
    public class Match<T>: Match where T: ITemplate
    {
        public new T Template 
        {
            get { return (T)base.Template; }
            set { base.Template = value; }
        }
    }

    public class Match
    {
        public int X;
        public int Y;
        public float Score;
        public ITemplate Template;

        public Rectangle BoundingRect
        {
            get { return GetBoundingRect(this); }
        }

        public static Rectangle GetBoundingRect(Match m)
        {
            Size size = m.Template.Size;
            return new Rectangle(m.X, m.Y, size.Width, size.Height);
        }

        public static Point GetCenter(Match m)
        {
            Rectangle matchRect = Match.GetBoundingRect(m);
            Point matchCenter = new Point(matchRect.X + matchRect.Width / 2, matchRect.Y + matchRect.Height / 2);
            return matchCenter;
        }
    }

    
}
