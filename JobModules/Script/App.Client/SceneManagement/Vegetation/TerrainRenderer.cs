using System;
using System.Collections.Generic;
using App.Client.GPUInstancing.Core;
using App.Client.GPUInstancing.Core.Utils;
using App.Shared;
using Core.SceneManagement;
using Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Object = UnityEngine.Object;

namespace App.Client.SceneManagement.Vegetation
{
    public class TerrainRenderer : ITerrainRenderer
    {
        static LoggerAdapter logger = new LoggerAdapter(typeof(TerrainRenderer));

        private bool _hasTerrain;

        private readonly GpuInstancingDetailOnTerrain _detailManager;
        private Dictionary<string, Vector3> _minPosMap = new Dictionary<string, Vector3>();
        private Dictionary<string, List<string>> _sceneToTerrain = new Dictionary<string, List<string>>();
        
        private bool _enableGpui = true;
        private Dictionary<string, Terrain> _cachedTerrain = new Dictionary<string, Terrain>();
        private Dictionary<string, float> _cachedDetailDist = new Dictionary<string, float>();

        private readonly List<string> _loadedTerrainNames = new List<string>();

        private int _loadingDataCount = 0;

        public TerrainRenderer()
        {
            _detailManager = new GpuInstancingDetailOnTerrain(
                (ComputeShader) Resources.Load(Constants.CsPath.DetailInstantiationInResource),
                (ComputeShader) Resources.Load(Constants.CsPath.MergeBufferInResource),
                (ComputeShader) Resources.Load(Constants.CsPath.VisibilityDeterminationInResource));
        }
        
        public void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!_sceneToTerrain.ContainsKey(scene.name))
                _sceneToTerrain.Add(scene.name, new List<string>());

            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                var terrain = rootGameObject.GetComponentInChildren<Terrain>();
                if (terrain != null)
                {
                    if (!_minPosMap.ContainsKey(terrain.name))
                    {
                        _sceneToTerrain[scene.name].Add(terrain.name);

                        _minPosMap.Add(terrain.name, terrain.transform.position);
                        _cachedTerrain.Add(terrain.name, terrain);
                        _cachedDetailDist.Add(terrain.name, terrain.detailObjectDistance);
                    }

                    if (SharedConfig.EnableGpui)
                        terrain.detailObjectDistance = 0;

                    _loadedTerrainNames.Add(terrain.name);
                    ++_loadingDataCount;

                    _hasTerrain = true;

                    break;
                }
            }
        }

        public void SceneUnloaded(Scene scene)
        {
            if (_sceneToTerrain.ContainsKey(scene.name))
            {
                foreach (var terrain in _sceneToTerrain[scene.name])
                {
                    _detailManager.TerrainUnloaded(_minPosMap[terrain]);
                    _minPosMap.Remove(terrain);
                    _cachedTerrain.Remove(terrain);
                    _cachedDetailDist.Remove(terrain);
                }

                _sceneToTerrain[scene.name].Clear();
            }
        }

        public void GoLoaded(UnityObject obj)
        {
        }

        public void GoUnloaded(UnityObject obj)
        {
        }

        public void GetTerrainDataNames(List<string> names)
        {
            if (_loadedTerrainNames.Count > 0)
            {
                names.AddRange(_loadedTerrainNames);
                _loadedTerrainNames.Clear();
            }
        }

        public void LoadedTerrainData(Object obj, UnityObject asset)
        {
            --_loadingDataCount;

            var terrainName = asset.Address.AssetName;
            var data = asset.As<TextAsset>();

            if (data == null)
            {
                if (_cachedTerrain.ContainsKey(terrainName))
                {
                    try
                    {
                        if (!_detailManager.TerrainLoaded(_cachedTerrain[terrainName], _cachedDetailDist[terrainName]))
                            logger.ErrorFormat("unmatch parameter in {0}", terrainName);
                    }
                    catch (Exception e)
                    {
                        logger.Error(terrainName + "\n" + e.Message + "\n" + e.StackTrace);
                        throw e;
                    }
                }
                else
                    logger.ErrorFormat("wrong loaded terraindata name: {0}", terrainName);
            }
            else
            {
                if (_cachedTerrain.ContainsKey(terrainName))
                {
                    if (!_detailManager.TerrainLoaded(data, _cachedTerrain[terrainName], _cachedDetailDist[terrainName]))
                        logger.ErrorFormat("unmatch parameter in {0}", terrainName);
                }
            }
        }

        public bool IsLoadingEnd { get { return _loadingDataCount == 0; } }

        public void SetCamera(Camera cam)
        {
            _detailManager.SetRenderingCamera(cam);
        }

        public void Draw()
        {
            if (SharedConfig.EnableGpui)
            {
                if (_hasTerrain)
                    _detailManager.Draw();
            }

            if (_enableGpui != SharedConfig.EnableGpui)
            {
                _enableGpui = SharedConfig.EnableGpui;
                if (!_enableGpui)
                {
                    foreach (var terrain in _cachedTerrain)
                    {
                        terrain.Value.detailObjectDistance = _cachedDetailDist[terrain.Key];
                    }
                }
                else
                {
                    foreach (var terrain in _cachedTerrain)
                    {
                        terrain.Value.detailObjectDistance = 0;
                    }
                }
            }
        }
    }
}