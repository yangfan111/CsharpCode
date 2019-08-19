using System;
using System.Linq;
using System.Runtime.CompilerServices;
using App.Shared.Util;
using Core;
using Core.Configuration;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared
{
    public class EffectLocalObjectGenerator : LocalObjectGenerator
    {
        public EffectLocalObjectPool[] LocalObjectPools { get; private set; }

        public EffectLocalObjectPool this[EEffectObjectClassify classify]
        {
            get { return LocalObjectPools[(int) classify]; }
        }

        protected override void Clear()
        {
            for (int i = 0; i < LocalObjectPools.Length; i++)
            {
                LocalObjectPools[i].Reusable();
            }
            ChunkEffectBehavior.ChunkEffectBehaviors.Clear();
        }
        void Awake()
        {
            ChunkEffectBehavior.ChunkEffectBehaviors.Clear();
            LocalObjectPools = new EffectLocalObjectPool[(int) EEffectObjectClassify.Count];
            for (int i = 0; i < LocalObjectPools.Length; i++)
            {
                LocalObjectPools[i] = new EffectLocalObjectPool((EEffectObjectClassify)i);
                LocalObjectPools[i].PreLoad();
            }
        }
        

        public ClientEffectEmitter GetClientEffectEmitter(EEffectObjectClassify effectObjectClassify)
        {
            return LocalObjectPools[(int) effectObjectClassify].GetClientEffectBehaviorItem();
        }
    }


  

    public class EffectLocalObjectPool : AbstractLocalObjectPool<ClientEffectEmitter>
    {
        private EEffectObjectClassify classify;

        public EffectLocalObjectPool(EEffectObjectClassify classify)
        {
            this.classify = classify;
        }
        public override void PreLoad()
        {
            var go = new GameObject("Efc_"+classify);
            poolFolder = go.transform;
            poolFolder.SetParent(LocalObjectGenerator.generatorGo.transform);
            cfgItem = SingletonManager.Get<ClientEffectCommonConfigManager>().GetConfigByType(classify);
            if (cfgItem != null && cfgItem.PreLoadCfgId > 0)
            {
                ClientEffectEmitter firstAsset;
                for (int i = 0; i < 5; i++)
                {
                    firstAsset = GetClientEffectBehaviorItem();
                    firstAsset.Preload(cfgItem.PreLoadCfgId);
                }
            }

          
        }

        private ClientEffectEmitter[] clientEffectBehaviorsBuffer; 
        /// <summary>
        /// </summary>
        /// <param name="isConstEffect">是否持续播放</param>
        /// <param name="existDuration">毫秒</param>
        /// <returns></returns>
        public ClientEffectEmitter GetClientEffectBehaviorItem()
        {
            ClientEffectEmitter clientEffectEmitter;
            //      int          nowSec = DateTime.UtcNow.Second;
            if (reusableEffects.Count > 0)
            {
                clientEffectEmitter = reusableEffects.Pop();
            }
            else if (playingEffects.Count >= cfgItem.ObjectLimit)
            {
                clientEffectBehaviorsBuffer = playingEffects.ToArray<ClientEffectEmitter>();
                int cutoffCount = Mathf.CeilToInt(playingEffects.Count * cfgItem.CutoffThreshold);
                Array.Sort(clientEffectBehaviorsBuffer, ClientEffectEmitter.SequenceIndexComparer);
                int accCutoffNum = 0;
                for (int i = 0; i < playingEffects.Count; i++)
                {
                    var ele = clientEffectBehaviorsBuffer[i];
                    if (!ele.IsStatic)
                    {
                        Reusable(ele);
                        ++accCutoffNum;
                    }

                    if (accCutoffNum >= cutoffCount)
                        break;
                }

                if (reusableEffects.Count > 0)
                {
                    clientEffectEmitter = reusableEffects.Pop();
                    
                }
                else
                {
                    clientEffectEmitter = ClientEffectEmitter.Allocate();
                    clientEffectEmitter.classify = classify;
                }
                    
            }
            else
            {
                clientEffectEmitter = ClientEffectEmitter.Allocate();
                clientEffectEmitter.classify = classify;
                //    var clone = new GameObject(string.Format("efc:{0}", accumulator++));
            }

            clientEffectEmitter.SequenceIndex = ++accumulator ;
            clientEffectEmitter.Duration = cfgItem.LifeTime / 1000;
            
            //AKRESULT result = akGameObj.Register();
            //AkLogger.Message(result.ToString());

            playingEffects.Add(clientEffectEmitter);

            return clientEffectEmitter;
        }
    }
}