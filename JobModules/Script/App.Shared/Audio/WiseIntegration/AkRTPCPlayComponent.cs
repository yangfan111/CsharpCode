using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Shared.Audio.WiseIntegration
{
    public class AkRTPCPlayComponent : MonoBehaviour
    {
     //   private float duration;
        private uint rtpcId;
        private float timeEnd;
        private System.Action stopCallback;

        public void Initialize(uint rtpcId, float duration, bool setImmediately,System.Action stopCallback)
        {
            timeEnd = Time.time + duration;
            this.rtpcId   = rtpcId;
            this.stopCallback = stopCallback;

            //   this.duration = duration;
            enabled = true;
            if (setImmediately)
            {
                AkSoundEngine.SetRTPCValue(rtpcId, duration);
            }
        }

        void Update()
        {
            if (timeEnd < Time.time)
            {
                AkSoundEngine.SetRTPCValue(rtpcId, 0);
                stopCallback();
                enabled = false;
                return;
            }
            AkSoundEngine.SetRTPCValue(rtpcId, timeEnd-Time.time);
          //  DebugUtil.MyLog("Set:"+(timeEnd-Time.time));

        }
    }
}