using System;
using System.Collections.Generic;
using Core.SceneManagement;
using Core.Utils;
using Shared.Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Object = UnityEngine.Object;

namespace App.Shared.SceneManagement.Streaming
{
    class StreamingManager : ISceneResourceManager, IStreamingResourceHandler
    {
        struct LoadingGo
        {
            public AssetInfo Addr;
            public int SceneIndex;
            public int GoIndex;
        }
        
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(StreamingManager));
        
        private readonly ISceneResourceRequestHandler _requestHandler;
        private readonly StreamingData _sceneDescription;

        private readonly Dictionary<string, int> _sceneIndex = new Dictionary<string, int>();
        private readonly Dictionary<int, LoadedScene> _scenes = new Dictionary<int, LoadedScene>();

        private readonly WorldCompositionManager _worldComposition;
        private readonly IStreamingGoManager _streamingGo;
        // compromise for deadline
        private readonly IStreamingGoManager _culling;

        private int _concurrentLimit = ConcurrentLimit;
        private const int AsapLimit = 500;
        private const int ConcurrentLimit = 5;
        private int _concurrentCount;

        private int _destroyingCount;
        private const int DestroyLimit = 10;
        
        private readonly Queue<LoadingGo> _goRequestQueue = new Queue<LoadingGo>();
        private readonly Queue<AssetInfo> _sceneRequestQueue = new Queue<AssetInfo>();
        private readonly List<LoadingGo> _loadingGoes = new List<LoadingGo>();

        private readonly Queue<UnityObjectWrapper<GameObject>> _toBeDestroyedGo = new Queue<UnityObjectWrapper<GameObject>>();
        private readonly Dictionary<int, Queue<int>> _unloadingScene = new Dictionary<int, Queue<int>>();

        public StreamingManager(ISceneResourceRequestHandler requestHandler,
                                IStreamingGoManager streamingGo,
                                IStreamingGoManager culling,
                                StreamingData sceneDescription,
                                WorldCompositionParam param)
        {
            _requestHandler = requestHandler;
            _sceneDescription = sceneDescription;
            
            _worldComposition = new WorldCompositionManager(this, param);
            _streamingGo = streamingGo ?? new StreamingGoByScene();
            _culling = culling;

            _streamingGo.SetResourceHandler(this);

            if (_sceneDescription != null)
            {
                var count = _sceneDescription.Scenes.Count;
                for (int i = 0; i < count; i++)
                    _sceneIndex.Add(_sceneDescription.Scenes[i].SceneName, i);
            }

            _requestHandler.SceneLoaded += SceneLoaded;
            _requestHandler.SceneUnloaded += SceneUnloaded;
            _requestHandler.GoLoaded += GoLoaded;
            _requestHandler.GoUnloaded += GoUnloaded;
        }

        #region ISceneResourceManager

        public void UpdateOrigin(Vector3 value, OriginStatus status)
        {
            _worldComposition.UpdateOrigin(value);
            _streamingGo.UpdateOrigin(value, status);
            if (_culling != null)
                _culling.UpdateOrigin(value, status);
        }

        public void SetAsapMode(bool value)
        {
            _concurrentLimit = value ? AsapLimit : ConcurrentLimit;
        }

        #endregion
        
        #region IStreamingResourceHandler

        public void LoadScene(AssetInfo addr)
        {
            if (_sceneIndex.ContainsKey(addr.AssetName))
            {
                var sceneIndex = _sceneIndex[addr.AssetName];

                if (!_scenes.ContainsKey(sceneIndex))
                {
                    _sceneRequestQueue.Enqueue(addr);
                    RequestForScene();
                }
                else
                    _logger.WarnFormat("load loaded scene: {0}", addr.AssetName);
            }
            else
            {
                _sceneRequestQueue.Enqueue(addr);
                RequestForScene();
                _logger.WarnFormat("not recognized scene: {0}", addr.AssetName);
            }
        }

        public void UnloadScene(string sceneName)
        {
            if (_sceneIndex.ContainsKey(sceneName))
            {
                var sceneIndex = _sceneIndex[sceneName];

                if (_scenes.ContainsKey(sceneIndex))
                {
                    _unloadingScene.Add(sceneIndex, new Queue<int>());
                    _streamingGo.SceneUnloaded(sceneName);
                    if (_culling != null)
                        _culling.SceneUnloaded(sceneName);
                    
                    RequestForUnload();
                }
                else
                    _logger.WarnFormat("unload not loaded scene: {0}", sceneName);
            }
            else
            {
                _streamingGo.SceneUnloaded(sceneName);
                if (_culling != null)
                    _culling.SceneUnloaded(sceneName);
                _requestHandler.AddUnloadSceneRequest(sceneName);
                _logger.WarnFormat("not recognized scene: {0}", sceneName);
            }
        }

        public bool LoadGo(int sceneIndex, int goIndex)
        {
            if (sceneIndex >= _sceneDescription.Scenes.Count)
                return false;

            var streamingObject = _sceneDescription.Scenes[sceneIndex].Objects[goIndex];
            _goRequestQueue.Enqueue(new LoadingGo
            {
                Addr = new AssetInfo
                {
                    BundleName = streamingObject.BundleName,
                    AssetName = streamingObject.AssetName
                },
                SceneIndex = sceneIndex,
                GoIndex = goIndex
            });

            RequestForGo();

            return true;
        }

