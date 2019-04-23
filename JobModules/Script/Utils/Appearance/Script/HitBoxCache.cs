using System.Collections.Generic;
using Core.Utils;
using UnityEngine;

namespace Utils.Appearance
{
       
    public class HitBoxCache:MonoBehaviour
    {
        private Dictionary<string, Collider> _hitBoxCache;
        private Dictionary<string, Transform> _transformCache;
        private Dictionary<string, Rigidbody> _rigidbodyCache;

        private readonly Vector3 _center = new Vector3(0, 1, 0);
        private readonly float _radius = 2f;
        public Vector3 SphereCenter()
        {
            return _center; 
        }

        public float SphereRadius()
        {
            return _radius;
        }
        
        public void Init()
        {
            _hitBoxCache = new Dictionary<string, Collider>();
            _transformCache = new Dictionary<string, Transform>();
            _rigidbodyCache = new Dictionary<string, Rigidbody>();
            SetCache(transform);
//            PrintCache();
        }

        private void SetCache(Transform trans)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                var child = trans.GetChild(i);
                SetCache(child);
                
                var rigidbody = child.GetComponent<Rigidbody>();
                if (rigidbody != null) _rigidbodyCache.Add(child.name, rigidbody);

                if (child.gameObject.layer != UnityLayerManager.GetLayerIndex(EUnityLayerName.Hitbox)) continue;

                var collider = child.GetComponent<Collider>();
                if (collider == null) continue;
                _transformCache.Add(child.name, child);
                _hitBoxCache.Add(child.name, collider);
            }
        }

        public Dictionary<string, Collider> GetHitBox()
        {
            if (_hitBoxCache != null) return _hitBoxCache;
            Init();
            return _hitBoxCache;
        }

        public Dictionary<string, Rigidbody> GetRigidBobies()
        {
            if (_rigidbodyCache != null) return _rigidbodyCache;
            Init();
            return _rigidbodyCache;
        }

        public Dictionary<string, Transform> GetTransforms()
        {
            if (_transformCache != null) return _transformCache;
            Init();
            return _transformCache;
        }
        
        private void PrintCache()
        {
            foreach (var item in _transformCache)
            {
                Debug.Log("HitBoxCache  name:" + item.Key + " " + item.Value.gameObject.layer + " " +
                          item.Value.gameObject.active + " " + _hitBoxCache[item.Key].enabled);
            }
            Debug.Log("HitBox Sphere: center:" + _center + " radius:" + _radius);
        }
    }
}