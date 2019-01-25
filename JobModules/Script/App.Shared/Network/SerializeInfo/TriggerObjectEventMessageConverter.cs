using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using Core.SceneTriggerObject;

namespace App.Shared.Network
{
    public class TriggerObjectEventMessageConverter
    {
        public static TriggerObjectSyncEvent FromProtoBuf(TriggerObjectEventMessage message)
        {
            switch (message.EventType)
            {
                case (int) TriggerObjectSyncEventType.DetachChunk:
                case (int)TriggerObjectSyncEventType.BreakChunk:
                {
                    var detachChunkEvent = ChunkSyncEvent.Allocate();
                    detachChunkEvent.EType = (TriggerObjectSyncEventType) message.EventType;
                    detachChunkEvent.SourceObjectId = message.SourceId;
                    detachChunkEvent.ChunkId = message.Ints[0];
                    return detachChunkEvent;
                }
                default:
                    throw new Exception("Unkown TriggerObjectSyncEventType : " + message.EventType);
            }
        }

        public static void ToProtoBuf(TriggerObjectEventMessage msg, TriggerObjectSyncEvent syncEvent)
        {
            switch (syncEvent.EType)
            {
                case TriggerObjectSyncEventType.DetachChunk:
                case TriggerObjectSyncEventType.BreakChunk:
                {
                    msg.EventType = (int)syncEvent.EType;
                    msg.SourceId = syncEvent.SourceObjectId;
                    var detachChunkEvent = (ChunkSyncEvent)syncEvent;
                    msg.Ints.Add(detachChunkEvent.ChunkId);
                    break;
                }
                default:
                    throw new Exception("Unkown TriggerObjectSyncEvent Type : " + syncEvent.EType);
            }
        }
    }
}
