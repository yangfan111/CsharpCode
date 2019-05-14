using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject;
using Core;
using Core.GameTime;
using Google.Protobuf.WellKnownTypes;
using Sharpen;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App.Shared
{
    public class AudioObjectGenerator
    {
        public static int accumulator         = 0;

        private readonly Stack<AkGameObj>   reusableAudios = new Stack<AkGameObj>(50);
        private readonly HashSet<AkGameObj> playingAuidos  = new HashSet<AkGameObj>();

        public void Dispose()
        {
            if (generatorGo)
                UnityEngine.Object.Destroy(generatorGo);
            generatorGo = null;
        }

        private GameObject generatorGo;

        private GameObject GenerateGo
        {
            get
            {
                if (generatorGo == null)
                {
                    generatorGo = new GameObject("AKAudioObjGenerator");
                    reusableAudios.Clear();
                    playingAuidos.Clear();
                    accumulator = 0;
                }

                return generatorGo;
            }
        }

        public void FinishAudio(AkGameObj instance)
        {
            if (!playingAuidos.Remove(instance))
                return;
            instance.IsMute = true;
            reusableAudios.Push(instance);
        }

        public AkGameObj GetAudioEmitter()
        {
            var generateTrans = GenerateGo.transform;

            AkGameObj akGameObj;
            int nowSec = DateTime.UtcNow.Second;
            if (reusableAudios.Count > 0)
            {
                akGameObj = reusableAudios.Pop();
            }
            else if (playingAuidos.Count >= GlobalConst.ObjectUsageMaxCount)
            {
                var akgameObjectList = playingAuidos.ToArray();
                int cutoffCount = Mathf.CeilToInt(playingAuidos.Count * 0.7f);
                for (int i = 0; i < playingAuidos.Count; i++)
                {
                    var ele = akgameObjectList[i];
                    if(nowSec> ele.EndTimeSecStamp || i<cutoffCount)
                    {
                        ele.IsMute = true;
                        playingAuidos.Remove(ele);
                        reusableAudios.Push(ele);
                    }
                  
                }
              
                akGameObj = reusableAudios.Pop();
            }
            else
            {
                //Object res =
                //    UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(
                //        "Assets/Assets/CoreRes/Sound/Model/AkObj.prefab");
                var clone = new GameObject(string.Format("ak:{0}", accumulator++));
                akGameObj                        = clone.AddComponent<AkGameObj>();
                //akGameObj.InstanceObj.instanceId = clone.GetInstanceID();
                akGameObj.SequenceIndex = accumulator - 1;
                akGameObj.transform.SetParent(generateTrans);
            }

            //AKRESULT result = akGameObj.Register();
            //AkLogger.Message(result.ToString());
            akGameObj.IsMute = false;
            akGameObj.EndTimeSecStamp = nowSec + GlobalConst.MaxAudioExistTimeSec;

            playingAuidos.Add(akGameObj);

            return akGameObj;
        }
    }
}