using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using Core.Configuration.Equipment;
using App.Shared.SceneTriggerObject;
using UnityEngine;
using XmlConfig;
using Utils.AssetManager;
using Core.Utils;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace App.Shared.SceneManagement
{

    public class TerrainInit : IOperationAfterConfigLoaded
    {
        private Action _callBack;
        private ISceneListener _sceneListener;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(TerrainInit));

        public TerrainInit(ISceneListener sceneListener)
        {
            _sceneListener = sceneListener;
        }

        public void ServerOperation(object config, Action callBack)
        {
            var sceneConfig = config as SceneConfig;
            SingletonManager.Get<DynamicScenesController>().SetParameters(sceneConfig.TerrainNamePattern,
                                                           sceneConfig.BundleName,
                                                           sceneConfig.TerrainMin,
                                                           sceneConfig.TerrainSize,
                                                           sceneConfig.TerrainDimension,
                                                           sceneConfig.AdditiveSceneName);

            SingletonManager.Get<DynamicScenesController>().SetSceneListener(_sceneListener.OnSceneLoaded, _sceneListener.OnSceneUnloaded);
            SingletonManager.Get<DynamicScenesController>().SetRadius(float.MaxValue, 0);
            SingletonManager.Get<DynamicScenesController>().AllSceneLoaded = SceneInitDone;
            SingletonManager.Get<DynamicScenesController>().InitialUpdate(sceneConfig.PlayerBirthPosition);

            _callBack = callBack;
        }

        public void ClientOperation(object config, Action callBack)
        {
            var sceneConfig = config as SceneConfig;
            SingletonManager.Get<DynamicScenesController>().SetParameters(sceneConfig.TerrainNamePattern,
                                                           sceneConfig.BundleName,
                                                           sceneConfig.TerrainMin,
                                                           sceneConfig.TerrainSize,
                                                           sceneConfig.TerrainDimension,
                                                           sceneConfig.AdditiveSceneName);
            SingletonManager.Get<DynamicScenesController>().SetSceneListener(_sceneListener.OnSceneLoaded, _sceneListener.OnSceneUnloaded);
            SingletonManager.Get<DynamicScenesController>().SetRadius(1, 0.5f);
            SingletonManager.Get<DynamicScenesController>().AllSceneLoaded = SceneInitDone;
            SingletonManager.Get<DynamicScenesController>().InitialUpdate(sceneConfig.PlayerBirthPosition);

            _callBack = callBack;
        }

        private void SceneInitDone()
        {
            SingletonManager.Get<DynamicScenesController>().AllSceneLoaded = null;
            _callBack();
        }

        public IList<SceneRequest> LoadInitialScene()
        {
            var ret = SingletonManager.Get<DynamicScenesController>().ManagermentRequest.ToList();
            SingletonManager.Get<DynamicScenesController>().ResetRequest();

            return ret;
        }
    }
}
