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
        protected ITriggerObjectManager _objectManager;
        protected ETriggerObjectType _triggerType;
        
        protected ServerMapTriggerObjectListener(Contexts contexts, ETriggerObjectType triggerType, IMatcher<MapObjectEntity> mapMatcher)
        {
            var triggerManager = SingletonManager.Get<TriggerObjectManager>();
            triggerManager.RegisterListener(triggerType, this);
            _objectManager = triggerManager.GetManager(triggerType);
            _triggerType = triggerType;
            MapObjectEntityFactory= contexts.session.entityFactoryObject.MapObjectEntityFactory;
            _mapObjGroup = contexts.mapObject.GetGroup(mapMatcher);
        }

        public abstract IEntity CreateMapObj(int id);

        public abstract void OnTriggerObjectLoaded(int id, GameObject gameObject);

        public void OnTriggerObjectUnloaded(int id)
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
