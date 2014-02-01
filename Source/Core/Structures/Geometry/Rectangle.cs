using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Rectangle: IEquatable<Rectangle>
    {
        public static readonly Rectangle Empty = new Rectangle();

        [FieldOffset(0)]
        public int X;
        [FieldOffset(4)]
        public int Y;
        [FieldOffset(8)]
        public int Width;
        [FieldOffset(12)]
        public int Height;

        [FieldOffset(0)]
        public Point Location;
        [FieldOffset(8)]
        public Size Size;

        [FieldOffset(0)]
        public int Left;
        [FieldOffset(4)]
        public int Top;
        public int Right { get { return X + Width; } }
        public int Bottom { get { return Y + Height; } }

        public Rectangle(int x, int y, int width, int height)
        {
            this.Left = 0; this.Top = 0;
            this.Location = new Point();
            this.Size = new Size();

            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public Rectangle(Point location, Size size)
        {
            this.Left = 0; this.Top = 0;
            this.Location = new Point();
            this.Size = new Size();

            this.X = location.X;
            this.Y = location.Y;
            this.Width = size.Width;
            this.Height = size.Height;
        }

        public void Offset(int dX, int dY)
        {
            this.X += dX;
            this.Y += dY;
        }

        public void Offset(Point offset)
        {
            this.X += offset.X;
            this.Y += offset.Y;
        }

        public bool Contains(Point p)
        {
            return p.X >= this.X && p.X <= this.Right &&
                   p.Y >= this.Y && p.Y <= this.Bottom;
        }

        public bool Contains(Rectangle rectangle)
        {
            return rectangle.Left >= this.Left &&
                   rectangle.Top >= this.Top &&
                   rectangle.Right <= this.Right &&
                   rectangle.Bottom <= this.Bottom;
        }

        public void Intersect(Rectangle rectangle)
        {
            this.X = Math.Max(rectangle.X, this.X);
            this.Y = Math.Max(rectangle.Y, this.Y);
            this.Width = Math.Min(rectangle.Width, this.Width);
            this.Height = Math.Min(rectangle.Height, this.Height);
        }

        public static Rectangle Intersect(Rectangle rectangleA, Rectangle rectangleB)
        {
            Rectangle rect = new Rectangle();
            intersect(rectangleA, rectangleB, ref rect);
            return rect;
        }

        private static void intersect(Rectangle rectangleA, Rectangle rectangleB, ref Rectangle intersectedRect)
        {
            intersectedRect.X = Math.Max(rectangleA.X, rectangleB.X);
            intersectedRect.Y = Math.Max(rectangleA.Y, rectangleB.Y);
            intersectedRect.Width = Math.Min(rectangleA.Width, rectangleB.Width);
            intersectedRect.Height = Math.Min(rectangleA.Height, rectangleB.Height);
        }

        public static Rectangle Round(RectangleF rectangle)
        {
            return new Rectangle 
            {
                X = (int)Math.Round(rectangle.X),
                Y = (int)Math.Round(rectangle.Y),
                Width = (int)Math.Round(rectangle.Width),
                Height = (int)Math.Round(rectangle.Height)
            };
        }

        public static implicit operator RectangleF(Rectangle rectangle)
        { 
            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public bool Equals(Rectangle other)
        {
            return this.X == other.X && this.Y == other.Y &&
                  this.Width == other.Width && this.Height == other.Height;
        }

        public bool IsEmpty
        {
            get { return this.Equals(Empty); }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RectangleF
    {
        public static readonly RectangleF Empty = new RectangleF();

        [FieldOffset(0)]
        public float X;
        [FieldOffset(4)]
        public float Y;
        [FieldOffset(8)]
        public float Width;
        [FieldOffset(12)]
        public float Height;

        [FieldOffset(0)]
        public PointF Location;
        [FieldOffset(8)]
        public SizeF Size;

        [FieldOffset(0)]
        public float Left;
        [FieldOffset(4)]
        public float Top;
        public float Right { get { return X + Width; } }
        public float Bottom { get { return Y + Height; } }

        public RectangleF(float x, float y, float width, float height)
        {
            this.Left = 0; this.Top = 0;
            this.Location = new PointF();
            this.Size = new SizeF();

            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public RectangleF(PointF location, SizeF size)
        {
            this.Left = 0; this.Top = 0;
            this.Location = new PointF();
            this.Size = new SizeF();

            this.X = location.X;
            this.Y = location.Y;
            this.Width = size.Width;
            this.Height = size.Height;
        }

        public void Offset(int dX, int dY)
        {
            this.X += dX;
            this.Y += dY;
        }

        public void Inflate(int dX, int dY)
        {
            this.X -= dX;
            this.Y -= dY;
            this.Width += 2 * dX;
            this.Height += 2 * dY;
        }

        public void Intersect(RectangleF rectangle)
        {
            this.X = Math.Max(rectangle.X, this.X);
            this.Y = Math.Max(rectangle.Y, this.Y);
            this.Width = Math.Min(rectangle.Width, this.Width);
            this.Height = Math.Min(rectangle.Height, this.Height);
        }

        public static RectangleF Intersect(RectangleF rectangleA, RectangleF rectangleB)
        {
            RectangleF rect = new RectangleF();
            intersect(rectangleA, rectangleB, ref rect);
            return rect;
        }

        private static void intersect(RectangleF rectangleA , RectangleF rectangleB, ref RectangleF intersectedRect)
        {
            intersectedRect.X      = Math.Max(rectangleA.X, rectangleB.X);
            intersectedRect.Y      = Math.Max(rectangleA.Y, rectangleB.Y);
            intersectedRect.Width  = Math.Min(rectangleA.Width, rectangleB.Width);
            intersectedRect.Height = Math.Min(rectangleA.Height, rectangleB.Height);
        }
    }
}
