using UnityEngine;

namespace App.Shared
{
    public class HitPlayerEffectBehavior : AbstractImmobileEffectBehavior
    {
        protected override void SetPosition(ClientEffectEmitter emitter)
        {
            emitter.nodeObject.transform.position = Position;
        }
        
    }

}