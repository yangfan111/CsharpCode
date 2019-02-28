using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.Components;
using App.Shared.Configuration;
using App.Shared.GameModules.SceneObject;
using App.Shared.Util;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityLayerMask;
using Core;
using Core.GameModule.Interface;
using App.Shared.SceneTriggerObject;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;
using UserInputManager.Lib;
using UserInputManager.Utility;
using Utils.Singleton;

namespace App.Client.GameModules.SceneObject
{
    public class ClientDoorListener : ClientAbstractTriggerObjectListener
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(ClientDoorListener));

        private ClientFracturedChunkDetachCallback _detachCallback;
        public ClientDoorListener(Contexts contexts) : base(contexts, ETriggerObjectType.Door,
            MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.DoorData,
                MapObjectMatcher.DestructibleData,
                MapObjectMatcher.RawGameObject),
            MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                    MapObjectMatcher.TriggerObjectId,
                    MapObjectMatcher.DoorData,
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
            var target = AddRaycastTarget(id, gameObject);
            var door = (MapObjectEntity)MapObjectEntityFactory.CreateDoor(id, gameObject, _detachCallback.OnDetach);
            DoorCastData.Make(target, door.entityKey.Value.EntityId);
        }

        protected override void OnlineTriggerObjectLoad(MapObjectEntity mapObject, GameObject gameObject)
        {
            LinkGameObjectToSceneObject(mapObject, gameObject);
        }

        private void LinkGameObjectToSceneObject(MapObjectEntity mapObject, GameObject gameObject)
        {
            var target = gameObject.GetComponentInChildren<RayCastTarget>();
            if (target == null)
            {
                target = AddRaycastTarget(mapObject.triggerObjectId.Id, gameObject);
            }
            MapObjectUtility.AddRawGameObject<FracturedObject>(mapObject, gameObject, _detachCallback.OnDetach);
            DoorCastData.Make(target, mapObject.entityKey.Value.EntityId);

            InitRotation(mapObject, gameObject);
        }

        private void InitRotation(MapObjectEntity mapObject, GameObject gameObject)
        {
            var rot = mapObject.doorData.Rotation;
            var eulerAngles = gameObject.transform.localEulerAngles;
            if (!rot.Equals(eulerAngles.y))
            {
                eulerAngles.y = rot;
                gameObject.transform.localEulerAngles = eulerAngles;
            }
        }


        protected  RayCastTarget AddRaycastTarget(string id, GameObject gameObject)
        {
            var colliders = gameObject.GetComponentsInChildren<Collider>();
            var colliderCount = colliders.Length;
            bool noInputCastCollider = true;
            RayCastTarget target = null;
            for(int i = 0; i < colliderCount;++i)
            {
                var collider = colliders[i];
                if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.UserInputRaycast))
                {
                    target = RayCastTargetUtil.AddRayCastTarget(collider.gameObject); 
                    noInputCastCollider = false;
                }
            }

            if (noInputCastCollider)
            {
                _logger.ErrorFormat("Door {0} has no inputraycast collider.", gameObject.name);
            }
            return target;
        }

    }
}
