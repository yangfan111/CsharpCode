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
        private readonly GpuInstancingTreeOnTerrain _treeManager;
        private Dictionary<string, Vector3> _minPosMap = new Dictionary<string, Vector3>();
        private Dictionary<string, List<string>> _sceneToTerrain = new Dictionary<string, List<string>>();
        
        private bool _enableGpui = true;
        private Dictionary<string, Terrain> _cachedTerrain = new Dictionary<string, Terrain>();
        private Dictionary<string, float> _cachedDetailDist = new Dictionary<string, float>();
        private Dictionary<string, float> _cachedTreeDist = new Dictionary<string, float>();

        private readonly List<string> _loadedTerrainNames = new List<string>();

        private int _loadingDataCount = 0;
        
        public bool IsReady
        {
            get { return _detailManager != null; }
//            get { return _detailManager != null && _treeManager != null; }
        }

        public TerrainRenderer()
        {
            var detailInstantiation = (ComputeShader) Resources.Load(Constants.CsPath.DetailInstantiationInResource);
            var treeInstantiation = (ComputeShader) Resources.Load(Constants.CsPath.TreeInstantiationInResource);
            var merge = (ComputeShader) Resources.Load(Constants.CsPath.MergeBufferInResource);
            var visibility = (ComputeShader) Resources.Load(Constants.CsPath.VisibilityDeterminationInResource);
            var sort = (ComputeShader) Resources.Load(Constants.CsPath.GpuSortInResource);

            if (merge != null && visibility != null)
            {
                if (detailInstantiation != null)
                    _detailManager = new GpuInstancingDetailOnTerrain(detailInstantiation, merge, visibility, sort);
                
//                if (treeInstantiation != null)
//                    _treeManager = new GpuInstancingTreeOnTerrain(treeInstantiation, merge, visibility);
            }
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
                        _cachedTreeDist.Add(terrain.name, terrain.treeDistance);
                    }

                    if (SharedConfig.EnableGpui)
                    {
                        terrain.detailObjectDistance = 0;
//                        terrain.treeDistance = 0;
                    }

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
//                    _treeManager.TerrainUnloaded(_minPosMap[terrain]);
                    _minPosMap.Remove(terrain);
                    _cachedTerrain.Remove(terrain);
                    _cachedDetailDist.Remove(terrain);
                    _cachedTreeDist.Remove(terrain);
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
                    var terrainData = _cachedTerrain[terrainName].terrainData;
                    var heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
                        terrainData.heightmapResolution);
                        
                    if (!_detailManager.TerrainLoaded(_cachedTerrain[terrainName], _cachedDetailDist[terrainName],
                        _minPosMap[terrainName], heightMap))
                        logger.ErrorFormat("{0} will not have grass", terrainName);
                        
//                    if (!_treeManager.TerrainLoaded(_cachedTerrain[terrainName], _cachedTreeDist[terrainName],
//                        _minPosMap[terrainName], heightMap))
//                        logger.ErrorFormat("{0} will not have tree", terrainName);
                }
                else
                    logger.ErrorFormat("wrong loaded terraindata name: {0}", terrainName);
            }
            else
            {
                if (_cachedTerrain.ContainsKey(terrainName))
                {
                    if (!_detailManager.TerrainLoaded(data, _cachedTerrain[terrainName], _cachedDetailDist[terrainName],
                        _minPosMap[terrainName]))
                        logger.ErrorFormat("{0} will not have grass", terrainName);
                        
//                    if (!_treeManager.TerrainLoaded(data, _cachedTerrain[terrainName], _cachedTreeDist[terrainName],
//                        _minPosMap[terrainName]))
//                        logger.ErrorFormat("{0} will not have tree", terrainName);
                }
            }
        }

        public bool IsLoadingEnd { get { return _loadingDataCount == 0; } }

        public void SetCamera(Camera cam)
        {
            if (_detailManager != null)
                _detailManager.SetRenderingCamera(cam);
            
//            if (_treeManager != null)
//                _treeManager.SetRenderingCamera(cam);
        }

        public void Draw()
        {
            if (SharedConfig.EnableGpui)
            {
                if (_hasTerrain)
                {
                    _detailManager.Draw();
//                    _treeManager.Draw();
                }
            }

            if (_enableGpui != SharedConfig.EnableGpui)
            {
                _enableGpui = SharedConfig.EnableGpui;
                if (!_enableGpui)
                {
                    foreach (var terrain in _cachedTerrain)
                    {
                        terrain.Value.detailObjectDistance = _cachedDetailDist[terrain.Key];
//                        terrain.Value.treeDistance = _cachedTreeDist[terrain.Key];
                    }
                }
                else
                {
                    foreach (var terrain in _cachedTerrain)
                    {
                        terrain.Value.detailObjectDistance = 0;
//                        terrain.Value.treeDistance = 0;
                    }
                }
            }
        }
    }
}