        public void UnloadGo(int sceneIndex, int goIndex)
        {
            if (_unloadingScene.ContainsKey(sceneIndex))
                _unloadingScene[sceneIndex].Enqueue(goIndex);
            else if (_scenes.ContainsKey(sceneIndex))
            {
                _toBeDestroyedGo.Enqueue(_scenes[sceneIndex].RemoveGo(goIndex));
                RequestForUnload();
            }
            else
                _logger.WarnFormat("unload go from not loaded scene, scene index: {0} go index: {1}", sceneIndex, goIndex);
        }
        
        #endregion
       
        private bool EnoughRoom()
        {
            return _concurrentCount < _concurrentLimit;
        }

        private void RequestForScene()
        {
            while (EnoughRoom() && _sceneRequestQueue.Count > 0)
            {
                _requestHandler.AddLoadSceneRequest(_sceneRequestQueue.Dequeue());
                _concurrentCount++;
            }
        }
        
        private void RequestForGo()
        {
            while (EnoughRoom() && _goRequestQueue.Count > 0)
            {
                var loadingGo = _goRequestQueue.Dequeue();
                _requestHandler.AddLoadGoRequest(loadingGo.Addr);
                _concurrentCount++;

                _loadingGoes.Add(loadingGo);
            }
        }

        private void RequestForUnload()
        {
            while (_destroyingCount < DestroyLimit && _toBeDestroyedGo.Count > 0)
            {
                var go = _toBeDestroyedGo.Dequeue();
                _requestHandler.AddUnloadGoRequest(go);
                ++_destroyingCount;
            }

            int emptySceneIndex = -1;

            foreach (var pair in _unloadingScene)
            {
                var sceneIndex = pair.Key;
                while (_destroyingCount < DestroyLimit && pair.Value.Count > 0)
                {
                    var goIndex = pair.Value.Dequeue();
                    var go = _scenes[sceneIndex].RemoveGo(goIndex);
                    
                    _requestHandler.AddUnloadGoRequest(go);
                    ++_destroyingCount;
                }

                if (pair.Value.Count <= 0)
                {
                    emptySceneIndex = pair.Key;
                    break;
                }
            }

            if (emptySceneIndex != -1)
            {
                _unloadingScene.Remove(emptySceneIndex);
                _requestHandler.AddUnloadSceneRequest(_scenes[emptySceneIndex].Scene.name);
            }
        }
    
        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var sceneIndex = -1;
            if (_sceneIndex.ContainsKey(scene.name))
            {
                sceneIndex = _sceneIndex[scene.name];
                _scenes.Add(sceneIndex, new LoadedScene(scene, _sceneDescription.Scenes[sceneIndex]));

                _concurrentCount--;
                
                RequestForScene();
            }
            
            _worldComposition.SceneLoaded(scene);
            _streamingGo.SceneLoaded(scene.name,
                                     sceneIndex,
                                     scene,
                                     sceneIndex == -1 ? null : _sceneDescription.Scenes[sceneIndex],
                                     _worldComposition.GetDimensionOfScene(scene.name));
            if (_culling != null)
                _culling.SceneLoaded(scene.name,
                                     sceneIndex,
                                     scene,
                                     sceneIndex == -1 ? null : _sceneDescription.Scenes[sceneIndex],
                                     _worldComposition.GetDimensionOfScene(scene.name));
                
        }
        
        private void SceneUnloaded(Scene scene)
        {
            if (_sceneIndex.ContainsKey(scene.name))
            {
                var sceneIndex = _sceneIndex[scene.name];
                var loadedScene = _scenes[sceneIndex];

                loadedScene.Clear();
                _scenes.Remove(sceneIndex);
            }
            else
                _logger.WarnFormat("unload not registered scene: {0}", scene.name);
            
            _worldComposition.SceneUnloaded(scene);
        }

        private void GoLoaded(UnityObjectWrapper<GameObject> go)
        {
            var count = _loadingGoes.Count;
            for (int i = 0; i < count; i++)
            {
                if (_loadingGoes[i].Addr.Equals(go.Address))
                {
                    var sceneIndex = _loadingGoes[i].SceneIndex;
                    if (_scenes.ContainsKey(sceneIndex) && !_unloadingScene.ContainsKey(sceneIndex))
                    {
                        _scenes[sceneIndex].AddGo(go, _loadingGoes[i].GoIndex);
                        _streamingGo.GoLoaded(sceneIndex, _loadingGoes[i].GoIndex, go);
                        if (_culling != null)
                            _culling.GoLoaded(sceneIndex, _loadingGoes[i].GoIndex, go);
                    }
                    else
                    {
                        _toBeDestroyedGo.Enqueue(go);
                        RequestForUnload();
                    }

                    _concurrentCount--;
                    _loadingGoes.RemoveAt(i);
                    
                    RequestForGo();

                    return;
                }
            }
        }

        private void GoUnloaded(UnityObjectWrapper<GameObject> go)
        {
            --_destroyingCount;

            RequestForUnload();
        }
    }
}