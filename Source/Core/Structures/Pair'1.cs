using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions
{
    public class Pair<T>
    {
        public T First;
        public T Second;

        public Pair()
        {
            this.First = default(T);
            this.Second = default(T);
        }

        public Pair(T first, T second)
        {
            this.First = first;
            this.Second = second;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is Pair<T> == false)
                return false;

            var pair = obj as Pair<T>;

            if (pair.First.Equals(this.First) &&
                pair.Second.Equals(this.Second))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("<{0}, {1}>", First, Second);
        }
    }
}
