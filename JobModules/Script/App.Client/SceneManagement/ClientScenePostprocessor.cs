using System;
using App.Shared;
using App.Shared.Configuration;
using App.Shared.SceneManagement;
using Core.SceneManagement;
using Core.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace App.Client.SceneManagement
{

    enum OCType
    {
        None = 0,
        Umbra = 1,
        HZB = 2,
    }

    public class ClientScenePostprocessor : Singleton<ClientScenePostprocessor>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientScenePostprocessor));

        // comes from DynamicScenesController.cs
        public void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var go in scene.GetRootGameObjects())
            {
                var terrain = go.GetComponent<Terrain>();
                if (terrain != null)
                {
                    Type t = Type.GetType("ArtPlugins.TerrainProxy, Assembly-CSharp");
                    if (t != null)
                    {
                        terrain.drawTreesAndFoliage = true;
                        terrain.heightmapPixelError = 20;
                        terrain.collectDetailPatches = true;
                        terrain.detailObjectDistance = 120;
                        terrain.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                        terrain.gameObject.AddComponent(t);

                        _logger.Debug("ArtPlugins.TerrainProxy, Assembly-CSharp Founded");
                    }
                    else
                        _logger.Error("ArtPlugins.TerrainProxy, Assembly-CSharp Not Founded");
                    
                    terrain.treeDistance = 800;
                }

                var mapConfig = SingletonManager.Get<MapConfigManager>().SceneParameters;

    
                foreach (var v in go.GetComponentsInChildren<Camera>())
                {
                    if (v != Camera.main) continue;//非主相机不需要GQS_Bind_Camera
                    Type t = Type.GetType("ArtPlugins.GQS_Bind_Camera, Assembly-CSharp");
                    if (t != null)
                    {
                        v.gameObject.AddComponent(t);
                    }
                    else
                    {
                        _logger.Error("ArtPlugins.GQS_Bind_Camera is null ??? !!!");
                    }
                            
                    v.useOcclusionCulling = mapConfig.OcType == (int) OCType.Umbra;
//                    if (v.GetComponent<AudioListener>() == null)
//                    {
//                        v.gameObject.AddComponent<AudioListener>();
//                    }
                }

                SetDepthPrepass(mapConfig);
                SetHZBCulling(mapConfig);

                InitCameraPostProcessEffect();
            }
        }

        private void SetDepthPrepass(XmlConfig.AbstractMapConfig mapConfig)
        {
            GraphicsSettings.depthPrepassEnable = mapConfig.depthPrepassEnable;
            GraphicsSettings.minRadiusRatioForDepthPrepass = mapConfig.minRadiusRatioForDepthPrepass;

            if (mapConfig.depthPrepassEnable)
                SharedConfig.GrassQueue = 1300;
            else
                SharedConfig.GrassQueue = -1;
            
        }

       private void SetHZBCulling(XmlConfig.AbstractMapConfig mapConfig)
        {
            GraphicsSettings.hzbCullingEnable = mapConfig.OcType == (int)OCType.HZB;
            GraphicsSettings.hzbMinCullingDistance = mapConfig.hzbMinCullingDistance;
            GraphicsSettings.hzbCameraRejectEnable = mapConfig.hzbCameraRejectEnable;
            GraphicsSettings.hzbCameraTranslationThreshold = mapConfig.hzbCameraTranslationThreshold;
            GraphicsSettings.hzbCameraRotationThreshold = mapConfig.hzbCameraRotationThreshold;
        }


        private void InitCameraPostProcessEffect()
        {
            //close SE Screen Space Shadow
            var mainCam = Camera.main;
            if (mainCam == null)
            {
                _logger.Error("Main Camera does not exist!");
                return;
            }

            var sss = mainCam.GetComponent("SEScreenSpaceShadows") as MonoBehaviour;
            if (sss != null)
            {
                sss.enabled = false;
            }

        }
        public void SceneUnloaded(Scene scene)
        {
            
        }
    }
}