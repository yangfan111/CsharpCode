using System;
using App.Shared.Audio;
using Core;
using Core.ObjectPool;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared
{
    public interface IEffectBehavior
    {
        void FrameUpdate(ClientEffectEmitter emitter);
        void Recycle(ClientEffectEmitter emitter);
        void PlayEffect(ClientEffectEmitter emitter, GameObject effectGo);

        bool NeedRecycle { get;}

    }

    public abstract class EffectBehaviorAdapter : IEffectBehavior
    {
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

        public bool NeedRecycle { get;protected set; }

        protected abstract void Free(ClientEffectEmitter emitter);
    }
  

 
}