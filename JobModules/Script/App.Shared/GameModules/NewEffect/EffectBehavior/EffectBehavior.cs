using System;
using System.Collections.Generic;
using Core.ObjectPool;
using UnityEngine;

namespace App.Shared
{
    public interface IEffectBehavior
    {
        bool NeedRecycle { get; }
        void FrameUpdate(ClientEffectEmitter emitter);
        void Recycle(ClientEffectEmitter emitter);
        void PlayEffect(ClientEffectEmitter emitter, GameObject effectGo);
    }

    public enum EEffectBehaviorType : byte
    {
        HitPlayerEffect,
        MovableEffect,
        ChunkedEffect,
        NormalEffect,
        YawEffect,
        Count
    }

    public abstract class EffectBehaviorAdapter : IEffectBehavior
    {
        // protected static readonly IObjectAllocator[]
        //                 Allocators = new IObjectAllocator[(byte) EEffectBehaviorType.Count];
        //
        // protected static readonly Dictionary<EEffectBehaviorType, Type> effectBehaviorTypes =
        //                 new Dictionary<EEffectBehaviorType, Type>
        //                 {
        //                                 {EEffectBehaviorType.HitPlayerEffect, typeof(HitPlayerEffectBehavior)},
        //                                 {EEffectBehaviorType.MovableEffect, typeof(MoveEffectBehavior)},
        //                                 {EEffectBehaviorType.ChunkedEffect, typeof(ChunkEffectBehavior)},
        //                                 {EEffectBehaviorType.NormalEffect, typeof(NormalEffectBehavior)},
        //                                 {EEffectBehaviorType.YawEffect, typeof(YawEffectBehavior)}
        //                 };

        public virtual void FrameUpdate(ClientEffectEmitter emitter)
        {
        }

        public virtual void Recycle(ClientEffectEmitter emitter)
        {
            emitter.nodeObject.transform.position = Vector3.zero;
            Free(emitter);
        }

        public virtual void PlayEffect(ClientEffectEmitter emitter, GameObject effectGo)
        {
        }

        public bool NeedRecycle { get; protected set; }

        // public static object Allocate(EEffectBehaviorType eftType)
        // {
        //     return Get(eftType).Allocate();
        // }
        //
        // private static IObjectAllocator Get(EEffectBehaviorType eftType)
        // {
        //     var allocator = Allocators[(byte) eftType];
        //     if (allocator == null)
        //     {
        //         allocator                  = ObjectAllocators.GetAllocator(effectBehaviorTypes[eftType]);
        //         Allocators[(byte) eftType] = allocator;
        //     }
        //
        //     return allocator;
        // }
        //
        // public static void Free(EEffectBehaviorType eftType, IEffectBehavior effectBehavior)
        // {
        //     Get(eftType).Free(effectBehavior);
        // }

        protected abstract void Free(ClientEffectEmitter emitter);
    }
}