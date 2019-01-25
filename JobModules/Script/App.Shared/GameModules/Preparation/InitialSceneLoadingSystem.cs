using System.Collections.Generic;
using System.ComponentModel;
using App.Shared.Configuration;
using App.Shared.SceneManagement;
using App.Shared.SceneManagement.Basic;
using App.Shared.SceneManagement.Streaming;
using App.Shared.SceneTriggerObject;
using Assets.Utils.Configuration;
using Core.GameModule.Interface;
using Core.SessionState;
using Entitas;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Preparation
{
    public class InitialSceneLoadingSystem : IModuleInitSystem, IResourceLoadSystem
    {
        private readonly Contexts _contexts;
        private readonly ISessionState _sessionState;
        private readonly LevelManager _levelManager;
        
        private List<AssetInfo> _sceneRequests = new List<AssetInfo>();
        private List<AssetInfo> _goRequests = new List<AssetInfo>();
        
        public bool IsServer { get; set; }
        public bool AsapMode { get; set; }
        private readonly Vector3 _initPosition;
        public InitialSceneLoadingSystem(ISessionState sessionState, Contexts ctx, IStreamingGoManager streamingGo)
        {
            _sessionState = sessionState;
            _contexts = ctx;
            
            _sessionState.CreateExitCondition(typeof(InitialSceneLoadingSystem));

            _levelManager = new LevelManager();
            _contexts.session.commonSession.LevelManager = _levelManager;
            _initPosition = _contexts.session.commonSession.InitPosition;

            var allMaps = SingletonManager.Get<MapsDescription>();
            
            switch (allMaps.CurrentLevelType)
            {
                case LevelType.BigMap:
                    var worldCompositionParam = new WorldCompositionParam
                    {
                        TerrainMin = allMaps.BigMapParameters.TerrainMin,
                        TerrainDimension = allMaps.BigMapParameters.TerrainDimension,
                        TerrainSize = allMaps.BigMapParameters.TerrainSize,
                        TerrainNamePattern = allMaps.BigMapParameters.TerrainNamePattern,
                        AssetBundleName = allMaps.BigMapParameters.BundleName,
                        FixedScenes = allMaps.BigMapParameters.AdditiveSceneName,
                        LoadRadiusInGrid = 1.0f,
                        UnloadRadiusInGrid = 1.5f
                    };
                    if (IsServer)
                    {
                        worldCompositionParam.LoadRadiusInGrid = float.MaxValue;
                        worldCompositionParam.UnloadRadiusInGrid = float.MaxValue;
                    }

                    _levelManager.SetToWorldCompositionLevel(worldCompositionParam, streamingGo);
                    _levelManager.SetAsapMode(true);
                    break;
                case LevelType.SmallMap:
                    _levelManager.SetToFixedScenesLevel(new OnceForAllParam
                    {
                        AssetBundleName = allMaps.SmallMapParameters.BundleName,
                        FixedScenes = new List<string>
                        {
                            allMaps.SmallMapParameters.AssetName
                        }
                    });
                    break;
                case LevelType.Exception:
                    throw new InvalidEnumArgumentException("map id not set");
            }
        }
        
        public void OnInitModule(ILoadRequestManager manager)
        {
            _levelManager.UpdateOrigin(IsServer ? Vector3.zero : _initPosition);

            RequestForResource(manager);
        }

        public void OnLoadResources(ILoadRequestManager loadRequestManager)
        {
            RequestForResource(loadRequestManager);

            if (_levelManager.NotFinishedRequests <= 0)
            {
                _levelManager.SetAsapMode(AsapMode);
                _sessionState.FullfillExitCondition(typeof(InitialSceneLoadingSystem));
            }
        }

        private void RequestForResource(ILoadRequestManager manager)
        {
            _levelManager.GetRequests(_sceneRequests, _goRequests);
            
            foreach (var request in _sceneRequests)
            {
                manager.AddSceneRequest(new SceneRequest
                {
                    Address = request,
                    IsAdditive = true,
                    IsLoad = true
                });
            }
            
            foreach (var request in _goRequests)
            {
                manager.AppendLoadRequest(null, request, _levelManager.GoLoadedWrapper);
            }
            
            _sceneRequests.Clear();
            _goRequests.Clear();
        }
    }
}