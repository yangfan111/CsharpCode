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
        private readonly TerrainRenderer _renderer;
        
        public TerrainRendererInitSystem(ICommonSessionObjects commonSession, ClientSessionObjectsComponent clientSession)
        {
            _commonSession = commonSession;
            _clientSession = clientSession;
            
            _renderer = new TerrainRenderer();
            _clientSession.TerrainRenderer = _renderer;
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            if (_renderer.IsReady)
            {
                _commonSession.LevelManager.SceneLoaded += _clientSession.TerrainRenderer.SceneLoaded;
                _commonSession.LevelManager.SceneUnloaded += _clientSession.TerrainRenderer.SceneUnloaded;
            }
        }
    }
}