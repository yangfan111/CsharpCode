using System;
using System.Collections.Generic;
using Core.Utils.System46;

namespace Core.EntityComponent
{
    public interface IEnumeratableComparatorHandler<TItem>
    {
        int CompareItem(TItem leftItem, TItem rightItem);
        void OnLeftItemMissing(TItem rightItem);
        void OnRightItemMissing(TItem leftItem);
        void OnItemSame(TItem leftItem, TItem rightItem);
        bool IsBreak();
    }

    public class DictionaryComarator<TItem>
    {
        public static int Diff(MyDictionary<int, TItem> left,
            MyDictionary<int, TItem> right, IEnumeratableComparatorHandler<TItem> handler,
            Func<TItem, bool> filter = null,bool skipMissHandle = false)
        {
            int loopCount = 0;
          
            foreach (var kv in left)
            {
                loopCount++;
                var lv = kv.Value;
                var k = kv.Key;
                if (filter != null && filter(lv))
                {
                    TItem rv;
                    if (right.TryGetValue(k, out rv))
                    {
                        handler.OnItemSame(lv, rv);
                    }
                    else if(!skipMissHandle)
                    {
                        handler.OnRightItemMissing(lv);
                    }
                }
            }

            if (!skipMissHandle)
            {
                foreach (var kv in right)
                {
                    loopCount++;
                    var rv = kv.Value;
                    var k = kv.Key;
                    if (filter != null && filter(rv) && !left.ContainsKey(k))
                    {
                        handler.OnLeftItemMissing(rv);
                    }
                }
            }

            return loopCount;
        }
    }

    public class SortedEnumeratableComparator<TItem>
    {
        public static int Diff(List<TItem> left,
            List<TItem> right, IEnumeratableComparatorHandler<TItem> handler, Func<TItem, bool> filter = null)
        {
            int loopCount = 0;
            var leftEnumerator = 0;
            var rightEnumberator = 0;
            int leftCount = left.Count;
            int rightCount = right.Count;
            bool hasLeft = leftEnumerator < leftCount;
            bool hasRight = rightEnumberator < rightCount;
            while (hasLeft && hasRight && !handler.IsBreak())
            {
                loopCount++;
                var leftEntry = left[leftEnumerator];
                var rightEntry = right[rightEnumberator];
                int result = handler.CompareItem(leftEntry, rightEntry);
                if (result == 0)
                {
                    // component wise
                    leftEnumerator++;
                    rightEnumberator++;
                    hasLeft = leftEnumerator < leftCount;
                    hasRight = rightEnumberator < rightCount;
                    if (filter != null && filter(leftEntry))
                    {
                        handler.OnItemSame(leftEntry, rightEntry);
                    }
                }
                else if (result < 0)
                {
                    if (filter != null && filter(leftEntry))
                    {
                        handler.OnRightItemMissing(leftEntry);
                    }

                    leftEnumerator++;
                    hasLeft = leftEnumerator < leftCount;
                }
                else
                {
                    if (filter != null && filter(rightEntry))
                    {
                        handler.OnLeftItemMissing(rightEntry);
                    }

                    rightEnumberator++;
                    hasRight = rightEnumberator < rightCount;
                }
            }

            while (hasLeft && !handler.IsBreak())
            {
                loopCount++;
                var leftEntry = left[leftEnumerator];
                leftEnumerator++;
                hasLeft = leftEnumerator < leftCount;
                if (filter != null && filter(leftEntry))
                {
                    handler.OnRightItemMissing(leftEntry);
                }
            }

            while (hasRight && !handler.IsBreak())
            {
                loopCount++;

                var rightEntry = right[rightEnumberator];
                rightEnumberator++;
                hasRight = rightEnumberator < rightCount;
                if (filter != null && filter(rightEntry))
                {
                    handler.OnLeftItemMissing(rightEntry);
                }
            }

            return loopCount;
        }
    }
}