using App.Client.SceneManagement;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Client.GameModules.SceneManagement
{
    public class ClientScenePostprocessorSystem : IModuleInitSystem
    {
        private readonly ICommonSessionObjects _session;

        public ClientScenePostprocessorSystem(ICommonSessionObjects session)
        {
            _session = session;
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            _session.LevelManager.SceneLoaded += SingletonManager.Get<ClientScenePostprocessor>().SceneLoaded;
            _session.LevelManager.SceneUnloaded += SingletonManager.Get<ClientScenePostprocessor>().SceneUnloaded;
        }
    }
}