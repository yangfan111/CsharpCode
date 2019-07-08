﻿using System.Collections.Generic;
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
        private List<IEnumerable<AssetInfoEx<MeshRenderer>>> _lightmapsRequests = new List<IEnumerable<AssetInfoEx<MeshRenderer>>>();

        private readonly bool _isServer;
        public bool AsapMode { get; set; }
        private readonly Vector3 _initPosition;
        public InitialSceneLoadingSystem(ISessionState sessionState, Contexts ctx, IStreamingGoManager streamingGo, bool isServer)
        {
            _sessionState = sessionState;
            _contexts = ctx;

            _sessionState.CreateExitCondition(typeof(InitialSceneLoadingSystem));

            _levelManager = new LevelManager(_contexts.session.commonSession.AssetManager, isServer);
            _contexts.session.commonSession.LevelManager = _levelManager;
            _initPosition = _contexts.session.commonSession.InitPosition;

            _isServer = isServer;

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
                        PreMapName = allMaps.BigMapParameters.PreMapName,
                        FixedScenes = allMaps.BigMapParameters.AdditiveSceneName,
                        LoadRadiusInGrid = 1.0f,
                        UnloadRadiusInGrid = 1.5f
                    };
                    if (_isServer)
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

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            _levelManager.UpdateOrigin(_isServer ? Vector3.zero : _initPosition);

            RequestForResource(assetManager);
        }

        public void OnLoadResources(IUnityAssetManager assetManager)
        {
            RequestForResource(assetManager);

            if (_levelManager.NotFinishedRequests <= 0)
            {
                _levelManager.SetAsapMode(AsapMode);
                _sessionState.FullfillExitCondition(typeof(InitialSceneLoadingSystem));
            }
        }

        private void RequestForResource(IUnityAssetManager assetManager)
        {
            _levelManager.GetRequests(_sceneRequests, _goRequests, _lightmapsRequests);

            foreach (var request in _sceneRequests)
            {
                assetManager.LoadSceneAsync(request, true);
            }

            foreach (var request in _goRequests)
            {
                _levelManager.LoadResource("InitialSceneLoadingSystem", assetManager, request);
            }

            foreach (var request in _lightmapsRequests)
            {
                MeshRenderer mr = null;
                List<AssetInfo> infos = new List<AssetInfo>();
                foreach (var ex in request)
                {
                    infos.Add(ex.asset);
                    if (mr == null) mr = ex.data;
                }
                assetManager.LoadAssetsAsync(mr, infos, _levelManager.LightmapsLoadedWrapper);
            }

            _sceneRequests.Clear();
            _goRequests.Clear();
            _lightmapsRequests.Clear();
        }
    }
}