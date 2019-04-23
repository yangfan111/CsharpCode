using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using Core.OC;
using Core.SceneManagement;
using OC;
using UnityEngine;
using Utils.AssetManager;

namespace App.Client.GameModules.OC
{

    public class StreamOCParam : OCParam
    {
        public Vector3 InitPosition;

        public Vector3 TerrainMin;
        public int TerrainSize;
        public int TerrainDimension;
        public string TerrainNamePattern;


        public float LoadRadiusInGrid;
        public float UnloadRadiusInGrid;

        public ILevelManager LevelManager;
    }

    public class StreamSceneOCController : IOcclusionCullingController
    {

        enum PVSStatus
        {
            Loaded, Loading, UnLoaded, 
        }

        class PVSStat
        {
            public Vector2 Center;
            public PVSStatus Status;
            public string SceneName;
        }

        private readonly StreamOCParam _param;
        private MultiScene _scene;

        private readonly PVSStat[,] _pvsStats;
        public StreamSceneOCController(OCParam param)
        {
            OcclusionRunningState.HasPVSData = true;
            OcclusionRunningState.OcclusionEnabled = SharedConfig.EnableCustomOC;

            _param = param as StreamOCParam;
            _scene = new MultiScene(String.Empty, _param.TerrainNamePattern, _param.TerrainDimension, _param.TerrainSize, _param.OCData);
            _param.LevelManager.AfterGoLoaded += OnGoLoad;
            _param.LevelManager.BeforeGoUnloaded += OnGoUnload;

            _pvsStats = new PVSStat[_param.TerrainDimension, _param.TerrainDimension];
            for (int i = 0; i < _param.TerrainDimension; ++i)
            {
                for (int j = 0; j < _param.TerrainDimension; ++j)
                {

                    var stat = new PVSStat()
                    {
                        Center = new Vector2(i + 0.5f, j + 0.5f),
                        SceneName = String.Format(_param.TerrainNamePattern, i, j),
                        Status = PVSStatus.UnLoaded
                    };

                    _pvsStats[i, j] = stat;
                }
            }

            UpdatePVSCache(_param.InitPosition, true);
        }

        public void DoCulling(Vector3 position)
        {
            if (SharedConfig.EnableCustomOC)
            {
                UpdatePVSCache(position, false);
                _scene.DoCulling(position);
            }
            else
            {
                _scene.UndoDisabledObjects();
            }
        }

        private Vector2 ToGridCoordinate(Vector3 pos)
        {
            return new Vector2((pos.x - _param.TerrainMin.x) / _param.TerrainSize,
                (pos.z - _param.TerrainMin.z) / _param.TerrainSize);
        }

        private bool IsPVSShouldLoad(Vector2 gridCoordinate, Vector2 sceneCenter)
        {
            return Math.Abs(gridCoordinate.x - sceneCenter.x) <= _param.LoadRadiusInGrid
                   && Math.Abs(gridCoordinate.y - sceneCenter.y) <= _param.LoadRadiusInGrid;
        }

        private bool IsPVSShouldUnload(Vector2 gridCoordinate, Vector2 sceneCenter)
        {
            return Math.Abs(gridCoordinate.x - sceneCenter.x) >= _param.UnloadRadiusInGrid
                   || Math.Abs(gridCoordinate.y - sceneCenter.y) >= _param.UnloadRadiusInGrid;
        }

        private void UpdatePVSCache(Vector3 position, bool block)
        {
            var gridCoord = ToGridCoordinate(position);
            for (int i = 0; i < _param.TerrainDimension; ++i)
            {
                for (int j = 0; j < _param.TerrainDimension; ++j)
                {
                    var stat = _pvsStats[i, j];
                    switch (stat.Status)
                    {
                        case PVSStatus.Loaded:
                            if (IsPVSShouldUnload(gridCoord, stat.Center))
                            {
                                stat.Status = PVSStatus.UnLoaded;
                                _scene.Unload(i, j);
                            }
                            break;
                        case PVSStatus.UnLoaded:
                            if (IsPVSShouldLoad(gridCoord, stat.Center))
                            {
                                stat.Status = PVSStatus.Loading;
                                if (!_scene.Load(i, j, OnPVSLoaded, block))
                                {
                                    stat.Status = PVSStatus.UnLoaded;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void OnPVSLoaded(int x, int y)
        {
            var stat = _pvsStats[x, y];
            stat.Status = PVSStatus.Loaded;
        }

        private void OnGoLoad(UnityObject unityObj)
        {
            var go = unityObj.AsGameObject;
            if (go != null)
            {
                _scene.OnGameObjectLoad(go);
            }
        }

        private void OnGoUnload(UnityObject unityObj)
        {
            var go = unityObj.AsGameObject;
            if (go != null)
            {
                _scene.OnGameObjectUnload(go);
            }
        }


        public void Dispose()
        {

        }
    }
}
