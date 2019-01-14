using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using App.Shared.SceneTriggerObject;
using Utils.AssetManager;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.SceneManagement
{
    public class LevelInit: IOperationAfterConfigLoaded
    {
        private Action _callBack;

        private ISceneListener _sceneListener;
        public LevelInit(ISceneListener sceneListener)
        {
            _sceneListener = sceneListener;
            SingletonManager.Get<LevelController>().SetSceneListener(_sceneListener.OnSceneLoaded, _sceneListener.OnSceneUnloaded);
        }

        public void ServerOperation(object config, Action callBack)
        {
            _callBack = callBack;
            SingletonManager.Get<LevelController>().SetConfig(config as LevelConfig);
            SingletonManager.Get<LevelController>().AllSceneLoaded = LevelInitDone;
        }

        public void ClientOperation(object config, Action callBack)
        {
            _callBack = callBack;
            SingletonManager.Get<LevelController>().SetConfig(config as LevelConfig);
            SingletonManager.Get<LevelController>().AllSceneLoaded = LevelInitDone;
        }

        private void LevelInitDone()
        {
            SingletonManager.Get<LevelController>().AllSceneLoaded = null;
            _callBack();
        }

        public IList<SceneRequest> LoadInitialScene()
        {
            return SingletonManager.Get<LevelController>().SceneRequest();
        }
    }
}
