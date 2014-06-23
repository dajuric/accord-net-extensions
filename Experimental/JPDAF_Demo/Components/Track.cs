using System.Threading;

namespace JPDAF_Demo
{
    public class Track<TTracker, TTag> 
    {
        static int nextID = 0;

        public int ID { get; private set; }

        public bool IsTentative { get; set; }

        public TTracker Tracker { get; private set; }

        public TTag Tag { get; set; }

        public Track(TTracker tracker, bool isTentative = true)
        {
            this.Tracker = tracker;
            this.ID = nextID;
            this.IsTentative = isTentative;

            Interlocked.Increment(ref nextID);
        }
    }
}
