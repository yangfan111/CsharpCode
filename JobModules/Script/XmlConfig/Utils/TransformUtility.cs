using System;
using UnityEngine;

namespace Core.HitBox
{
    public static  class TransformUtility
    {
        public static Transform FindChildRecursively(this Transform target, string name)
        {
            if (target.childCount == 0 && target.name != name)
                return null;
            foreach (Transform t in target)
                if (t.name == name)
                    return t;
            foreach (Transform t in target)
            {
                var result = FindChildRecursively(t, name);

                if (result == null)
                    continue;
                return result;
            }
            return null;
        }

        public static void Recursively(this Transform target, Action<Transform> action)
        {
            action(target);
            foreach (Transform t in target)
            {
                action(t);
                Recursively(t, action);
            }
        }
    }
}