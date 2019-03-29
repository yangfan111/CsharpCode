using App.Client.CastObjectUtil;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using UnityEngine;
using UserInputManager.Lib;
using UserInputManager.Utility;

namespace App.Client.GameModules.SceneObject
{
    public class SceneObjectCastTargetLoadSystem : ReactiveResourceLoadSystem<SceneObjectEntity>
    {
        private Queue<RayCastTarget> _gameObjectPool = new Queue<RayCastTarget>();
        private const string PoolName = "SceneObjectCastPool";
        private const string DefaultName = "SceneObjectCastGo";
        private GameObject _poolRoot;
        public SceneObjectCastTargetLoadSystem(SceneObjectContext sceneObjectContext):base(sceneObjectContext)
        {
            _poolRoot = new GameObject(PoolName);
            _gameObjectPool.Clear();
        }

        public void Recycle(GameObject gameObject)
        {
            _gameObjectPool.Enqueue(gameObject.GetComponent<RayCastTarget>());
            gameObject.SetActive(false);
        }

        public GameObject Get(int entityId, int key, string tip)
        {
            if(_gameObjectPool.Count < 1)
            {
                _gameObjectPool.Enqueue(MakeCastTarget());
            }
            var target = _gameObjectPool.Dequeue();
            target.gameObject.SetActive(true);
            CommonCastData.Make(target, entityId,  key, tip);
            return target.gameObject;
        }

        private RayCastTarget MakeCastTarget()
        {
            var go = new GameObject(DefaultName);
            var col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            var target = RayCastTargetUtil.AddRayCastTarget(go);
            go.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.UserInputRaycast);
            go.transform.parent = _poolRoot.transform;
            return target;
        }

        public override void SingleExecute(SceneObjectEntity entity)
        {
            if(entity.isFlagDestroy && entity.hasRawGameObject)
            {
                Recycle(entity.rawGameObject.Value);
            }
            else
            {
                var go = Get(entity.entityKey.Value.EntityId, entity.simpleCastTarget.Key, entity.simpleCastTarget.Tip);
                go.transform.localScale = entity.simpleCastTarget.Scale * Vector3.one;
                entity.AddRawGameObject(go);
                go.transform.position = entity.position.Value;
                go.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.UserInputRaycast);
            }
        }

        protected override ICollector<SceneObjectEntity> GetTrigger(IContext<SceneObjectEntity> context)
        {
            return context.CreateCollector(SceneObjectMatcher.Position.Added(), SceneObjectMatcher.FlagDestroy.Added());
        }

        protected override bool Filter(SceneObjectEntity entity)
        {
            return entity.hasSimpleCastTarget;
        }
    }
}
