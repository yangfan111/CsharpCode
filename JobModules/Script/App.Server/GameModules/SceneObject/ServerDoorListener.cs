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
using Core.Utils;
using Entitas;
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

        public override void OnTriggerObjectLoaded(string id, GameObject gameObject)
        {
            _logger.DebugFormat("Door Loaded {0}", id);
            MapObjectEntityFactory.CreateDoor(id, gameObject, _detachCallback.OnDetach);
        }
    }
}
