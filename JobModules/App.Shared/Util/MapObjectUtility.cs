using System;
using System.Collections.Generic;
using App.Shared.GameModules.Common;
using Core.EntityComponent;
using Core.SceneTriggerObject;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;

namespace App.Shared.Util
{
    public static class MapObjectUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SceneObjectUtility));
        
        public static void AddRawGameObject<T>(IEntity obj, GameObject go,
            Action<object> detachCallback) where T : FracturedBaseObject
        {
            AddRawGameObject(obj, go);

            var fracturedObject = go.GetComponent<T>();
            if (fracturedObject != null)
            {
                fracturedObject.EventDetachCallback = detachCallback;
            }
        }

        private static void AddRawGameObject(IEntity obj, GameObject gameObject)
        {
            MapObjectEntity mapObject = obj as MapObjectEntity;
            mapObject.AddRawGameObject(gameObject);
            var entityReference = gameObject.GetComponent<EntityReference>();
            if (entityReference == null)
            {
                entityReference = gameObject.AddComponent<EntityReference>();
            }
            entityReference.Init(mapObject.entityAdapter);
        }
        
        public static void SendTriggerObjectEventToServer(MapObjectEntity o,
            TriggerObjectSyncEvent evt)
        {
            var mapObject = (MapObjectEntity)o;
            if (!mapObject.hasTriggerObjectEvent)
            {
                mapObject.AddTriggerObjectEvent(new Queue<TriggerObjectSyncEvent>());
            }

            mapObject.triggerObjectEvent.SyncEvents.Enqueue(evt);
            if (!mapObject.isTriggerObjectEventFlag)
            {
                mapObject.isTriggerObjectEventFlag = true;
            }
        }
        
        public static MapObjectEntity GetMapObjectOfFracturedHittable(FracturedHittable fracturedHittable)
        {
            var testChunkAdpater = fracturedHittable as FracturedObjectChunkAdapter;
            MapObjectEntity result = null;
            if (testChunkAdpater != null && testChunkAdpater.FracturedObjectSource != null)
            {
                return GetMapObjectByGameObject(testChunkAdpater.FracturedObjectSource.gameObject);
            }

            var testFracturedChunk = fracturedHittable as FracturedChunk;
            if (testFracturedChunk != null)
            {
                return GetMapObjectOfFracturedChunk(testFracturedChunk);
            }

            var testFracturedGlassyChunk = fracturedHittable as FracturedGlassyChunk;
            if (testFracturedGlassyChunk != null)
            {
                return GetMapObjectOfFracturedChunk(testFracturedGlassyChunk);
            }
            
            var testFracturedObject = fracturedHittable as FracturedObject;
            if (testFracturedObject != null)
            {
                return GetMapObjectByGameObject(testFracturedObject.gameObject);
            }

            var testGlassyChunkAdapter = fracturedHittable as FracturedGlassyChunkAdapter;
            if (testGlassyChunkAdapter != null)
            {
                return GetMapObjectByGameObject(testGlassyChunkAdapter.FracturedObjectSource.gameObject);
            }

            return null;
        }

        public static MapObjectEntity GetMapObjectByGameObject(GameObject gameObject)
        {
            var  entityReference = gameObject.GetComponent<EntityReference>();
            if (entityReference == null)
            {
                _logger.ErrorFormat("Can not find Destructible Object Entity for FracturedObject {0}",
                    gameObject.name);
                return null;
            }

            var destructibleObject = entityReference.Reference as MapObjectEntity;
            if (destructibleObject == null)
            {
                _logger.ErrorFormat("Entity Reference is Null for FracturedObject {0}",
                    gameObject.name);
                return null;
            }
            return destructibleObject;
        }

        public static MapObjectEntity GetMapObjectOfFracturedChunk(FracturedChunk chunk)
        {
            var mapObject = GetMapObjectOfFracturedChunk<FracturedChunk, FracturedObject>(chunk);
            return mapObject != null && mapObject.hasDestructibleData ? mapObject : null;
        }

        public static MapObjectEntity GetMapObjectOfFracturedChunk(FracturedGlassyChunk chunk)
        {
            var mapObject = GetMapObjectOfFracturedChunk<FracturedGlassyChunk, FracturedGlassyObject>(chunk);

            return mapObject != null && mapObject.hasGlassyData ? mapObject : null;
        }
        
        private static MapObjectEntity GetMapObjectOfFracturedChunk<TChunk, TObject>(TChunk chunk) 
            where TChunk: FracturedBaseChunk<TObject>
            where TObject: FracturedBaseObject
        {
            var fracturedObjectSource = chunk.FracturedObjectSource;
          
            var mapObject = GetMapObjectByGameObject(fracturedObjectSource.gameObject);
            if (mapObject == null)
            {
                _logger.ErrorFormat("Can not find Destructible Object Entity for FracturedObject {0}, ChunkId {1}",
                    fracturedObjectSource.name, chunk.ChunkId);
            }

            return mapObject;
        }

    }
}