﻿using System;
using System.Collections.Generic;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Components;
using App.Shared.GameModules.Common;
using App.Shared.Util;
using Assets.XmlConfig;
using com.wd.free.para;
using Core;
using Core;
using Core.EntityComponent;
using Core.GameTime;
using Core.IFactory;
using Core.SceneTriggerObject;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;

namespace App.Shared.EntityFactory
{
    public class ServerMapObjectEntityFactory:IMapObjectEntityFactory
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(ServerMapObjectEntityFactory));

        private readonly MapObjectContext _mapObjectContext;
        protected readonly IEntityIdGenerator _idGenerator;
        
        public ServerMapObjectEntityFactory(MapObjectContext mapObjectContext,IEntityIdGenerator entityIdGenerator)
        {
            _mapObjectContext = mapObjectContext;
            _idGenerator = entityIdGenerator;
        }

        public virtual IEntity CreateDoor(int objectId, GameObject gameObject)
        {
            return CreateDoorInternal(objectId, gameObject, ResetDoor);
        }
        
        protected IEntity CreateDoorInternal(int objectId,
            GameObject gameObject, Action<Entity> resetAction)
        {
            var door = (MapObjectEntity)CreateDestructibleObjectInternal(objectId, gameObject, resetAction, false);
            var initRotation = gameObject.transform.localEulerAngles.y;
            door.AddDoorData(initRotation, initRotation);
            return door;
        }

        protected MapObjectEntity CreateMapObject(int entityId, int objectId,
            GameObject gameObject, Action<Entity> resetAction)
        {
            var obj = _mapObjectContext.CreateEntity();
            obj.AddEntityKey(new EntityKey(entityId, (short) EEntityType.MapObject));
            obj.AddPosition();
            obj.position.Value = gameObject.transform.position;
            obj.isFlagSyncNonSelf = true;
            obj.AddTriggerObjectId(objectId);
            obj.AddRawGameObject(gameObject);
            obj.AddFlagImmutability(0);
            if (resetAction != null)
            {
                obj.AddReset(resetAction);
            }
            return obj;
        }
        
        private static void AddEntityReference(IEntity mapObject, GameObject gameObject)
        {
            var entityReference = gameObject.GetComponent<EntityReference>();
            if (entityReference == null)
            {
                entityReference = gameObject.AddComponentUncheckRequireAndDisallowMulti<EntityReference>();
            }
            var mapObj = mapObject as MapObjectEntity;
            if (mapObj != null)
                entityReference.Init(mapObj.entityAdapter);
        }
        
        private void ResetDoor(Entity entity)
        {
            var door = (MapObjectEntity) entity;
            if (!door.hasTriggerObjectId || !door.hasDoorData || !door.hasRawGameObject)
            {
                _logger.ErrorFormat("Can not reset door {0}", door.entityKey.Value.EntityId);
                return;
            }

            ResetDestructibleObject(door);

            door.doorData.Reset();
            //此处使用0因为调用reset是在前一局结束的时候，servertime是上一局的时间
            door.ReplaceFlagImmutability(0);
            if (null != door.rawGameObject.Value)
            {
                var eulerAngles = door.rawGameObject.Value.transform.localEulerAngles;
                eulerAngles.y = door.doorData.InitialRotation;
                door.rawGameObject.Value.transform.localEulerAngles = eulerAngles;
            }
            else
            {
                _logger.ErrorFormat("raw gameobject is null in {0}", door.entityKey.Value);
            }
        }

        private void ResetDestructibleObject(Entity entity)
        {
            var obj = (MapObjectEntity) entity;
            if (!obj.hasTriggerObjectId || !obj.hasDestructibleData || !obj.hasRawGameObject)
            {
                _logger.ErrorFormat("Can not reset destructible object {0}", obj.entityKey.Value.EntityId);
                return;
            }

            obj.destructibleData.ResetOnAction();

            var gameObject = obj.rawGameObject.Value;
            if (null != gameObject)
            {
                var fracturedObject = gameObject.GetComponent<FracturedObject>();
                if (fracturedObject != null)
                {
                    fracturedObject.ResetChunks();
                }
            }
            else
            {
                _logger.ErrorFormat("raw gameobject is null in {0}", obj.entityKey.Value);
            }
        }
 
        public virtual IEntity CreateDestructibleObject(int objectId,
            GameObject gameObject)
        {
            return CreateDestructibleObjectInternal(objectId, gameObject.gameObject, ResetDestructibleObject, true);
        }

        protected IEntity CreateDestructibleObjectInternal(int objectId,
            GameObject gameObject, Action<Entity> resetAction,
            bool isDestructibleObject)
        {
            
            var destructibleObject = CreateMapObject(objectId, objectId, gameObject, resetAction);
            AddEntityReference(destructibleObject, gameObject);
            var fracturedObject = gameObject.GetComponent<FracturedObject>();
            if (fracturedObject != null)
            {
                destructibleObject.AddDestructibleData(fracturedObject.StartAsWhole);
                fracturedObject.Owner = gameObject;
                fracturedObject.SetAsServerObject(SharedConfig.IsServer);
            }
            else
            {
                destructibleObject.AddDestructibleData(true);
            }

            destructibleObject.AddTriggerObjectEvent(new Queue<TriggerObjectSyncEvent>());

            //distinguish door object from a simple destructible object.
            if (isDestructibleObject)
            {
                destructibleObject.isDestructibleObjectFlag = true;
            }

            MapObjectUtility.AttachRawObjToFracture(gameObject);

            return destructibleObject;
        }


        public IEntity CreateGlassyObject(int objectId, GameObject gameObject)
        {
            var glassyObject = CreateSceneObject(_idGenerator.GetNextEntityId(), objectId, gameObject, ResetGlassyObject);
            AddEntityReference(glassyObject, gameObject);
            var fracturedGlassyObject = gameObject.GetComponent<FracturedGlassyObject>();
            if (fracturedGlassyObject != null)
            {
                fracturedGlassyObject.SetAsServerObject(SharedConfig.IsServer);
                fracturedGlassyObject.Owner = gameObject;
            }
            glassyObject.AddTriggerObjectEvent(new Queue<TriggerObjectSyncEvent>());
            glassyObject.AddGlassyData();

            MapObjectUtility.AttachRawObjToFracture(gameObject);
            return glassyObject;
        }

        private MapObjectEntity CreateSceneObject(int entityId, int objectId,
            GameObject gameObject, Action<Entity> resetAction)
        {
            var obj = _mapObjectContext.CreateEntity();
            obj.AddEntityKey(new EntityKey(entityId, (short) EEntityType.MapObject));
            obj.AddPosition();
            obj.position.Value = gameObject.transform.position;
            obj.isFlagSyncNonSelf = true;
            obj.AddTriggerObjectId(objectId);
            obj.AddRawGameObject(gameObject);
            obj.AddFlagImmutability(0);

            if (resetAction != null)
            {
                obj.AddReset(resetAction);
            }

            return obj;
        }

        private void ResetGlassyObject(Entity entity)
        {
            var glassyObj = (MapObjectEntity)entity;

            if (!glassyObj.hasTriggerObjectId || !glassyObj.hasGlassyData || !glassyObj.hasRawGameObject)
            {
                _logger.ErrorFormat("Can not reset glassy object {0}", glassyObj.entityKey.Value.EntityId);
                return;
            }

            var go = glassyObj.rawGameObject.Value;
            if (go == null)
            {
                _logger.ErrorFormat("The game object for glassy obejct {0} is null!", glassyObj);
            }
            else
            {
                var fracturedGlassyObject = go.GetComponent<FracturedGlassyObject>();
                if (fracturedGlassyObject != null)
                    fracturedGlassyObject.ResetChunks();
            }
           
            glassyObj.glassyData.Reset();
        }

    }
}