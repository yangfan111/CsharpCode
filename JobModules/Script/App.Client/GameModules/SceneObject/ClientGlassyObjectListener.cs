using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.Configuration;
using App.Shared.Util;
using App.Shared.SceneTriggerObject;
using Core.Utils;
using UltimateFracturing;
using UnityEngine;
using Core.SceneTriggerObject;
using Utils.Singleton;

namespace App.Client.GameModules.SceneObject
{
    public class ClientGlassyObjectListener : ClientAbstractTriggerObjectListener
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientGlassyObjectListener));

        public ClientGlassyObjectListener(Contexts contexts): base(contexts, ETriggerObjectType.GlassyObject,
            MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                MapObjectMatcher.TriggerObjectId,
                MapObjectMatcher.GlassyData,
                MapObjectMatcher.RawGameObject),
            MapObjectMatcher.AllOf(MapObjectMatcher.EntityKey,
                    MapObjectMatcher.TriggerObjectId,
                    MapObjectMatcher.GlassyData)
                .NoneOf(MapObjectMatcher.RawGameObject))
        {
            
        }

        protected override void OnDeativeObjectActive(MapObjectEntity mapObject, GameObject gameObject)
        {
            LinkGameObjectToSceneObject(mapObject, gameObject);
        }

        protected override void OfflineTriggerObjectLoad(string id, GameObject gameObject)
        {
            MapObjectEntityFactory.CreateGlassyObject(id, gameObject, OnChunkBroken);
        }

        protected override void OnlineTriggerObjectLoad(MapObjectEntity mapObject, GameObject gameObject)
        {
            LinkGameObjectToSceneObject(mapObject, gameObject);
        }

        protected void LinkGameObjectToSceneObject(MapObjectEntity mapObject, GameObject gameObject)
        {
            MapObjectUtility.AddRawGameObject<FracturedGlassyObject>(mapObject, gameObject, OnChunkBroken);

            var glassyData = mapObject.glassyData;
            var fo = gameObject.GetComponent<FracturedGlassyObject>();
            if (fo != null && glassyData.HasAnyChunkBroken())
            {
                var chunkId = glassyData.GetNextBrokenChunkId();
                while (chunkId >= 0)
                {
                    fo.PutToBrokenState(chunkId);
                    chunkId = glassyData.GetNextBrokenChunkId(chunkId);
                }
            }
        }

        private void OnChunkBroken(object o)
        {
            var chunk = (FracturedGlassyChunk) o;
            var mapObject = MapObjectUtility.GetMapObjectOfFracturedChunk(chunk);

            if (mapObject != null && !SharedConfig.IsOffline)
            {
                //send to server only if the corresponding chunk is not broken on server-side.
                if (!mapObject.glassyData.IsBroken(chunk.ChunkId))
                {
                    var evt = ChunkSyncEvent.Allocate();
                    evt.EType = TriggerObjectSyncEventType.BreakChunk;
                    evt.SourceObjectId = mapObject.entityKey.Value.EntityId;
                    evt.ChunkId = chunk.ChunkId;

                    MapObjectUtility.SendTriggerObjectEventToServer(mapObject, evt);
                }
                
            }
        }
    }
}
