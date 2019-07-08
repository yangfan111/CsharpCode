using System;
using System.Collections.Generic;
using System.Linq;
using App.Shared.Util;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject;
using Core;
using Core.GameTime;
using Google.Protobuf.WellKnownTypes;
using Sharpen;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App.Shared
{
    public class AudioLocalObjectGenerator:LocalObjectGenerator
    {
       

        private readonly Stack<AkGameObj>   reusableAudios = new Stack<AkGameObj>(50);
        private readonly HashSet<AkGameObj> playingAuidos  = new HashSet<AkGameObj>();
        public void FinishAudio(AkGameObj instance)
        {
            if (!playingAuidos.Remove(instance))
                return;
            instance.IsMute = true;
            reusableAudios.Push(instance);
        }

        private void CreateNew(out AkGameObj akGameObj)
        {
            var clone = new GameObject(string.Format("ak:{0}", accumulator++));
            akGameObj = clone.AddComponent<AkGameObj>();
            //akGameObj.InstanceObj.instanceId = clone.GetInstanceID();
            akGameObj.SequenceIndex = accumulator - 1;
            akGameObj.transform.SetParent(generatorGo.transform);
        }
        public AkGameObj GetAudioEmitter()
        {
            AkGameObj akGameObj;
            int nowSec = DateTime.UtcNow.Second;
            if (reusableAudios.Count > 0)
            {
                akGameObj = reusableAudios.Pop();
            }
            else if (playingAuidos.Count >= GlobalConst.AudioObjectUsageMaxCount)
            {
            
                var akgameObjectList = playingAuidos.ToArray();
             //   Array.Sort(akgameObjectList);
                int cutoffCount =  Mathf.CeilToInt(akgameObjectList.Length * GlobalConst.AudioObjectCuttoffThreshold);
                for (int i = 0; i < cutoffCount; i++)
                {
                    var ele = akgameObjectList[i];
                    ele.IsMute = true;
                    playingAuidos.Remove(ele);
                    reusableAudios.Push(ele);
                  
                }
                if(reusableAudios.Count>0)
                    akGameObj = reusableAudios.Pop();
                else
                    CreateNew(out akGameObj);
            }
            else
            {
                CreateNew(out akGameObj);
            }

            //AKRESULT result = akGameObj.Register();
            //AkLogger.Message(result.ToString());
            akGameObj.IsMute = false;
          //  akGameObj.EndTimeSecStamp = nowSec + GlobalConst.MaxAudioExistTimeSec;

            playingAuidos.Add(akGameObj);

            return akGameObj;
        }
    }
}