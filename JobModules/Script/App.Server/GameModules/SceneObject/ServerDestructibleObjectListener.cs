using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Configuration;
using App.Shared.GameModules.SceneObject;
using Core;
using App.Shared.SceneTriggerObject;
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

        public override void OnTriggerObjectLoaded(string id, GameObject gameObject)
        {
            _logger.DebugFormat("Destructible Object Loaded {0} {1}", id, gameObject.name);
            MapObjectEntityFactory.CreateDestructibleObject(id, gameObject, _detachCallback.OnDetach);
        }
    }
}
