using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Player;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace Assets.App.Client.GameModules.Player
{
    class ClientCameraFinalRenderSystem : AbstractSelfPlayerRenderSystem
    {
        float[] f = new float[32];
        private float lastFar = 0;
        private float lastNear = 0;


        public ClientCameraFinalRenderSystem(Contexts contexts) : base(contexts)
        {

        }

        protected override bool Filter(PlayerEntity player)
        {
            return player.hasCameraObj && player.hasCameraFinalOutputNew;
        }

        public override void OnRender(PlayerEntity playerEntity)
        {
            var camera = playerEntity.cameraObj.MainCamera;
            var fcamera = playerEntity.cameraObj.FPCamera;
            if (Math.Abs(lastFar - playerEntity.cameraFinalOutputNew.Far) > 0.01f)
            {
                lastFar = playerEntity.cameraFinalOutputNew.Far;

                camera.far = 8000;
               
            }

            if (Math.Abs(lastNear - playerEntity.cameraFinalOutputNew.Near) > 0.0001f)
            {
                lastNear = camera.near;
                camera.near = playerEntity.cameraFinalOutputNew.Near;
            }


            camera.fieldOfView = SsjjFovToUnityFov(playerEntity.cameraFinalOutputNew.Fov, Screen.width, Screen.height);
            fcamera.fieldOfView = SsjjFovToUnityFov(playerEntity.cameraFinalOutputNew.Fov, Screen.width, Screen.height);
            fcamera.transform.position = camera.transform.position = playerEntity.cameraFinalOutputNew.Position;
            fcamera.transform.rotation = camera.transform.rotation =
                Quaternion.Euler(playerEntity.cameraFinalOutputNew.EulerAngle);
        }

        public static float SsjjFovToUnityFov(float fov, float width, float height)
        {
            var d = Mathf.Tan(fov / 2 * Mathf.Deg2Rad) * height / width;
            return Mathf.Atan(d) * Mathf.Rad2Deg * 2;
        }
    }
}