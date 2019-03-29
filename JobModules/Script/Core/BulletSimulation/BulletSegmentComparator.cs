using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core.Utils;

namespace Core.BulletSimulation
{
    public class BulletSegmentComparator : IComparer<DefaultBulletSegment>
    {
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public int Compare(DefaultBulletSegment x, DefaultBulletSegment y)
        {
            AssertUtility.Assert(y != null, "y != null");
            AssertUtility.Assert(x != null, "x != null");

            if (x.ServerTime == y.ServerTime) return 0;
            if (x.ServerTime < y.ServerTime) return -1;
            return 1;
        }
    }
}