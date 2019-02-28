using App.Server.SceneManagement;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Client.ClientGameModules.SceneManagement
{
    public class ServerScenePostprocessorSystem : IModuleInitSystem
    {
        private readonly ICommonSessionObjects _session;

        public ServerScenePostprocessorSystem(ICommonSessionObjects session)
        {
            _session = session;
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            _session.LevelManager.SceneLoaded += SingletonManager.Get<ServerScenePostprocessor>().SceneLoaded;
            _session.LevelManager.SceneUnloaded += SingletonManager.Get<ServerScenePostprocessor>().SceneUnloaded;
        }   
    }
}