using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using Core;
using App.Shared.SceneTriggerObject;
using Core.IFactory;
using Entitas;
using UnityEngine;
using Utils.Singleton;

namespace App.Server.GameModules.SceneObject
{

    public abstract class ServerMapTriggerObjectListener : ITriggerObjectListener
    {
        protected IMapObjectEntityFactory MapObjectEntityFactory;
        private IGroup<MapObjectEntity> _mapObjGroup;
       
        protected ServerMapTriggerObjectListener(Contexts contexts, ETriggerObjectType triggerType, IMatcher<MapObjectEntity> mapMatcher)
        {
            var triggerManager =  SingletonManager.Get<TriggerObjectManager>();
            triggerManager.RegisterListener(triggerType, this);
            MapObjectEntityFactory= contexts.session.entityFactoryObject.MapObjectEntityFactory;
            _mapObjGroup = contexts.mapObject.GetGroup(mapMatcher);
        }

        public abstract void OnTriggerObjectLoaded(string id, GameObject gameObject);

        public void OnTriggerObjectUnloaded(string id)
        {
            foreach (var obj in _mapObjGroup.GetEntities())
            {
                if (obj.triggerObjectId.Id == id)
                {
                    obj.isFlagDestroy = true;
                    break;
                }
            }
        }
    }
}
