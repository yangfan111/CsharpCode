using System;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine.SceneManagement;
using XmlConfig;
using UnityEngine;
using Utils.AssetManager;
using Core.Utils;
using Shared.Scripts.RuntimeScripts;

namespace App.Shared.SceneManagement
{
    public class LevelController : SceneAbstractController<LevelController>
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(LevelController));
        public Action AllSceneLoaded;

        private Scene lastScene;

        private PositionRelatedEffectUpdater _effectUpdater;
        
        public LevelController()
        {
        }

        private LevelConfig _asset;
        public void SetConfig(LevelConfig asset)
        {
            _asset = asset;
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void Clear()
        {
            Logger.InfoFormat("LevelController Clear");
            lastScene = default(Scene);
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        public IList<SceneRequest> SceneRequest()
        {
            return new List<SceneRequest>
            {
                new SceneRequest
                {
                     IsLoad = true,
                     Address = new AssetInfo(_asset.BundleName, _asset.AssetName),
                     IsAdditive = true
                }
            };
        }

        public IUpdatePositionRelatedEffect GetPositionRelatedEffectUpdater()
        {
            return _effectUpdater;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode modee)
        {
            if(lastScene.isLoaded && lastScene.name != null)
            {
                Logger.InfoFormat("The last scene {0} have been loaded, unload it first!", lastScene.name);
                SceneManager.UnloadSceneAsync(lastScene);
            }
            lastScene = scene;

            if (!scene.isLoaded)
            {
                throw new Exception("Scene is not loaded.");
            }

            SceneManager.SetActiveScene(scene);
            Logger.DebugFormat("SET ACTIVE SCENE: {0}", scene.name);
            foreach (var go in scene.GetRootGameObjects())
            {
                var comp = go.GetComponentInChildren<PositionRelatedEffect>();
                if (comp != null)
                    _effectUpdater = new PositionRelatedEffectUpdater(comp);

                // 禁用相机，保证角色动画不更新
                if (SharedConfig.IsServer)
                {
                    foreach (var v in go.GetComponentsInChildren<Camera>())
                    {
                        v.enabled = false;
                    }
                }
            }
            
            if (AllSceneLoaded != null)
            {
                AllSceneLoaded();
            }

            if (onSceneLoaded != null)
            {
                onSceneLoaded(0, 0, scene);
            }
        }

        protected override void OnDispose()
        {
            Clear();
        }
    }
}
