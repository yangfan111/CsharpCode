using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Configuration;
using App.Shared.GameModules.SceneObject;
using Core;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;

namespace App.Server.GameModules.SceneObject
{
    class ServerDestructibleObjectListener : ServerMapTriggerObjectListener
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerDestructibleObjectListener));

        private ServerFracturedChunkDetachCallback _detachCallback;
        public ServerDestructibleObjectListener(Contexts contexts) : base(contexts, ETriggerObjectType.DestructibleObject,
            MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.DestructibleObjectFlag,
                MapObjectMatcher.DestructibleData))
        {
            _detachCallback = new ServerFracturedChunkDetachCallback(contexts);
        }

        public override IEntity CreateMapObj(int id)
        {
            var gameObj = _objectManager.Get(id);
            if (gameObj == null) return null;
            var obj = (MapObjectEntity)MapObjectEntityFactory.CreateDestructibleObject(id, gameObj);
            MapObjectUtility.RecordMapObj(id, (int)_triggerType, obj);
            MapObjectUtility.FetchFractruedState(gameObj);
            return obj;
        }

        public override void OnTriggerObjectLoaded(int id, GameObject gameObject)
        {
            _logger.DebugFormat("Destructible Object Loaded {0} {1}", id, gameObject.name);
            MapObjectUtility.RecordGameObjId(gameObject, (int)_triggerType, id);
            MapObjectUtility.AttachRawObjToFracture(gameObject);
            MapObjectUtility.AddCallBack<FracturedObject>(gameObject, _detachCallback.OnDetach);
            MapObjectUtility.AddFracturedRecorder(gameObject);
        }
    }
}
