using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.SceneObject;
using App.Shared.GameModules.Vehicle;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core;
using Core.SceneTriggerObject;
using Core.Utils;
using UltimateFracturing;
using UnityEngine;

namespace App.Client.GameModules.SceneObject
{
    public class ClientFracturedChunkDetachCallback
    {
        private static readonly float SYNC_DELAY_TIME = 1f;
        private LoggerAdapter _logger = new LoggerAdapter(typeof(ClientFracturedChunkDetachCallback));

        public void OnDetach(object o)
        {
            FracturedChunk chunk = o as FracturedChunk;
            var destructibleObject = MapObjectUtility.GetMapObjectOfFracturedChunk(chunk);
            if (destructibleObject == null)
            {
                var evt = ChunkSyncEvent.Allocate();
                evt.EType = TriggerObjectSyncEventType.DetachChunk;
                evt.ChunkId = chunk.ChunkId;
                MapObjectUtility.StoreTriggerObjectEvent(chunk.FracturedObjectSource.gameObject, evt);
            }
            if (destructibleObject != null)
            {
                var data = destructibleObject.destructibleData;
                var chunkId = chunk.ChunkId;
                if (data.IsInDestructionState(chunkId))
                {
                    return;
                }

                data.SetDestruction(chunk.ChunkId);

                //synchronization delay for vehicle-collidable destructible object
                if (chunk.FracturedObjectSource.EnableVehicleCollision)
                {
                    data.SyncDelay = SYNC_DELAY_TIME;
                }

                 //notify server to destory destructible object
                 SendDetachChunkEventToServer(destructibleObject, chunk.ChunkId);
            }
        }

        private void SendDetachChunkEventToServer(MapObjectEntity destructibleObject, int chunkId)
        {
            var evt = ChunkSyncEvent.Allocate();
            evt.EType = TriggerObjectSyncEventType.DetachChunk;
            evt.SourceObjectId = destructibleObject.entityKey.Value.EntityId;
            evt.ChunkId = chunkId;

            MapObjectUtility.SendTriggerObjectEventToServer(destructibleObject, evt);
        }
    }
}