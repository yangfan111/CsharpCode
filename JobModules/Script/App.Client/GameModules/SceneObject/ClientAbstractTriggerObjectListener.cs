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
        protected TriggerObjectManager _triggerObjectManager;
        
        protected ClientAbstractTriggerObjectListener(Contexts contexts, ETriggerObjectType triggerType,
            IMatcher<MapObjectEntity> activeMapMatcher, IMatcher<MapObjectEntity> deactiveMapMatcher)
        {
            _triggerType = triggerType;
            _contexts = contexts;
            
            _triggerObjectManager = SingletonManager.Get<TriggerObjectManager>();
            _triggerObjectManager.RegisterListener(triggerType, this);
            _objectManager = _triggerObjectManager.GetManager(triggerType);  
          
            var mapContext = contexts.mapObject;
            _activeMapObjects = mapContext.GetGroup(activeMapMatcher);
            _deactiveMapObjects = mapContext.GetGroup(deactiveMapMatcher);
            
            MapObjectEntityFactory = contexts.session.entityFactoryObject.MapObjectEntityFactory;
            _detachCallback = new ClientFracturedChunkDetachCallback();   

        }
        
        protected GameObject GetTriggerObject(int id)
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
                    OnDeativeObjectActive(id, obj, go);
                }
            }

            if (_deactiveMapObjects.Any())
            {
                _logger.DebugFormat("Waiting For Trigger Object Loading: Type {0} Count {1}", _triggerType, _deactiveMapObjects.Count());
            }
        }

        public void OnTriggerObjectLoaded(int id, GameObject gameObject)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("Trigger Object: Type {0} id {1} Loaded Self Position {2}.", _triggerType, id, _contexts.player.flagSelfEntity.position.Value);
            }
            
            if (SharedConfig.IsOffline)
            {
                LoadTriggerObject(id, gameObject);
            }
            else
            {
                for (int k = 0; k < (int)ETriggerObjectType.MaxCount; k++)
                {
                    var idset =_objectManager.GetAllId();
                    foreach (var item in idset)
                    {
                        if (item == id)
                        {
                            LoadTriggerObject(id, gameObject);
                        }
                    }
                }
            }
        }

        public void OnTriggerObjectUnloaded(int id)
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

        protected abstract void OnDeativeObjectActive(int id, MapObjectEntity mapObject, GameObject gameObject);
        protected abstract void LoadTriggerObject(int id, GameObject gameObject);
        public abstract IEntity CreateMapObj(int id);
    }
}
