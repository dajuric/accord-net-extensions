using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions
{
    public struct Size: IEquatable<Size>
    {
        public static readonly Size Empty = new Size(0, 0);

        public int Width;
        public int Height;

        public Size(Point pt)
        {
            Width = pt.X;
            Height = pt.Y;
        }

        public Size(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Determine if this size is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (Width == 0 && Height == 0);
            }
        }

        /// <summary>
        /// Convert a SizeF object into a Size object using ceiling conversion.
        /// </summary>
        /// <param name="value">Size to round.</param>
        /// <returns>Rounded size.</returns>
        public static Size Ceiling(SizeF value)
        {
            return new Size((int)(Math.Ceiling(value.Width)),
                            (int)(Math.Ceiling(value.Height)));
        }

        public override bool Equals(Object obj)
        {
            if (obj is Size == false)
                return false;

            return Equals(this, (Size)obj);
        }

        public override int GetHashCode()
        {
            return (Width ^ Height);
        }

        public static Size Round(SizeF value)
        {
            return new Size((int)(Math.Round(value.Width)),
                            (int)(Math.Round(value.Height)));
        }

        public override String ToString()
        {
            return "{Width = " + Width + ", Height = " + Height + "}";
        }

        public static Size Truncate(SizeF value)
        {
            return new Size((int)(value.Width), (int)(value.Height));
        }

        public static Size operator +(Size sz1, Size sz2)
        {
            return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        public static Size operator -(Size sz1, Size sz2)
        {
            return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public static bool operator ==(Size left, Size right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Size left, Size right)
        {
            return !Equals(left, right);
        }

        public static explicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }

        public static implicit operator SizeF(Size size)
        {
            return new SizeF(size.Width, size.Height);
        }

        public bool Equals(Size other)
        {
            return Equals(this, other);
        }

        public static bool Equals(Size left, Size right)
        {
            return left.Width == right.Width && left.Height == right.Height;
        }
    };

    public struct SizeF : IEquatable<SizeF>
    {
        public static readonly SizeF Empty = new SizeF(0, 0);

        public float Width;
        public float Height;

        public SizeF(PointF pt)
        {
            Width = pt.X;
            Height = pt.Y;
        }

        public SizeF(float width, float height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Determine if this SizeF is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (Width == 0 && Height == 0);
            }
        }

        /// <summary>
        /// Convert a SizeFF object into a SizeF object using ceiling conversion.
        /// </summary>
        /// <param name="value">SizeF to round.</param>
        /// <returns>Rounded SizeF.</returns>
        public static Size Ceiling(SizeF value)
        {
            return new Size((int)(Math.Ceiling(value.Width)),
                            (int)(Math.Ceiling(value.Height)));
        }

        public override bool Equals(Object obj)
        {
            if (obj is SizeF == false)
                return false;

            return Equals(this, (SizeF)obj);
        }

        public override int GetHashCode()
        {
            return ((int)Width ^ (int)Height);
        }

        public static Size Round(SizeF value)
        {
            return new Size((int)(Math.Round(value.Width)),
                            (int)(Math.Round(value.Height)));
        }

        public override String ToString()
        {
            return "{Width = " + Width + ", Height = " + Height + "}";
        }

        public static Size Truncate(SizeF value)
        {
            return new Size((int)(value.Width), (int)(value.Height));
        }

        public static SizeF operator +(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        public static SizeF operator -(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public static bool operator ==(SizeF left, SizeF right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SizeF left, SizeF right)
        {
            return !Equals(left, right);
        }

        public static explicit operator PointF(SizeF size)
        {
            return new PointF(size.Width, size.Height);
        }

        public bool Equals(SizeF other)
        {
            return Equals(this, other);
        }

        public static bool Equals(SizeF left, SizeF right)
        {
            return left.Width == right.Width && left.Height == right.Height;
        }
    }; 

}
