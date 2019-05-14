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
            _detachCallback = new ClientFracturedChunkDetachCallback();
        }

        protected override void OnDeativeObjectActive(int id, MapObjectEntity mapObject, GameObject gameObject)
        {
            MapObjectUtility.AddRawGameObject<FracturedObject>(mapObject, gameObject);
            MapObjectUtility.RecordMapObj(id, (int) ETriggerObjectType.DestructibleObject, mapObject);
            MapObjectUtility.SendLastEvent(mapObject);
        }


        protected override void LoadTriggerObject(int id, GameObject gameObject)
        {
            MapObjectUtility.RecordGameObjId(gameObject, (int) ETriggerObjectType.DestructibleObject, id);
            MapObjectUtility.AttachRawObjToFracture(gameObject);
            MapObjectUtility.AddCallBack<FracturedObject>(gameObject, _detachCallback.OnDetach);
        }

        public override IEntity CreateMapObj(int id)
        {
            var gameObj = _objectManager.Get(id);
            if (gameObj == null) return null;
            var dest = (MapObjectEntity) MapObjectEntityFactory.CreateDestructibleObject(id, gameObj);
            MapObjectUtility.RecordMapObj(id, (int) _triggerType, dest);
            return dest;
        }
    }
}
