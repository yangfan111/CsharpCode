using Core.ObjectPool;
using UnityEngine;

namespace App.Shared
{
    public class HitPlayerEffectBehavior : AbstractImmobileEffectBehavior
    {
        protected override void SetPosition(ClientEffectEmitter emitter)
        {
            emitter.nodeObject.transform.position = Position;
        }
        protected override void Free(ClientEffectEmitter emitter)
        {
            base.Free(emitter);
            ObjectAllocatorHolder<HitPlayerEffectBehavior>.Free(this);
        }
    }

}