using App.Shared.Cam;
using App.Shared.EntityFactory;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using System;
using System.Collections.Generic;
using Core.GameModule.Interface;
using UnityEngine;

namespace App.Client.GameModules.Player
{
    public class ClientPlayerCameraInitSystem : IEntityInitSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientPlayerCameraInitSystem));
        private IGroup<PlayerEntity> _iGroup;
        private float lastLogTime;

        public ClientPlayerCameraInitSystem(
            PlayerContext playerContext)
        {
            _iGroup = playerContext.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.UserCmd, PlayerMatcher.FlagSelf)
                .NoneOf(PlayerMatcher.CameraObj));
        }

        public void OnEntityInit()
        {
         
            foreach (var player in _iGroup.GetEntities())
            {
                var mainCamera = Camera.main;
                if (null == mainCamera)
                {
                    if (Time.time - lastLogTime > 2000)
                    {
                        Logger.Error("no init main camera in scene");
                        lastLogTime = Time.time;
                    }

                    return;
                }
                try
                {
                    ICameraFactory cameraFactory = new SingleCameraFactory();
                   
                    mainCamera.transform.localScale = Vector3.one;
                    mainCamera.cullingMask &= ~UnityLayerManager.GetLayerMask(EUnityLayerName.UI);
                    mainCamera.cullingMask &= ~UnityLayerManager.GetLayerMask(EUnityLayerName.UIParticle);
                    mainCamera.cullingMask &= ~UnityLayerManager.GetLayerMask(EUnityLayerName.RenderUIParticle);
                    var fpCamera = cameraFactory.CreateFpCamera(mainCamera);
                    var fxCamera = cameraFactory.CreateFxCamera(mainCamera);
                    player.AddCameraObj();
                    player.AddCameraFx();
                    player.cameraObj.EffectCamera = fxCamera;
                    player.cameraObj.FPCamera = fpCamera;
                    player.cameraObj.MainCamera = mainCamera;
                }
                catch
                {
                    Logger.Error("MainCamera is null ??? !!!");
                }
            }
        }
    }
}