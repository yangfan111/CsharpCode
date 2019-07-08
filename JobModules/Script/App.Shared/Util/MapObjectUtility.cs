using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using App.Shared.GameModules.Common;
using App.Shared.Player.Events;
using App.Shared.SceneTriggerObject;
using Core.Event;
using Core.SceneTriggerObject;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;
using UnityEngineInternal;

namespace App.Shared.Util
{
    public static class MapObjectUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(MapObjectUtility));
        
        private static MapObjectRecorder _mapObjectRecorder = new MapObjectRecorder();
        
        private static RawMapObjectIdRecorder _gameObjIdRecorder = new RawMapObjectIdRecorder();
        
        public static void AddRawGameObject<T>(IEntity obj, GameObject go) where T : FracturedBaseObject
        {
            MapObjectEntity mapObject = obj as MapObjectEntity;
            mapObject.AddRawGameObject(go);
            var entityReference = go.GetComponent<EntityReference>();
            if (entityReference == null)
            {
                entityReference = go.AddComponentUncheckRequireAndDisallowMulti<EntityReference>();
            }
            entityReference.Init(mapObject.entityAdapter);
        }

        public static void FetchFractruedState(GameObject gameObj)
        {
            var mapObj = GetMapObjectByGameObject(gameObj);
            if (mapObj == null) return;
            var glassObj = gameObj.GetComponent<FracturedGlassyObject>();
            if (glassObj != null)
            {
                foreach (var chunk in glassObj.Chunks)
                {
                    if (chunk.IsBroken())
                    {
                        mapObj.glassyData.SetBroken(chunk.ChunkId);
                    }
                }
            }

            var fracObj = gameObj.GetComponent<FracturedObject>();
            if (fracObj != null)
            {
                foreach (var frac in fracObj.ListFracturedChunks)
                {
                    if (frac.IsBroken())
                    {
                        mapObj.destructibleData.SetDestruction(frac.ChunkId);
                    }
                }
                mapObj.destructibleData.LastSyncDestructionState = mapObj.destructibleData.DestructionState;
            }
            

        }

        public static void AddCallBack<T>(GameObject go, Action<object> callback) where T : FracturedBaseObject
        {
            var fracturedObj = go.GetComponent<T>();
            if (fracturedObj != null)
            {
                fracturedObj.EventDetachCallback = callback;
            }
        }

        public static void AttachRawObjToFracture(GameObject go)
        {
            AttachRawObj(go.transform, go);
        }

        public static void AddFracturedRecorder(GameObject gameObject)
        {
            var boxcollider = gameObject.GetComponentInChildren<BoxCollider>();
            if (boxcollider != null && boxcollider.gameObject.GetComponent<FractureObjRecorder>()==null)
            {
                var fracObj = boxcollider.gameObject.AddComponentUncheckRequireAndDisallowMulti<FractureObjRecorder>();
                fracObj.owner = gameObject;
            }
        }

        private static void AttachRawObj(Transform trans, GameObject owner)
        {
            for(int i=0;i<trans.childCount;i++)
            {
                var sub = trans.GetChild(i);
                var fracturedObject = sub.GetComponent<FracturedHittable>();
                if (fracturedObject != null)
                    fracturedObject.Owner = owner;
                AttachRawObj(sub, owner);
            }
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

        private static Dictionary<GameObject, List<TriggerObjectSyncEvent>> _triggerEvent = new Dictionary<GameObject, List<TriggerObjectSyncEvent>>();
        public static void StoreTriggerObjectEvent(GameObject obj, TriggerObjectSyncEvent evt)
        {
            if (!_triggerEvent.ContainsKey(obj))
                _triggerEvent.Add(obj, new List<TriggerObjectSyncEvent>());
            _triggerEvent[obj].Add(evt);
        }

        public static void SendLastEvent(MapObjectEntity entity)
        {
            var rawObj = entity.rawGameObject.Value;
            if (!_triggerEvent.ContainsKey(rawObj)) return;
            foreach (var evt in _triggerEvent[rawObj])
            {
                evt.SourceObjectId = entity.entityKey.Value.EntityId;
                SendTriggerObjectEventToServer(entity, evt);
            }
            _triggerEvent[rawObj].Clear();
            _triggerEvent.Remove(rawObj);
        }
        
        public static MapObjectEntity GetMapObjectByGameObject(GameObject gameObject)
        {
            var  entityReference = gameObject.GetComponent<EntityReference>();
            if (entityReference == null)
            {
                _logger.DebugFormat("gameObj {0} do not have entityReference",gameObject.name);
                return null;
            }

            var destructibleObject = entityReference.Reference as MapObjectEntity;
            if (destructibleObject == null)
            {
                _logger.DebugFormat("Entity Reference is Null for FracturedObject {0}",
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
            return GetMapObjectByGameObject(fracturedObjectSource.gameObject);
        }

        public static void RecordMapObj(int id, int type, MapObjectEntity entity)
        {
            _mapObjectRecorder.Add(id, type, entity);
        }

        public static MapObjectEntity GetMapObjByRawId(int id, int type)
        {
            return _mapObjectRecorder.Get(id, type);
        }

        public static void RecordGameObjId(GameObject obj, int type, int id)
        {
            _gameObjIdRecorder.RecordId(obj, type, id);
        }

        public static void DeleteRecord(int id)
        {
            var rawObj = _gameObjIdRecorder.GetObj(id);
            var type = _gameObjIdRecorder.GetType(rawObj);
            _mapObjectRecorder.Delete(id, type);
            _gameObjIdRecorder.RemoveRecord(id);
        }

        public static int GetGameObjId(GameObject obj)
        {
            return _gameObjIdRecorder.GetId(obj);
        }

        public static int GetGameObjType(GameObject obj)
        {
            return _gameObjIdRecorder.GetType(obj);
        }

        public static void SendCreateMapObjMsg(int type, int id)
        {
            var player = Contexts.sharedInstance.player.flagSelfEntity;
            var evt = EventInfos.Instance.Allocate(EEventType.CreateMapObj, false) as CreateMapObjEvent;
            evt.Type = type;
            evt.Id = id;
            player.uploadEvents.Events.AddEvent(evt);
        }

        public static void StoreCreateMapObjMsg(int type, int id, PlayerEntity ownerPlayer = null)
        {
            var player = ownerPlayer ?? Contexts.sharedInstance.player.flagSelfEntity;
            var evt = EventInfos.Instance.Allocate(EEventType.CreateMapObj, false) as CreateMapObjEvent;
            evt.Type = type;
            evt.Id = id;
            player.uploadEvents.Events.AddEvent(evt);
            _logger.InfoFormat("StoreCreateMapObjMsg: {0}", evt);
        }
    }
}