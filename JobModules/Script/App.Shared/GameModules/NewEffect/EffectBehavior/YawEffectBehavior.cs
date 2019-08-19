using Core.ObjectPool;
using UnityEngine;

namespace App.Shared
{
    public class YawEffectBehavior : AbstractImmobileEffectBehavior
    {
        private float Pitch;
        private float Yaw;
        public void Initialize(float pitch,float yaw, Vector3 position,Transform parent = null)
        {
            Pitch = pitch;
            Yaw   = yaw;
            base.Initialize(position,parent);
        }
        public void Initialize(float pitch,float yaw, Vector3 position,int audioClientEffectArg1, int audioClientEffectArg2,
                               AudioClientEffectType audioClientEffectType,Transform parent = null)
        {
            Pitch = pitch;
            Yaw   = yaw;
            base.Initialize(position,new AudioEffectData(audioClientEffectArg1,audioClientEffectArg2,audioClientEffectType),parent);
          
        }

        protected override void SetPosition(ClientEffectEmitter emitter)
        {
            if (Parent)
            {
                emitter.nodeObject.transform.forward       = Parent.forward;
                emitter.nodeObject.transform.localPosition = new Vector3(0, 0, 0.05f);
            }
            else
            {
                emitter.nodeObject.transform.SetPositionAndRotation(Position, Quaternion.Euler(0, Yaw, 0));
            }
        }
        protected override void Free(ClientEffectEmitter emitter)
        {
            base.Free(emitter);
            ObjectAllocatorHolder<YawEffectBehavior>.Free(this);

        }
    }

}