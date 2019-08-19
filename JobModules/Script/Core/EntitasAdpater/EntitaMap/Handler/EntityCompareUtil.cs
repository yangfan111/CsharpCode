using System.Collections;
using System.Collections.Generic;

namespace Core.EntityComponent
{
    public class EntityCompareHelper
    {
        public static int CompareEnumeratableGameComponents(IDictionary<int, IGameComponent> left,
                                                  IDictionary<int, IGameComponent> right, IEntityMapDiffHandler handler,
                                                  EntityDiffData diffCacheData)
        {
            var count = 0;
            foreach (var kv in left)
            {
                count++;
                var lv = kv.Value;
                var k  = kv.Key;

                IGameComponent rv;
                if (right.TryGetValue(k, out rv))
                {
                    if (!handler.IsExcludeComponent(rv))
                        handler.OnDiffComponent(diffCacheData.LeftEntity, lv, diffCacheData.RightEntity, rv);
                }
                else
                {
                    if (!handler.IsExcludeComponent(lv))
                        handler.OnRightComponentMissing(diffCacheData.LeftEntity, diffCacheData.RightEntity, lv);
                }
            }

            foreach (var kv in right)
            {
                count++;

                var k = kv.Key;
                if (!left.ContainsKey(k))
                {
                    var rv = kv.Value;
                    if (!handler.IsExcludeComponent(rv))
                        handler.OnLeftComponentMissing(diffCacheData.LeftEntity, diffCacheData.RightEntity, rv);
                }
            }

            return count;
        }


        public static int CompareEnumeratableGameComponents(List<IGameComponent> left, List<IGameComponent> right,
                                                  IEntityMapDiffHandler diffHandler, EntityDiffData diffCacheData)
        {
            int  loopCount        = 0;
            var  leftEnumerator   = 0;
            var  rightEnumberator = 0;
            int  leftCount        = left.Count;
            int  rightCount       = right.Count;
            bool hasLeft          = leftEnumerator < leftCount;
            bool hasRight         = rightEnumberator < rightCount;
            while (hasLeft && hasRight && !diffHandler.IsBreak())
            {
                loopCount++;
                var leftComponent  = left[leftEnumerator];
                var rightComponent = right[rightEnumberator];
                int result         = GameComponentIComparer.Instance.Compare(leftComponent, rightComponent);
                if (result == 0)
                {
                    // component wise
                    leftEnumerator++;
                    rightEnumberator++;
                    hasLeft  = leftEnumerator < leftCount;
                    hasRight = rightEnumberator < rightCount;
                    if (!diffHandler.IsExcludeComponent(leftComponent))
                    {
                        diffHandler.OnDiffComponent(diffCacheData.LeftEntity, leftComponent, diffCacheData.RightEntity,
                            rightComponent);
                    }
                }
                else if (result < 0)
                {
                    if (!diffHandler.IsExcludeComponent(leftComponent))
                    {
                        diffHandler.OnRightComponentMissing(diffCacheData.LeftEntity, diffCacheData.RightEntity,
                            leftComponent);
                    }

                    leftEnumerator++;
                    hasLeft = leftEnumerator < leftCount;
                }
                else
                {
                    if (!diffHandler.IsExcludeComponent(rightComponent))
                    {
                        diffHandler.OnLeftComponentMissing(diffCacheData.LeftEntity, diffCacheData.RightEntity,
                            rightComponent);
                    }

                    rightEnumberator++;
                    hasRight = rightEnumberator < rightCount;
                }
            }

            while (hasLeft && !diffHandler.IsBreak())
            {
                loopCount++;
                var leftComponent = left[leftEnumerator];
                leftEnumerator++;
                hasLeft = leftEnumerator < leftCount;
                if (!diffHandler.IsExcludeComponent(leftComponent))
                {
                    diffHandler.OnRightComponentMissing(diffCacheData.LeftEntity, diffCacheData.RightEntity,
                        leftComponent);
                }
            }

            while (hasRight && !diffHandler.IsBreak())
            {
                loopCount++;

                var rightComponent = right[rightEnumberator];
                rightEnumberator++;
                hasRight = rightEnumberator < rightCount;
                if (!diffHandler.IsExcludeComponent(rightComponent))
                {
                    diffHandler.OnLeftComponentMissing(diffCacheData.LeftEntity, diffCacheData.RightEntity,
                        rightComponent);
                }
            }

            return loopCount;
        }
    }
}