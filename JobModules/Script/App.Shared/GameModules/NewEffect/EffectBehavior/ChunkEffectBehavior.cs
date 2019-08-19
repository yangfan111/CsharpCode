using System.Collections.Generic;
using App.Shared.Util;
using Core.ObjectPool;
using UnityEngine;

namespace App.Shared
{
     public class ChunkEffectBehavior : NormalEffectBehavior
        {
            private int chunckId;
            public static readonly  LinkedList<ChunkEffectBehavior> ChunkEffectBehaviors = new LinkedList<ChunkEffectBehavior>();
            private static List<ChunkEffectBehavior> findRetList = new List<ChunkEffectBehavior>();
            public  void Initialize(Vector3 normal, Vector3 position, int audioClientEffectArg1, int audioClientEffectArg2,
                                            AudioClientEffectType audioClientEffectType,int chunckId,Transform Parent)
            {
                base.Initialize(normal, position, audioClientEffectArg1, audioClientEffectArg2, audioClientEffectType);
                this.chunckId = chunckId;
                this.Parent = Parent;
                NeedRecycle = false;
                ChunkEffectBehaviors.AddLast(this);
            }
            
            protected override void Free(ClientEffectEmitter emitter)
            {
                if (Parent)
                    emitter.nodeObject.transform.SetParent(emitter.PoolFolder);
                ObjectAllocatorHolder<ChunkEffectBehavior>.Free(this);
                ChunkEffectBehaviors.Remove(this);
            }
    
            public static void CleanupChunkEffectBehaviors(int chunkId)
            {
                findRetList.Clear();
                foreach (var node in ChunkEffectBehaviors)
                {
                    if (node != null && node.chunckId == chunkId)
                    {
                        findRetList.Add(node);
                        node.NeedRecycle = true;
                    }
                }
    
                foreach (var ret in findRetList)
                {
                    ChunkEffectBehaviors.Remove(ret);
                }
            }
    
        }
        
}