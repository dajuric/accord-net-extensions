using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions.Math.Geometry
{
    public struct Point<T>
    {
        public T X;
        public T Y;

        public override bool Equals(object obj)
        {
            if (obj is Point<T> == false)
                return false;

            var pt = (Point<T>)obj;

            if (this.X.Equals(pt.X) && this.Y.Equals(pt.Y))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}", this.X, this.Y);
        }
    }
}
