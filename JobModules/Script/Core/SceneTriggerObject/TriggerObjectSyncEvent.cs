using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.ObjectPool;

namespace Core.SceneTriggerObject
{
    public enum TriggerObjectSyncEventType
    {
        Undefined = 0,
        DetachChunk = 1,
        BreakChunk = 2,
    }

    public abstract class TriggerObjectSyncEvent : BaseRefCounter
    {
        public TriggerObjectSyncEventType EType;
        public int SourceObjectId;
    }

    public class ChunkSyncEvent : TriggerObjectSyncEvent
    {
        public int ChunkId;

        public static ChunkSyncEvent Allocate()
        {
            return ObjectAllocatorHolder<ChunkSyncEvent>.Allocate();
        }

        protected override void OnCleanUp()
        {
            EType = TriggerObjectSyncEventType.Undefined;
            ObjectAllocatorHolder<ChunkSyncEvent>.Free(this);
        }
    }
}
