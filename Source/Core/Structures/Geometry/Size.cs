using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace Accord.Extensions
{
    public struct SizeF
    {
        // Internal state.
        private float width;
        private float height;

        // The empty size.
        public static readonly SizeF Empty = new SizeF(0.0f, 0.0f);

        // Constructors.
        public SizeF(PointF pt)
        {
            width = pt.X;
            height = pt.Y;
        }
        public SizeF(SizeF size)
        {
            width = size.width;
            height = size.height;
        }
        public SizeF(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        // Determine if this size is empty.
        public bool IsEmpty
        {
            get
            {
                return (width == 0.0f && height == 0.0f);
            }
        }

        // Get or set the width.
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        // Get or set the height.
        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        // Determine if two sizes are equal.
        public override bool Equals(Object obj)
        {
            if (obj is SizeF)
            {
                SizeF other = (SizeF)obj;
                return (width == other.width && height == other.height);
            }
            else
            {
                return false;
            }
        }

        // Get a hash code for this object.
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Convert this size into a point.
        public PointF ToPointF()
        {
            return new PointF(width, height);
        }

        // Convert this size into its integer form.
        public Size ToSize()
        {
            return new Size((int)width, (int)height);
        }

#if CONFIG_EXTENDED_NUMERICS

	// Convert this object into a string.
	public override String ToString()
			{
				return "{Width=" + width.ToString() +
					   ", Height=" + height.ToString() + "}";
			}

#endif

        // Overloaded operators.
        public static SizeF operator +(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.width + sz2.width,
                             sz1.height + sz2.height);
        }
        public static SizeF operator -(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.width - sz2.width,
                             sz1.height - sz2.height);
        }
        public static bool operator ==(SizeF left, SizeF right)
        {
            return (left.width == right.width &&
                    left.height == right.height);
        }
        public static bool operator !=(SizeF left, SizeF right)
        {
            return (left.width != right.width ||
                    left.height != right.height);
        }
        public static explicit operator PointF(SizeF size)
        {
            return new PointF(size.width, size.height);
        }

    }; // struct SizeF

    public struct Size
    {
        // Internal state.
        private int width;
        private int height;

        // The empty size.
        public static readonly Size Empty = new Size(0, 0);

        // Constructors.
        public Size(Point pt)
        {
            width = pt.X;
            height = pt.Y;
        }
        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        // Determine if this size is empty.
        public bool IsEmpty
        {
            get
            {
                return (width == 0 && height == 0);
            }
        }

        // Get or set the width.
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        // Get or set the height.
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

	// Convert a SizeF object into a Size object using ceiling conversion.
	public static Size Ceiling(SizeF value)
			{
				return new Size((int)(Math.Ceiling(value.Width)),
								(int)(Math.Ceiling(value.Height)));
			}


        // Determine if two sizes are equal.
        public override bool Equals(Object obj)
        {
            if (obj is Size)
            {
                Size other = (Size)obj;
                return (width == other.width && height == other.height);
            }
            else
            {
                return false;
            }
        }

        // Get a hash code for this object.
        public override int GetHashCode()
        {
            return (width ^ height);
        }

	// Convert a SizeF object into a Size object using rounding conversion.
	public static Size Round(SizeF value)
			{
				return new Size((int)(Math.Round(value.Width)),
								(int)(Math.Round(value.Height)));
			}

        // Convert this object into a string.
        public override String ToString()
        {
            return "{Width=" + width.ToString() +
                   ", Height=" + height.ToString() + "}";
        }

        // Convert a SizeF object into a Size object using truncating conversion.
        public static Size Truncate(SizeF value)
        {
            return new Size((int)(value.Width), (int)(value.Height));
        }

        // Overloaded operators.
        public static Size operator +(Size sz1, Size sz2)
        {
            return new Size(sz1.width + sz2.width, sz1.height + sz2.height);
        }
        public static Size operator -(Size sz1, Size sz2)
        {
            return new Size(sz1.width - sz2.width, sz1.height - sz2.height);
        }
        public static bool operator ==(Size left, Size right)
        {
            return (left.width == right.width &&
                    left.height == right.height);
        }
        public static bool operator !=(Size left, Size right)
        {
            return (left.width != right.width ||
                    left.height != right.height);
        }
        public static explicit operator Point(Size size)
        {
            return new Point(size.width, size.height);
        }
        public static implicit operator SizeF(Size size)
        {
            return new SizeF(size.width, size.height);
        }

    }; // struct Size
}
