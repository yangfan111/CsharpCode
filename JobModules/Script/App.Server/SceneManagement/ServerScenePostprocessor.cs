using App.Shared;
using Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace App.Server.SceneManagement
{
    public class ServerScenePostprocessor : Singleton<ServerScenePostprocessor>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerScenePostprocessor));

        // comes from DynamicScenesController.cs
        public void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var go in scene.GetRootGameObjects())
            {
                foreach (var v in go.GetComponentsInChildren<Camera>())
                {
                    v.enabled = false;
                }
            }
        }

        public void SceneUnloaded(Scene scene)
        {
            
        }
    }
}