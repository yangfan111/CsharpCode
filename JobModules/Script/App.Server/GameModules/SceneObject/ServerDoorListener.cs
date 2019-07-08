using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Components.SceneObject;
using App.Shared.Configuration;
using App.Shared.GameModules.SceneObject;
using Core;
using Core.GameModule.Interface;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;

namespace App.Server.GameModules.SceneObject
{
    public class ServerDoorListener: ServerMapTriggerObjectListener
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerDoorListener));

        private ServerFracturedChunkDetachCallback _detachCallback;
        
        public ServerDoorListener(Contexts contexts) : base(contexts, ETriggerObjectType.Door,
            MapObjectMatcher.AllOf(
                MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.DoorData))
        {
            _detachCallback = new ServerFracturedChunkDetachCallback(contexts);
        }

        public override IEntity CreateMapObj(int id)
        {
            var gameObj = _objectManager.Get(id);
            if (gameObj == null)
            {
                _logger.ErrorFormat("Can't create mapObj, because gameObj(id={0}) is not loaded", id);
                return null;
            }
            var door = (MapObjectEntity)MapObjectEntityFactory.CreateDoor(id, gameObj);
            MapObjectUtility.RecordMapObj(id, (int) _triggerType, door);
            MapObjectUtility.FetchFractruedState(gameObj);
            return door;
        }

        public override void OnTriggerObjectLoaded(int id, GameObject gameObject)
        {
            _logger.InfoFormat("Door Loaded {0}", id);

            MapObjectUtility.RecordGameObjId(gameObject, (int)_triggerType, id);
            MapObjectUtility.AttachRawObjToFracture(gameObject);
            MapObjectUtility.AddCallBack<FracturedObject>(gameObject, _detachCallback.OnDetach);
            MapObjectUtility.AddFracturedRecorder(gameObject);
        }
    }
}
