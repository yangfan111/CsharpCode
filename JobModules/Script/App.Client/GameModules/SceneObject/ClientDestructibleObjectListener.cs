using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.Components;
using App.Shared.Configuration;
using App.Shared.GameModules.SceneObject;
using App.Shared.SceneManagement;
using App.Shared.Util;
using App.Shared.SceneTriggerObject;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.SceneObject
{
    public class ClientDestructibleObjectListener : ClientAbstractTriggerObjectListener
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(ClientDestructibleObjectListener));
        private ClientFracturedChunkDetachCallback _detachCallback;
        public ClientDestructibleObjectListener(Contexts contexts): base(contexts, ETriggerObjectType.DestructibleObject,
            MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.DestructibleObjectFlag,
                MapObjectMatcher.DestructibleData,
                MapObjectMatcher.RawGameObject),
            MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                    MapObjectMatcher.TriggerObjectId,
                    MapObjectMatcher.DestructibleObjectFlag,
                    MapObjectMatcher.DestructibleData)
                .NoneOf(MapObjectMatcher.RawGameObject))
        {
            _detachCallback = new ClientFracturedChunkDetachCallback(contexts);
        }

        protected override void OnDeativeObjectActive(MapObjectEntity mapObject, GameObject gameObject)
        {
            LinkGameObjectToSceneObject(mapObject, gameObject);
        }

        protected override void OfflineTriggerObjectLoad(string id, GameObject gameObject)
        {
            MapObjectEntityFactory.CreateDestructibleObject(id, gameObject, _detachCallback.OnDetach);
        }

        protected override void OnlineTriggerObjectLoad(MapObjectEntity mapObject, GameObject gameObject)
        {
            LinkGameObjectToSceneObject(mapObject, gameObject);
        }

        private void LinkGameObjectToSceneObject(MapObjectEntity sceneObject, GameObject gameObject)
        {
            MapObjectUtility.AddRawGameObject<FracturedObject>(sceneObject, gameObject, _detachCallback.OnDetach);
        }
    }
}
