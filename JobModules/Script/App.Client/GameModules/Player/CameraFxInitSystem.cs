using System.Collections.Generic;
using Core.GameModule.System;
using Entitas;
using App.Shared;
using Utils.AssetManager;
using UnityEngine;
using Core.Utils;

namespace App.Client.GameModules.Player
{
    public class CameraFxInitSystem : ReactiveResourceLoadSystem<PlayerEntity>
    {
        public CameraFxInitSystem(IContext<PlayerEntity> context) : base(context)
        {
            
        }

        public void OnLoadSucc(object source, UnityObject unityObj)
        {
            var player = source as PlayerEntity;
            var fxCam = player.cameraObj.EffectCamera;
            var assetInfo = unityObj.Address;
            if(assetInfo.Equals(AssetConfig.GetCameraPoisonEffect()))
            {
                var go = unityObj.AsGameObject;
                if(null != go)
                {
                    player.cameraFx.Poison = go.GetComponentInChildren<ParticleSystem>().gameObject;
                    player.cameraFx.Poison.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.CameraFx);
                    go.transform.parent = fxCam.transform;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                }
            }
        }

        public override void SingleExecute(PlayerEntity entity)
        {
            AssetManager.LoadAssetAsync(entity, AssetConfig.GetCameraPoisonEffect(), OnLoadSucc);
        }

       

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.CameraObj);
        }
    }
}
