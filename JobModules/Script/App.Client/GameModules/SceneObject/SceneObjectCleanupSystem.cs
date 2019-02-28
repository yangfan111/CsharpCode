using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using UnityEngine;
using UserInputManager.Lib;
using Utils.AssetManager;

namespace App.Client.GameModules.SceneObject
{
    public class SceneObjectCleanupSystem : ReactiveEntityCleanUpSystem<SceneObjectEntity>
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(SceneObjectCleanupSystem));
        private readonly Contexts _contexts;
        public SceneObjectCleanupSystem(Contexts contexts) : base(contexts.sceneObject)
        {
            _contexts = contexts;
        }

        public override void SingleExecute(SceneObjectEntity entity)
        {
            if (entity.hasUnityObject)
            {
                var model = entity.unityObject.UnityObject.AsGameObject;
                if(null != model)
                {
                    DestroyRaycastTarget(model);
                    Resize(model);
                    if(entity.hasEffects)
                    {
                        DestroyEffects(entity, model);
                    }
                }
               
            }
            else if (entity.hasMultiUnityObject)
            {
                var models = entity.multiUnityObject.LoadedAssets;
                foreach(var model in models)
                {
                    if(null == model.Value)
                    {
                        Logger.ErrorFormat("model is null for scene object {0}", model.Key);
                        continue;
                    }
                    var go = model.Value.AsGameObject; 
                    if(null == go)
                    {
                        Logger.ErrorFormat("asset is not gameobject in assets of {0}", entity.entityKey);
                        continue;
                    }
                    DestroyRaycastTarget(go);
                    Resize(go);
                    if(entity.hasEffects)
                    {
                        DestroyEffects(entity, go);
                    }
                }
            }
           
        }

        private void DestroyEffects(SceneObjectEntity entity, GameObject model)
        {
            foreach (var effect in entity.effects.GlobalEffects)
            {
                _contexts.session.clientSessionObjects.GlobalEffectManager.RemoveGameObject(effect, model);
            }
        }

       

        protected override bool Filter(SceneObjectEntity entity)
        {
            return true;
        }

        protected override ICollector<SceneObjectEntity> GetTrigger(IContext<SceneObjectEntity> context)
        {
            return context.CreateCollector(SceneObjectMatcher.FlagDestroy);
        }

        private void Resize(GameObject model)
        {
            model.transform.localScale = Vector3.one;
        }

        private void DestroyRaycastTarget(GameObject model)
        {
            var col = model.transform.Find(SceneObjectConstant.NormalColliderName);
            if(null != col)
            {
                var target = col.GetComponent<RayCastTarget>();
                if(null != target)
                {
                    Object.Destroy(target);
                }
                var trigger = col.GetComponent<SceneObjectTriggerEnterListener>();
                if(null != trigger)
                {
                    Object.Destroy(trigger);
                }
                col.GetComponent<Collider>().enabled = false;
            }
        }
    }
}