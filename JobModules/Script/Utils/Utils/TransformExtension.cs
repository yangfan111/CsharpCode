using System;
using UnityEngine;

namespace Utils.Utils
{
    public static class TransformExtension
    {
        public static Transform FindTransformRecursive(this Transform c, string target,
            int MaxRecursiveCount = Int32.MaxValue)
        {
            if (!c.gameObject.activeSelf) return null;
            var r = c.Find(target);
            if (r != null)
            {
                return r;
            }
            if (MaxRecursiveCount > 0)
            {
                var count = c.childCount;
                for (int i = 0; i < count; i++)
                {
                    var o = c.GetChild(i);

                    r = o.FindTransformRecursive(target, MaxRecursiveCount - 1);
                    if (r != null)
                    {
                        return r;
                    }
                }
            }

            return null;
        }
    }
}