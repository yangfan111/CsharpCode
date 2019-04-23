using App.Client.SceneManagement.Vegetation;
using App.Shared.Components;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Utils.AssetManager;

namespace App.Client.GameModules.SceneManagement
{
    public class TerrainRendererInitSystem : IModuleInitSystem
    {
        private readonly ICommonSessionObjects _commonSession;
        private readonly ClientSessionObjectsComponent _clientSession;
        
        public TerrainRendererInitSystem(ICommonSessionObjects commonSession, ClientSessionObjectsComponent clientSession)
        {
            _commonSession = commonSession;
            _clientSession = clientSession;
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            _clientSession.TerrainRenderer = new TerrainRenderer();
            _commonSession.LevelManager.SceneLoaded += _clientSession.TerrainRenderer.SceneLoaded;
            _commonSession.LevelManager.SceneUnloaded += _clientSession.TerrainRenderer.SceneUnloaded;
        }
    }
}