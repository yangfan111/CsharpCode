using System;
using App.Shared;
using App.Shared.SceneManagement;
using Core.SceneManagement;
using Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace App.Client.SceneManagement
{
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
                            
                    v.useOcclusionCulling = false;
//                    if (v.GetComponent<AudioListener>() == null)
//                    {
//                        v.gameObject.AddComponent<AudioListener>();
//                    }
                }

                InitCameraPostProcessEffect();
            }
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