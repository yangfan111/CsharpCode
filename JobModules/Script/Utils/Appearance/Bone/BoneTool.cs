using Core.Utils;
using UnityEngine;

namespace Utils.Appearance.Bone
{
    public static class BoneTool
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BoneTool));
        
        public static void CacheTransform(GameObject obj)
        {
            var cache = obj.GetComponent<TransformCache>();
            if (cache == null)
            {
                cache = obj.AddComponentUncheckRequireAndDisallowMulti<TransformCache>();
                BoneTool.CacheTransform(obj, cache);
            }
        }
        
        private static void CacheTransform(GameObject role, TransformCache transformCache)
        {
            var cache = transformCache.Cache;
            var transforms = role.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms)
            {
                if (!cache.ContainsKey(transform.name))
                {
                    cache.Add(transform.name, transform);
                }
                else
                {
                    Logger.InfoFormat("{0} is already in {1} cache", transform.name, role.name );
                }
            }
        }

        public static Transform FindTransformFromCache(GameObject obj, string target, bool containsUnActive = false)
        {
            var cache = obj.GetComponent<TransformCache>();
            if (cache == null)
            {
                return null;
            }

            return FindTransformFromCache(cache, target, containsUnActive);
        }

        public static Transform FindTransformFromCache(TransformCache cache, string target, bool containsUnActive = false)
        {
            Transform ret;
            cache.Cache.TryGetValue(target, out ret);
            if(containsUnActive) return ret;
            if (null != ret && null != ret.gameObject && !ret.gameObject.activeInHierarchy) ret = null;
            return ret;
        }
    }
}
