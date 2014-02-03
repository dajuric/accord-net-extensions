using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions
{
    public class History<T> : ICloneable, IEnumerable<T>, IEnumerator<T>
    {
        public delegate void AddElement(T elem);
        public event AddElement OnAddElement;

        private List<T> histElems;
        int maxNumOfElems;

        public History(int maxNumOfElems = UInt16.MaxValue)
        {
            this.histElems = new List<T>();
            this.maxNumOfElems = maxNumOfElems;
        }

        public void Add(T elem)
        {
            if (histElems.Count == maxNumOfElems)
            {
                histElems.RemoveAt(0); //remove the oldest element
            }

            histElems.Add(elem);

            if (OnAddElement != null)
                OnAddElement(elem);
        }

        public T this[int histDepth]
        {
            get
            {
                T elem;

                if (histDepth > histElems.Count - 1)
                    elem = histElems.FirstOrDefault(); //the oldest element
                else
                    elem = histElems[histElems.Count - histDepth - 1];

                return elem;
            }
            set
            {
                if (histElems.Count == 0)
                    this.Add(value);

                else if (histDepth > histElems.Count - 1)
                    histElems[0] = value;

                else
                    histElems[histElems.Count - histDepth - 1] = value;
            }
        }

        public T Current
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        public T Oldest
        {
            get { return this[histElems.Count - 1]; }
            set { this[histElems.Count - 1] = value; }
        }

        public int Count { get { return this.histElems.Count; } }
        public int MaxCount { get { return this.maxNumOfElems; } }

        public void Clear()
        {
            this.histElems.Clear();
        }

        /// <summary>
        /// Returns part of the history. First element is the newest.
        /// </summary>
        public List<T> GetRange(int maxHistDepth)
        {
            int maxDepth = Math.Max(this.histElems.Count - maxHistDepth, 0);

            List<T> range = new List<T>();
            for (int i = this.histElems.Count - 1; i >= maxDepth; i--)
            {
                range.Add(this.histElems[i]);
            }

            return range;
        }

        public void RemoveRange(int startDepth, int numOfElems)
        {
            if (startDepth >= this.histElems.Count)
                return;

            int index = Math.Max(this.histElems.Count - 1 - startDepth - numOfElems, 0);
            int count = Math.Min(numOfElems, this.histElems.Count - 1 - index);

            this.histElems.RemoveRange(index, count);
        }

        public List<T> GetAllElements()
        {
            return GetRange(this.Count);
        }

        public override string ToString()
        {
            string str = "";
            foreach (var item in histElems)
            {
                str += item.ToString() + ", ";
            }

            str = str.Remove(str.Length - 3);
            return str;
        }

        public object Clone()
        {
            History<T> hist = new History<T>(this.maxNumOfElems);
            hist.histElems.AddRange(this.histElems);
            return hist;
        }

        #region IEnumerable
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        T IEnumerator<T>.Current
        {
            get { return this[idx]; }
        }

        void IDisposable.Dispose()
        { }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return (++idx < this.Count);
        }

        int idx = 0;
        void System.Collections.IEnumerator.Reset()
        {
            idx = 0;
        }
        #endregion
    }
}
