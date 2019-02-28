using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.Configuration;
using App.Shared.SceneManagement;
using App.Shared.Util;
using Core;
using App.Shared.SceneTriggerObject;
using Core.IFactory;
using Core.Utils;
using Entitas;
using log4net.Util;
using UltimateFracturing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UserInputManager.Lib;
using Utils.Singleton;

namespace App.Client.GameModules.SceneObject
{
     
    public abstract class ClientAbstractTriggerObjectListener:ITriggerObjectListener
    {
        protected static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientAbstractTriggerObjectListener));
        private Contexts _contexts;
        protected ETriggerObjectType _triggerType;
        protected ITriggerObjectManager _objectManager;
        private IGroup<MapObjectEntity> _deactiveMapObjects;
        private IGroup<MapObjectEntity> _activeMapObjects;
        protected IMapObjectEntityFactory MapObjectEntityFactory;     
        private ClientFracturedChunkDetachCallback _detachCallback;
        
        protected ClientAbstractTriggerObjectListener(Contexts contexts, ETriggerObjectType triggerType,
            IMatcher<MapObjectEntity> activeMapMatcher, IMatcher<MapObjectEntity> deactiveMapMatcher)
        {
            _triggerType = triggerType;
            _contexts = contexts;
            var triggerObjectManager = SingletonManager.Get<TriggerObjectManager>();
            triggerObjectManager.RegisterListener(triggerType, this);
            _objectManager = triggerObjectManager.GetManager(triggerType);  
          
            var mapContext = contexts.mapObject;
            _activeMapObjects = mapContext.GetGroup(activeMapMatcher);
            _deactiveMapObjects = mapContext.GetGroup(deactiveMapMatcher);
            MapObjectEntityFactory = contexts.session.entityFactoryObject.MapObjectEntityFactory;
            _detachCallback = new ClientFracturedChunkDetachCallback(contexts);         
        }
        
        protected GameObject GetTriggerObject(string id)
        {
            return _objectManager.Get(id);
        }

        public void Update()
        {
            var objs = _deactiveMapObjects.GetEntities();
            for (int i = 0; i < objs.Length; ++i)
            {
                var obj = objs[i];
                var id = obj.triggerObjectId.Id;
                var go = GetTriggerObject(id);

                if (go != null)
                {
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.DebugFormat("Trigger Object: Type {0} id {1} Loaded On Update Self Position {2}.", _triggerType, id, _contexts.player.flagSelfEntity.position.Value);
                    }

                    OnDeativeObjectActive(obj, go);
                }
            }

            if (_deactiveMapObjects.Any())
            {
                _logger.DebugFormat("Waiting For Trigger Object Loading: Type {0} Count {1}", _triggerType, _deactiveMapObjects.Count());
            }
        }

        public void OnTriggerObjectLoaded(string id, GameObject gameObject)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("Trigger Object: Type {0} id {1} Loaded Self Position {2}.", _triggerType, id, _contexts.player.flagSelfEntity.position.Value);
            }

            if (SharedConfig.IsOffline)
            {
                OfflineTriggerObjectLoad(id, gameObject);
            }
            else
            {
                var objs = _deactiveMapObjects.GetEntities();
                for (int i = 0; i < objs.Length; ++i)
                {
                    var obj = objs[i];
                    if (obj.triggerObjectId.Id == id)
                    {
                        OnlineTriggerObjectLoad(obj, gameObject);
                        break;
                    }
                }
            }
        }

        public void OnTriggerObjectUnloaded(string id)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("Trigger Object: Type {0} id {1} Unloaded Self Position {2}.", _triggerType, id, _contexts.player.flagSelfEntity.position.Value);
            }
            
            var doors = _activeMapObjects.GetEntities();
            for (int i = 0; i < doors.Length; ++i)
            {
                var door = doors[i];
                if (door.triggerObjectId.Id == id)
                {
                    door.isFlagDestroy = true;
                    break;
                }
            }
        }
        
        protected abstract void OnDeativeObjectActive(MapObjectEntity mapObject, GameObject gameObject);
        protected abstract void OfflineTriggerObjectLoad(string id, GameObject gameObject);
        protected abstract void OnlineTriggerObjectLoad(MapObjectEntity mapObject, GameObject gameObject);
    }
}
