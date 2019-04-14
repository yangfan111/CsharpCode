using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.Singleton;

namespace Core.HitBox
{
    public class HitBoxTransformProviderCache:DisposableSingleton<HitBoxTransformProviderCache>
    {
        private Dictionary<int, HitBoxTransformProvider> _providers = new Dictionary<int, HitBoxTransformProvider>();
        public HitBoxTransformProvider GetProvider(GameObject o)
        {
            var id =  o.GetInstanceID();
            if (!_providers.ContainsKey(id))
                _providers[id] = new HitBoxTransformProvider(o);
            return _providers[id];
        }

        protected override void OnDispose()
        {
            _providers.Clear();
        }
    }
    public class HitBoxTransformProvider : IHitBoxTransformProvider
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HitBoxTransformProvider));

        private readonly GameObject _currentGameObject;
        private HitBoxCache _handler;

        public float BoundSphereRadius()
        {
            return _handler.SphereRadius(); 
        }

        public Vector3 BoundSpherePosition()
        {
            return _handler.SphereCenter();
        }

        public HitBoxTransformProvider(GameObject currentGameObject)
        {
            _currentGameObject = currentGameObject;
            _handler = currentGameObject.GetComponent<HitBoxCache>();
            if (_handler == null)
                _handler = currentGameObject.AddComponent<HitBoxCache>();
            _handler.Init();
        }
        
        public void SetActive(bool active)
        {
            foreach (var item in _handler.GetHitBox())
            {
                if(item.Value!=null)
                    item.Value.enabled = active;
            }
        }

        public void SetColliderInRigidBody(bool active)
        {
            foreach (var item in _handler.GetRigidBobies())
            {
                if(item.Value!=null)
                    item.Value.detectCollisions = active;
            }
        }

        public void FlushLayerOfHitBox()
        {
            foreach (var item in _handler.GetTransforms())
            {
                item.Value.gameObject.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.Hitbox);
            }
        }

        public Dictionary<string,Collider> GetHitBoxColliders()
        {
            return _handler.GetHitBox();
        }

        public Dictionary<string, Rigidbody> GetRigidBodies()
        {
            return _handler.GetRigidBobies();
        }
        
        public Dictionary<string, Transform> GetHitBoxTransforms()
        {
            return _handler.GetTransforms();
        }
        
        public override string ToString()
        {
            return _currentGameObject.name;
        }
    }
}