using Core;
using UnityEngine;

namespace App.Shared
{


    
    public class NormalEffectBehavior : AbstractImmobileEffectBehavior
    {
        private Vector3 Normal;
        public void Initialize(Vector3 normal, Vector3 position, int audioClientEffectArg1, int audioClientEffectArg2,
                               AudioClientEffectType audioClientEffectType)
        {
            Normal = normal;
            base.Initialize(position,new AudioEffectData(audioClientEffectArg1, audioClientEffectArg2, audioClientEffectType));
        }

        public void Initialize(Vector3 normal, Vector3 position)
        {
            Normal = normal;
            base.Initialize(position);
        }
        
        protected override void SetPosition(ClientEffectEmitter emitter)
        {
            emitter.nodeObject.transform.SetPositionAndRotation(Position + GlobalConst.offset * Normal,
                Quaternion.FromToRotation(Vector3.forward, Normal));
        }
    }
}