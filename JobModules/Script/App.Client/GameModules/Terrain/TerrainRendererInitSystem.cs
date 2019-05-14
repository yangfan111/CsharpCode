using App.Client.SceneManagement.Vegetation;
using App.Shared.Components;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Utils.AssetManager;

namespace App.Client.GameModules.Terrain
{
    public class TerrainRendererInitSystem : IModuleInitSystem
    {
        private readonly ICommonSessionObjects _commonSession;
        private readonly ClientSessionObjectsComponent _clientSession;
        
        public TerrainRendererInitSystem(ICommonSessionObjects commonSession, ClientSessionObjectsComponent clientSession)
        {
            _commonSession = commonSession;
            _clientSession = clientSession;
            _clientSession.TerrainRenderer = new TerrainRenderer();
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            _commonSession.LevelManager.SceneLoaded += _clientSession.TerrainRenderer.SceneLoaded;
            _commonSession.LevelManager.SceneUnloaded += _clientSession.TerrainRenderer.SceneUnloaded;
        }
    }
}