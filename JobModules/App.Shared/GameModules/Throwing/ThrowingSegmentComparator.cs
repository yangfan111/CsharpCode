using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core.Utils;

namespace App.Shared.GameModules.Throwing
{
    public class ThrowingSegmentComparator : IComparer<IThrowingSegment>
    {
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public int Compare(IThrowingSegment x, IThrowingSegment y)
        {
            AssertUtility.Assert(y != null, "y != null");
            AssertUtility.Assert(x != null, "x != null");

            if (x.ServerTime == y.ServerTime) return 0;
            if (x.ServerTime < y.ServerTime) return -1;
            return 1;
        }
    }
}