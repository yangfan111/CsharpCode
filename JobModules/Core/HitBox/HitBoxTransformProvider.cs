using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace Core.HitBox
{
    public class HitBoxTransformProvider : IHitBoxTransformProvider
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HitBoxTransformProvider));
        private readonly Dictionary<string, Transform> _transformCache;

        public HitBoxTransformProvider(GameObject currentGameObject, Vector3 rootPosition, Quaternion rotation)
        {
            _currentGameObject = currentGameObject;
            _transformCache = new Dictionary<string, Transform>();
            RootPosition = rootPosition;
            RootRotation = rotation;
            BuildTransformCache(currentGameObject.transform, _transformCache);
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

        public Transform GetTransform(string boneName)
        {
            Transform rc = null;
            _transformCache.TryGetValue(boneName, out rc);
            return rc;
        }

        public override string ToString()
        {
            return _currentGameObject.name;
        }
    }
}