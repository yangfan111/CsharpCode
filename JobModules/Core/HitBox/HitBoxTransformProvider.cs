using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace Core.HitBox
{
    public class HitBoxTransformProviderCache:DisposableSingleton<HitBoxTransformProviderCache>
    {
        private Dictionary<int, HitBoxTransformProvider> _providers = new Dictionary<int, HitBoxTransformProvider>();
        public HitBoxTransformProvider GetProvider(GameObject o)
        {
            var id =  o.GetInstanceID();
            if (_providers.ContainsKey(id))
            {
                return _providers[id];
            }
            else
            {
                var p= new HitBoxTransformProvider(o);
                _providers[id] = p;
                return p;
            }
        }

        protected override void OnDispose()
        {
            _providers.Clear();
        }
    }
    public class HitBoxTransformProvider : IHitBoxTransformProvider
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HitBoxTransformProvider));
        private readonly Dictionary<string, Transform> _transformCache;
        private readonly Dictionary<int, Transform> _boneToTransformsCache;
        public HitBoxTransformProvider(GameObject currentGameObject)
        {
            _currentGameObject = currentGameObject;
            _transformCache = new Dictionary<string, Transform>();
            _boneToTransformsCache = new Dictionary<int, Transform>();
            BuildTransformCache(currentGameObject.transform, _transformCache);
        }

        public void Update(Vector3 rootPosition, Quaternion rotation)
        {
            RootPosition = rootPosition;
            RootRotation = rotation;
        }

        private void  BuildTransformCache(Transform transform, Dictionary<string,Transform> transformCache)
        {
            // if fetch the transform dynamic, weapon will be included
            if (!transformCache.ContainsKey(transform.name))
            {
                transformCache.Add(transform.name, transform);
                for (int i = 0; i < transform.childCount; i++)
                {
                    var tf = transform.GetChild(i);
                    BuildTransformCache(tf, transformCache);
                }
            }
        }

        private readonly GameObject _currentGameObject;
        public Vector3 RootPosition { get; private set; }

        public Quaternion RootRotation { get; private set; }

        public Transform GetTransform(Transform bone)
        {
            var id = bone.gameObject.GetInstanceID();
            if (_boneToTransformsCache.ContainsKey(id))
            {
                return _boneToTransformsCache[id];
            }
            Transform rc = null;
            _transformCache.TryGetValue(bone.name, out rc);
            _boneToTransformsCache[id] = rc;
            return rc;
        }

        public override string ToString()
        {
            return _currentGameObject.name;
        }
    }
}