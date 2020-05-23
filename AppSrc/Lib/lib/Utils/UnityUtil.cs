using UnityEngine;

namespace YF.Utils
{
    public static class UnityUtil
    {
        public static T AddUniqueComponent<T>(MonoBehaviour mono,bool persistNew) where T : Component
        {
            return AddUniqueComponent<T>(mono.gameObject,persistNew);
        }
        public static T AddUniqueComponent<T>(GameObject go,bool persistNew) where T : Component
        {
            var component = go.GetComponent<T>();
            if (component && persistNew)
            {
                Object.DestroyImmediate(component);
            }
            component = go.AddComponent<T>();
            return component;
        }
    }
}