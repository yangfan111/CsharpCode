using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace App.Shared.SceneManagement.Streaming
{
    class WorldCompositionManager
    {
        enum SceneLoadingStatus
        {
            Unloaded, Unloading, Loading, Loaded
        }

        class SceneNode
        {
            public Vector2 Center;
            public SceneLoadingStatus Status;
            public string SceneName;
            public Vector4 Dimension;
        }
        
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(WorldCompositionManager));

        private readonly WorldCompositionParam _param;
        private readonly IStreamingResourceHandler _streamingManager;

        private readonly SceneNode[,] _sceneLoadingStatus;
        private readonly Dictionary<string, SceneNode> _sceneNameToScene;

        public WorldCompositionManager(IStreamingResourceHandler streamingManager, WorldCompositionParam param)
        {
            _streamingManager = streamingManager;
            _param = param;
            
            _sceneNameToScene = new Dictionary<string, SceneNode>();
            _sceneLoadingStatus = new SceneNode[_param.TerrainDimension, _param.TerrainDimension];

            for (int i = 0; i < _param.TerrainDimension; i++)
            {
                for (int j = 0; j < param.TerrainDimension; j++)
                {
                    var node = new SceneNode
                    {
                        Center = new Vector2(i + 0.5f, j + 0.5f),
                        Status = SceneLoadingStatus.Unloaded,
                        SceneName = string.Format(_param.TerrainNamePattern, i, j)
                    };
                    node.Dimension = new Vector4(
                        node.Center.x * _param.TerrainSize + _param.TerrainMin.x,
                        _param.TerrainSize * 0.5f,
                        node.Center.y * _param.TerrainSize + _param.TerrainMin.z,
                        _param.TerrainSize);

                    _sceneLoadingStatus[i, j] = node;
                    _sceneNameToScene.Add(node.SceneName, node);
                }
            }

            // outside parameter measured in grid distance between player and border
            // since center is used, extra 0.5 need to be added
            _param.LoadRadiusInGrid += 0.5f;
            _param.UnloadRadiusInGrid += 0.5f;
        }

        public void UpdateOrigin(Vector3 pos)
        {
            var gridCoordinate = ToGridCoordinate(pos);
            for (int i = 0; i < _param.TerrainDimension; i++)
            {
                for (int j = 0; j < _param.TerrainDimension; j++)
                {
                    var node = _sceneLoadingStatus[i, j];

                    switch (node.Status)
                    {
                        case SceneLoadingStatus.Loaded:
                            if (IsSceneShouldBeInvisible(gridCoordinate, node.Center))
                            {
                                UnloadScene(node.SceneName);
                                node.Status = SceneLoadingStatus.Unloading;
                                _logger.InfoFormat("unload scene {0}/{1}/{2}/{3}", node.SceneName, pos, gridCoordinate, node.Center);
                            }

                            break;
                        case SceneLoadingStatus.Unloaded:
                            if (IsSceneShouldBeVisible(gridCoordinate, node.Center))
                            {
                                LoadScene(node.SceneName);
                                node.Status = SceneLoadingStatus.Loading;
                                _logger.InfoFormat("load scene {0}/{1}/{2}/{3}", node.SceneName, pos, gridCoordinate, node.Center);
                            }

                            break;
                    }
                }
            }
        }
        
        public void SceneLoaded(Scene scene)
        {
            if (_sceneNameToScene.ContainsKey(scene.name))
            {
                var node = _sceneNameToScene[scene.name];
                node.Status = SceneLoadingStatus.Loaded;
            }
        }

        public void SceneUnloaded(Scene scene)
        {
            if (_sceneNameToScene.ContainsKey(scene.name))
            {
                var node = _sceneNameToScene[scene.name];
                node.Status = SceneLoadingStatus.Unloaded;
            }
        }

        public Vector4 GetDimensionOfScene(string sceneName)
        {
            if (_sceneNameToScene.ContainsKey(sceneName))
            {
                return _sceneNameToScene[sceneName].Dimension;
            }
            
            return Vector4.zero;
        }

        private Vector2 ToGridCoordinate(Vector3 pos)
        {
            return new Vector2((pos.x - _param.TerrainMin.x) / _param.TerrainSize,
                               (pos.z - _param.TerrainMin.z) / _param.TerrainSize);
        }

        private bool IsSceneShouldBeVisible(Vector2 gridCoordinate, Vector2 sceneCenter)
        {
            return Math.Abs(gridCoordinate.x - sceneCenter.x) <= _param.LoadRadiusInGrid
                   && Math.Abs(gridCoordinate.y - sceneCenter.y) <= _param.LoadRadiusInGrid;
        }

        private bool IsSceneShouldBeInvisible(Vector2 gridCoordinate, Vector2 sceneCenter)
        {
            return Math.Abs(gridCoordinate.x - sceneCenter.x) >= _param.UnloadRadiusInGrid
                   || Math.Abs(gridCoordinate.y - sceneCenter.y) >= _param.UnloadRadiusInGrid;
        }

        private void LoadScene(string sceneName)
        {
            _streamingManager.LoadScene(new AssetInfo
            {
                BundleName = _param.AssetBundleName,
                AssetName = sceneName
            });
        }

        private void UnloadScene(string sceneName)
        {
            _streamingManager.UnloadScene(sceneName);
        }
    }
}