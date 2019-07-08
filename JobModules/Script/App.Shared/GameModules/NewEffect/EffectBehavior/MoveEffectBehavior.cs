using System;
using Core;
using Core.ObjectPool;
using Core.Utils;
using UnityEngine;
using Random = System.Random;

namespace App.Shared
{
    public class MoveEffectBehavior :EffectBehaviorAdapter
    {
        private Vector3 CurrPosition;
        private Quaternion InitRocatioin;
        private Vector3 Velocity;
        private float DelayDuration;
        private float lastUpdateTime;
        private float ShowEffectTime;
        private GameObject effectGo;
        public void Initialize(Vector3 currPosition, Quaternion initRocatioin, Vector3 velocity, float delayDuration)
        {
            // var interval = UnityEngine.Random.Range(GlobalConst.BulletFlyEfcFowardDistanceScope.x, GlobalConst.BulletFlyEfcFowardDistanceScope.y);
            // CurrPosition.x = currPosition.x + Velocity.x * interval;
            // CurrPosition.y = currPosition.y + Velocity.y * interval;
            // CurrPosition.z = currPosition.z + Velocity.z * interval;
            CurrPosition = currPosition;
            InitRocatioin  = initRocatioin;
            Velocity       = velocity;
            DelayDuration = delayDuration;
            lastUpdateTime = Time.time;
            NeedRecycle = false;
        }

        public override void PlayEffect(ClientEffectEmitter emitter, GameObject effectGo)
        {
            this.effectGo = effectGo;
            emitter.nodeObject.transform.SetPositionAndRotation(CurrPosition,InitRocatioin);
            effectGo.SetActive(false);
            ShowEffectTime = Time.time + DelayDuration;
        }
        public override void FrameUpdate(ClientEffectEmitter emitter)
        {
            var nowTime = Time.time;;
            var interval = nowTime - lastUpdateTime;
            if (effectGo && ShowEffectTime <= nowTime)
            {
                effectGo.SetActive(true);
                effectGo = null;
            }
            // O(1) = O(0) + V(0) * t;
            CurrPosition.x = CurrPosition.x + Velocity.x * interval;
            CurrPosition.y = CurrPosition.y + Velocity.y * interval;
            CurrPosition.z = CurrPosition.z + Velocity.z * interval;

            emitter.nodeObject.transform.position = CurrPosition;
        //   DebugUtil.MyLog("UpdatePos:{0},{1}",CurrPosition,interval.ToString("f3"));
            lastUpdateTime = nowTime;
           
        }

        protected override void Free(ClientEffectEmitter emitter)
        {
            ObjectAllocatorHolder<MoveEffectBehavior>.Free(this);
        }
    }
}