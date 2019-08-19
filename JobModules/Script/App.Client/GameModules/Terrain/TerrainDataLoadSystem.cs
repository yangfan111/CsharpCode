using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.SceneManagement;
using Core.SessionState;
using Utils.AssetManager;
using Core.Utils;

namespace App.Client.GameModules.Terrain
{
    public class TerrainDataLoadSystem : IResourceLoadSystem
    {
        private readonly ISessionState _sessionState;
        private readonly ILevelManager _levelManager;
        private readonly ITerrainRenderer _terrainRenderer;
        private readonly List<string> _terrainName = new List<string>();

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(TerrainDataLoadSystem));

        public TerrainDataLoadSystem(ISessionState sessionState, Contexts ctx)
            : this(ctx)
        {
            _sessionState = sessionState;
            
            _sessionState.CreateExitCondition(typeof(TerrainDataLoadSystem));
        }
        
        public TerrainDataLoadSystem(Contexts ctx)
        {
            _levelManager = ctx.session.commonSession.LevelManager;
            _terrainRenderer = ctx.session.clientSessionObjects.TerrainRenderer;
        }

        public void OnLoadResources(IUnityAssetManager assetManager)
        {
            _terrainRenderer.GetTerrainDataNames(_terrainName);
            if (_terrainName.Count > 0)
            {
                for (int i = 0; i < _terrainName.Count; ++i)
                {
                    AssetInfo addr = new AssetInfo("tablesfrombuilding", _terrainName[i]);

                    _logger.InfoFormat("´òËãÔØÈë _terrainName£¬BundleName={0}, AssetName={1}", addr.BundleName, addr.AssetName);

                    assetManager.LoadAssetAsync((UnityEngine.Object) null, addr, _terrainRenderer.LoadedTerrainData);
                }

                _terrainName.Clear();
            }

            if (_sessionState != null)
            {
                if (_levelManager.NotFinishedRequests <= 0 &&
                    _terrainRenderer.IsLoadingEnd)
                {
                    _sessionState.FullfillExitCondition(typeof(TerrainDataLoadSystem));
                }
                else
                {
                    if ((System.DateTime.Now - _lastPrintTime).TotalMilliseconds > _logInterval)
                    {
                        _lastPrintTime = System.DateTime.Now;

                        _logger.InfoFormat("NotFinishedRequests =={0}, IsLoadingEnd =={1}", _levelManager.NotFinishedRequests, _terrainRenderer.IsLoadingEnd);
                        _logger.InfoFormat(_levelManager.GetRequestString());

                        //if (_logInterval < 60000)
                        //    _logInterval *= 2;
                    }

                    
                }
            }
        }

        private System.DateTime _lastPrintTime = System.DateTime.Now;
        private int _logInterval = 2000;
    }
}