using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINE2D.TemplateMatching;

namespace LINE2D
{
    public class Match
    {
        public int X;
        public int Y;
        public float Score;
        public Template Template;

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

    public class MatchGroupMatching//: GroupMatching<Match>
    {
        public const float MIN_AREA_INTERESECT_PERCENTAGE = 0.8f;

        private static bool IsInGroup(Match candidate, List<Match> matches)
        {
            Rectangle candidateRect = candidate.BoundingRect;
            int candidateRectArea = candidateRect.Width * candidateRect.Height;

            if (matches.Count == 0)
                return true;

            foreach (var match in matches)
            {
                Rectangle matchRect = match.BoundingRect;
                int matchRectArea = matchRect.Width * matchRect.Height;

                Rectangle interesectRect = Rectangle.Intersect(matchRect, candidateRect);
                int intersectRectArea = interesectRect.Width * interesectRect.Height;

                int minMatchArea = Math.Min(matchRectArea, candidateRectArea);

                if (intersectRectArea >= minMatchArea * MIN_AREA_INTERESECT_PERCENTAGE)
                    return true;
            }

            return false;
        }

        public GroupMatch<Match>[] Group(Match[] matches)
        {
            var matchGroups = new Dictionary<GroupMatch<Match>, List<Match>>();

            foreach (var ungrupedMatch in matches)
            {
                bool isGroupFound = false;

                foreach (GroupMatch<Match> matchGroup in matchGroups.Keys)
                {
                    if (IsInGroup(ungrupedMatch, matchGroups[matchGroup]))
                    {
                        matchGroups[matchGroup].Add(ungrupedMatch);
                        isGroupFound = true;
                        break;
                    }
                }

                if (!isGroupFound)
                {
                    var newMatchGroup = new GroupMatch<Match>();

                    matchGroups.Add(newMatchGroup, new List<Match>());
                    matchGroups[newMatchGroup].Add(ungrupedMatch);
                }
            }

            foreach (var pair in matchGroups)
            {
                pair.Key.Detections = pair.Value.ToArray();
            }
            
            return matchGroups.Keys.ToArray();
        }

        /*public MatchGroupMatching()
            : base(null, null, 1,  MIN_AREA_INTERESECT_PERCENTAGE)
        {
            this.AverageFunc = averageDetections;
            this.NearFunc = areDetectionsNear;
        }

        private static Match averageDetections(Match[] detections)
        {
            var sortedDetections = detections.OrderBy(x => x.Score);

            return sortedDetections.ToList()[detections.Length / 2];
        }

        private bool areDetectionsNear(Match m1, Match m2, double threshold)
        {
            Rectangle r1 = m1.BoundingRect;
            Rectangle r2 = m2.BoundingRect;
           
            if (r1.Contains(r2) || r2.Contains(r1))
                return true;

            int minHeight = Math.Min(r1.Height, r2.Height);
            int minWidth = Math.Min(r1.Width, r2.Width);
            double delta = threshold * (minHeight + minWidth);

            return Math.Abs(r1.X - r2.X) <= delta
                && Math.Abs(r1.Y - r2.Y) <= delta
                && Math.Abs(r1.Right - r2.Right) <= delta
                && Math.Abs(r1.Bottom - r2.Bottom) <= delta;
        }*/
    }

    public static class GroupMatchExtensions
    {
        public static Rectangle GetBoundingRect(this GroupMatch<Match> groupMatch)
        {
            var matches = groupMatch.Detections;

            if (matches.Length == 0)
                return Rectangle.Empty;

            Rectangle rect = new Rectangle(UInt16.MaxValue, UInt16.MaxValue, 0, 0);

            foreach (var match in matches)
            {
                Rectangle matchRect = new Rectangle(match.X, match.Y, match.Template.Size.Width, match.Template.Size.Height);

                rect = new Rectangle
                {
                    X = Math.Min(rect.X, matchRect.X),
                    Y = Math.Min(rect.Y, matchRect.Y),
                    Width = Math.Max(rect.Width, matchRect.Width),
                    Height = Math.Max(rect.Height, matchRect.Height)
                };
            }

            return rect;
        }
    }

}
