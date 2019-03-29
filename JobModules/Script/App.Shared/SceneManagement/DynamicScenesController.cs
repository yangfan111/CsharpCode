using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Utils.AssetManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Utils;
using Shared.Scripts.RuntimeScripts;

namespace App.Shared.SceneManagement
{
    public class DynamicScenesController : SceneAbstractController<DynamicScenesController>, ISceneManagementEvent
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DynamicScenesController));

        private float _loadRadiusInGrid;
        private float _unloadRadiusInGrid;
        private int _gridSize;
        private int _gridCountInOneDimension;

        private Vector3 _basePosition;
        private string _sceneBaseName;
        private string _assetBundleName;
        private Regex _scenePositionRegex;

        public Action AllSceneLoaded;

        private SceneNode[,] _sceneLoadMap;
        private List<string> _additiveScenes;
        private int _loadingScene;
        private int _unloadingScene;

        public event SceneLoadedEventHandler SceneLoadedEvent;
        public event SceneUnloadEventedHandler SceneUnloadedEvent;

        private List<SceneRequest> _managementRequest = new List<SceneRequest>();

        private PositionRelatedEffectUpdater _effectUpdater;

        enum SceneState
        {
            Unloaded,
            Unloading,
            Loading,
            Loaded
        }


        public Vector3 BasePosition
        {
            get { return this._basePosition; }
        }
        struct SceneNode
        {
            public Vector2 Center;
            public SceneState State;
            public Scene Scene;
            public bool Active;
        }

        public DynamicScenesController()
        {
        }

        private void Clear()
        {
            _loadingScene = _unloadingScene = 0;
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneUnloaded -= SceneUnloaded;
        }

        public void SetParameters(string sceneBaseName,
                                  string bundleName,
                                  Vector3 basePosition,
                                  int gridSize,
                                  int gridCountInOneDimension,
                                  List<string> additiveScenes)
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
            SceneManager.sceneLoaded += SceneLoaded;

            _sceneBaseName = sceneBaseName;
            _scenePositionRegex = new Regex(_sceneBaseName.Replace("{0}", "(\\d+)").Replace("{1}", "(\\d+)"));

            _assetBundleName = bundleName;
            _basePosition = basePosition;
            _gridSize = gridSize;

            _gridCountInOneDimension = gridCountInOneDimension;
            _sceneLoadMap = new SceneNode[_gridCountInOneDimension, _gridCountInOneDimension];

            for (int i = 0; i < _gridCountInOneDimension; i++)
            {
                for (int j = 0; j < _gridCountInOneDimension; j++)
                {
                    _sceneLoadMap[i, j].Center = new Vector2(i + 0.5f, j + 0.5f);
                    _sceneLoadMap[i, j].State = SceneState.Unloaded;
                    _sceneLoadMap[i, j].Active = false;
                }
            }

            _additiveScenes = additiveScenes;
        }


        // Along any axis, there will be at least 'loadRadiusInGrid' scenes and at most
        // 'loadRadiusInGrid + unloadRedundanceInGrid' scenes loaded except the scene where player in.
        // Because distance measured between center of the scene and the player, 0.5 will be added
        // If unloadRedundanceInGrid == 0, a critical situation that a slight move forward and backward
        // will load/unload scene rapidly will happen
        public void SetRadius(float loadRadiusInGrid, float unloadRedundanceInGrid)
        {
            _loadRadiusInGrid = loadRadiusInGrid + 0.5f;
            _unloadRadiusInGrid = _loadRadiusInGrid + unloadRedundanceInGrid;
        }

        public void InitialUpdate(Vector3 newPosition)
        {
            if (_additiveScenes != null)
            {
                foreach (var v in _additiveScenes)
                {
                    _managementRequest.Add(new SceneRequest
                    {
                        IsLoad = true,
                        Address = new AssetInfo
                        {
                            BundleName = _assetBundleName,
                            AssetName = v
                        },
                        IsAdditive = true
                    });

                    _loadingScene++;
                }
            }

            Update(newPosition);
        }

        private Vector2 TranslatePosition(Vector3 realPos)
        {
            return  new Vector2(realPos.x - _basePosition.x, realPos.z - _basePosition.z);
        }

        public void Update(Vector3 newPosition)
        {
            Vector2 currentPosition = TranslatePosition(newPosition);

            LoadAndUnloadCheck(currentPosition);
        }

        public void SetState(int index1, int index2, bool active)
        {
            if (_sceneLoadMap[index1, index2].State == SceneState.Loaded && _sceneLoadMap[index1, index2].Active != active)
            {
                _sceneLoadMap[index1, index2].Active = active;
                foreach (var gameObject in _sceneLoadMap[index1, index2].Scene.GetRootGameObjects())
                {
                    var terrain = gameObject.GetComponent<Terrain>();
                    if (terrain != null)
                    {
                        terrain.drawTreesAndFoliage = active;
                    }
                }
            }
        }

        public List<SceneRequest> ManagermentRequest { get { return _managementRequest; } }
        public void ResetRequest()
        {
            if (_managementRequest.Count != 0)
            {
                _managementRequest.Clear();
            }
        }

        public IUpdatePositionRelatedEffect GetPositionRelatedEffectUpdater()
        {
            return _effectUpdater;
        }

        public void SetTreeDistance(float distance)
        {
            if (null == _sceneLoadMap)
            {
                Logger.Error("SceneLoadMap is null !");
                return;
            }

            foreach (var sceneNode in _sceneLoadMap)
            {
                if (sceneNode.State == SceneState.Loaded)
                {
                    foreach (var gameObject in sceneNode.Scene.GetRootGameObjects())
                    {
                        var terrain = gameObject.GetComponent<Terrain>();
                        if (terrain != null)
                        {
                            terrain.treeDistance = distance;
                        }
                    }
                }
            }
        }

        public void SetGrassDensity(float distance)
        {
            if (null == _sceneLoadMap)
            {
                Logger.Error("SceneLoadMap is null !");
                return;
            }

            foreach (var sceneNode in _sceneLoadMap)
            {
                if (sceneNode.State == SceneState.Loaded)
                {
                    foreach (var gameObject in sceneNode.Scene.GetRootGameObjects())
                    {
                        var terrain = gameObject.GetComponent<Terrain>();
                        if (terrain != null)
                        {
                            terrain.detailObjectDensity = distance;
                        }
                    }
                }
            }
        }

        public float GetTreeDistance()
        {
            if (null == _sceneLoadMap)
            {
                Logger.Error("SceneLoadMap is null !");
                return -1;
            }
            foreach (var sceneNode in _sceneLoadMap)
            {
                if (sceneNode.State == SceneState.Loaded)
                {
                    foreach (var gameObject in sceneNode.Scene.GetRootGameObjects())
                    {
                        var terrain = gameObject.GetComponent<Terrain>();
                        if (terrain != null)
                        {
                            return terrain.treeDistance;
                        }
                    }
                }
            }
            return -1;
        }

        public float GetGrassDensity()
        {
            if (null == _sceneLoadMap)
            {
                Logger.Error("SceneLoadMap is null !");
                return -1;
            }
            foreach (var sceneNode in _sceneLoadMap)
            {
                if (sceneNode.State == SceneState.Loaded)
                {
                    foreach (var gameObject in sceneNode.Scene.GetRootGameObjects())
                    {
                        var terrain = gameObject.GetComponent<Terrain>();
                        if (terrain != null)
                        {
                            return terrain.detailObjectDensity;
                        }
                    }
                }
            }
            return -1;
        }

        private bool IsTerrainNeedToUnload(float offsetInGridX,float offsetInGridY,SceneNode sceneNode)
        {
            if (Mathf.Abs(offsetInGridX - sceneNode.Center.x) > _unloadRadiusInGrid
                || Mathf.Abs(offsetInGridY - sceneNode.Center.y) > _unloadRadiusInGrid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsTerrainNeedToLoad(float offsetInGridX, float offsetInGridY, SceneNode sceneNode)
        {
            if (Mathf.Abs(offsetInGridX - sceneNode.Center.x) <= _loadRadiusInGrid
                && Mathf.Abs(offsetInGridY - sceneNode.Center.y) <= _loadRadiusInGrid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UnloadSceneNode(string blockName,ref SceneNode sceneNode)
        {
            _managementRequest.Add(new SceneRequest
            {
                IsLoad = false,
                Address = new AssetInfo
                {
                    AssetName = blockName
                }
            });

            sceneNode.State = SceneState.Unloading;
            _unloadingScene++;
        }

        private void LoadSceneNode(string blockName, ref SceneNode sceneNode)
        {
            _managementRequest.Add(new SceneRequest
            {
                IsLoad = true,
                Address = new AssetInfo
                {
                    BundleName = _assetBundleName,
                    AssetName = blockName
                },
                IsAdditive = true
            });

            sceneNode.State = SceneState.Loading;
            _loadingScene++;
        }
        private void LoadAndUnloadCheck(Vector2 currentOffset)
        {
            float offsetInGridX = currentOffset.x / _gridSize;
            float offsetInGridY = currentOffset.y / _gridSize;

            for (int i = 0; i < _gridCountInOneDimension; i++)
            {
                for (int j = 0; j < _gridCountInOneDimension; j++)
                {
                    if (_sceneLoadMap[i, j].State == SceneState.Loaded)
                    {
                        if(IsTerrainNeedToUnload(offsetInGridX,offsetInGridY,_sceneLoadMap[i, j]))
                        {
                            UnloadSceneNode(string.Format(_sceneBaseName, i, j), ref _sceneLoadMap[i, j]);
                        }
                    }
                    else if (_sceneLoadMap[i, j].State == SceneState.Unloaded)
                    {
                        if(IsTerrainNeedToLoad(offsetInGridX,offsetInGridY,_sceneLoadMap[i, j]))
                        {
                            LoadSceneNode(string.Format(_sceneBaseName, i, j), ref _sceneLoadMap[i, j]);
                        }
                    }
                }
            }
        }

        public bool IsAllVisibleTerrianLoaded(Vector3 currentPos)
        {
            Vector2 currentOffset = TranslatePosition(currentPos);
            float offsetInGridX = currentOffset.x / _gridSize;
            float offsetInGridY = currentOffset.y / _gridSize;
            for (int i = 0; i < _gridCountInOneDimension; i++)
            {
                for (int j = 0; j < _gridCountInOneDimension; j++)
                {
                    SceneNode sceneNode = _sceneLoadMap[i, j];
                   
                    if(IsTerrainNeedToLoad(offsetInGridX,offsetInGridY,sceneNode) && (sceneNode.State != SceneState.Loaded))
                    {
                        return false;
                    }
                    
                }
            }

            return true;
        } 

        private void SceneLoaded(Scene scene, LoadSceneMode modee)
        {
            var match = _scenePositionRegex.Match(scene.name);
            if (match.Success)
            {
                int index1 = Convert.ToInt32(match.Groups[1].Value);
                int index2 = Convert.ToInt32(match.Groups[2].Value);

                _sceneLoadMap[index1, index2].State = SceneState.Loaded;
                _sceneLoadMap[index1, index2].Scene = scene;
                _sceneLoadMap[index1, index2].Active = true;


                foreach (var gameObject in _sceneLoadMap[index1, index2].Scene.GetRootGameObjects())
                {
                    var terrain = gameObject.GetComponent<Terrain>();
                    if (terrain != null)
                    {
                        Type t = Type.GetType("ArtPlugins.TerrainProxy, Assembly-CSharp");
                        if (t != null)
                        {
                            terrain.gameObject.AddComponent(t);
                            Logger.Debug("ArtPlugins.TerrainProxy, Assembly-CSharp Founded");
                        }
                        else
                            Logger.Debug("ArtPlugins.TerrainProxy, Assembly-CSharp Not Founded");
                    }
                }



                if (onSceneLoaded != null)
                {
                    Logger.DebugFormat("onSceneLoaded Terrain {0} {1}", index1, index2);
                    onSceneLoaded(index1, index2, _sceneLoadMap[index1, index2].Scene);
                }

                _loadingScene--;

                if (SceneLoadedEvent != null)
                {
                    var center = _basePosition + new Vector3(_gridSize * (index1 + 0.5f), 0, _gridSize * (index2 + 0.5f));
                    // 预设：场景高度在0~1000m之间
                    float heightScope = 1000;
                    center.y = heightScope * 0.5f;
                    SceneLoadedEvent(scene, center, _gridSize > heightScope ? _gridSize : heightScope);
                }
            }

            if (_additiveScenes != null && _additiveScenes.Contains(scene.name))
            {
                SceneManager.SetActiveScene(scene);
                Logger.InfoFormat("SET ACTIVE SCENE: {0}", scene.name);
                foreach (var go in scene.GetRootGameObjects())
                {
                    var comp = go.GetComponentInChildren<PositionRelatedEffect>();
                    if (comp != null)
                        _effectUpdater = new PositionRelatedEffectUpdater(comp);

                    foreach (var v in go.GetComponentsInChildren<Camera>())
                    {
                        // 禁用相机，保证角色动画不更新
                        if (SharedConfig.IsServer)
                            v.enabled = false;
                        else
                        {
                            Type t = Type.GetType("ArtPlugins.GQS_Bind_Camera, Assembly-CSharp");
                            if (t != null)
                            {
                                v.gameObject.AddComponent(t);
                            }
                            else
                            {
                                Logger.Error("ArtPlugins.GQS_Bind_Camera is null ??? !!!");
                            }
                        }
                            
                        v.useOcclusionCulling = SharedConfig.EnableOC;
                    }
                }
                _loadingScene--;
            }

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                var terrain = gameObject.GetComponent<Terrain>();
                if (terrain != null)
                {
                    terrain.treeDistance = 800;
                }
            }

            Logger.InfoFormat("Scene loaded: {0}, remain loading scene: {1}", scene.name, _loadingScene);

            if (_loadingScene == 0 && AllSceneLoaded != null)
            {
                AllSceneLoaded();
            }
        }

        private void SceneUnloaded(Scene scene)
        {
            Logger.DebugFormat("Scene unloaded: {0}", scene.name);

            var match = _scenePositionRegex.Match(scene.name);
            if (match.Success)
            {
                int index1 = Convert.ToInt32(match.Groups[1].Value);
                int index2 = Convert.ToInt32(match.Groups[2].Value);

                _sceneLoadMap[index1, index2].State = SceneState.Unloaded;

                _unloadingScene--;

                if (_unloadingScene == 0)
                {
                    Resources.UnloadUnusedAssets();
                }

                if (onSceneUnloaded != null)
                    onSceneUnloaded(index1, index2);

                if (SceneUnloadedEvent != null)
                    SceneUnloadedEvent(scene);
            }
        }


        protected override void OnDispose()
        {
            Clear();
        }
        
        #region Debug

        public void SetVegetationActive(bool value)
        {
            foreach (var scene in _sceneLoadMap)
            {
                if (scene.State == SceneState.Loaded)
                {
                    foreach (var gameObject in scene.Scene.GetRootGameObjects())
                    {
                        var terrain = gameObject.GetComponent<Terrain>();
                        if (terrain != null)
                            terrain.drawTreesAndFoliage = value;
                    }
                }
            }
        }

        public void SetHeightmapActive(bool value)
        {
            foreach (var scene in _sceneLoadMap)
            {
                if (scene.State == SceneState.Loaded)
                {
                    foreach (var gameObject in scene.Scene.GetRootGameObjects())
                    {
                        var terrain = gameObject.GetComponent<Terrain>();
                        if (terrain != null)
                            terrain.drawHeightmap = value;
                    }
                }
            }           
        }
        
        #endregion
    }
}
