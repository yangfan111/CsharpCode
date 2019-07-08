using System;
using System.Collections.Generic;
using System.Linq;
using App.Shared;
using App.Shared.Util;
using Core;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;

namespace App.Client.GameModules.ClientEffect
{
    public class ClientEffectObjectRenderSystem : IRenderSystem
    {
        private IUnityAssetManager assetManager;

        public ClientEffectObjectRenderSystem(Contexts contexts)
        {
            assetManager = contexts.session.commonSession.AssetManager;
        }

        public void OnRender()
        {
            var localEffects = LocalObjectGenerator.EffectLocal.LocalObjectPools;
            foreach (EffectLocalObjectPool pool in localEffects)
            {
                if (pool.playingEffects.Count > 0)
                {
                 //   DebugUtil.MyLog(pool);
                    UpdatePlayingEffect(pool,pool.playingEffects.ToArray());
                }
            }
        }

        private List<ClientEffectEmitter> waitReusableList = new List<ClientEffectEmitter>();
        void UpdatePlayingEffect(EffectLocalObjectPool pool, IEnumerable<ClientEffectEmitter> playingEffects)
        {
            waitReusableList.Clear();
            foreach (ClientEffectEmitter effect in playingEffects)
            {
                switch (effect.StageType)
                {
                    case EEffectStageType.WaitCreate:
                        if (!effect.IsValid)
                        {
                            effect.StageType = EEffectStageType.WaitFinish;
                        }
                        else if (effect.CanPlayRightNow)
                        {
                            effect.DoRealPlay();
                        }
                        else
                        {
                            if(effect.IsPreload)
                            assetManager.LoadAssetAsync(effect.Asset.AssetName, effect.Asset, effect.OnPreLoadSucess,new AssetLoadOption(true));
                            else 
                                assetManager.LoadAssetAsync(effect.Asset.AssetName, effect.Asset, effect.OnLoadSucess,new AssetLoadOption(true));
                            effect.StageType = EEffectStageType.AsynLoading;
                        }

                        break;
                    case EEffectStageType.AsynLoading:

                        if (Time.time > effect.loadAsyncTimestamp + GlobalConst.MaxLoadAsyncWaitTime)
                        {
                            effect.StageType = EEffectStageType.WaitFinish;
                        }

                        break;
                    case EEffectStageType.WaitFinish:
                        waitReusableList.Add(effect);
                        break;
                    case EEffectStageType.Playing:
                        effect.DoFrameUpdate();
                        break;
                }
            }

            foreach (var value in waitReusableList)
            {
                pool.Reusable(value);
            }
        }
    }
}