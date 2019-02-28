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

        private readonly WorldCompositionManager _worldComposition;
        private readonly IStreamingGoManager _streamingGo;

        private int _concurrentLimit = ConcurrentLimit;
        private const int AsapLimit = 500;
        private const int ConcurrentLimit = 5;
        private int _concurrentCount;

        private int _destroyingCount;
        private const int DestroyLimit = 20;
        
        private readonly Queue<LoadingGo> _goRequestQueue = new Queue<LoadingGo>();
        private readonly Queue<AssetInfo> _sceneRequestQueue = new Queue<AssetInfo>();
        private readonly Dictionary<AssetInfo, Queue<LoadingGo>> _loadingGoes = new Dictionary<AssetInfo, Queue<LoadingGo>>();

        private readonly Dictionary<string, Queue<UnityObject>> _toBeDestroyedGo = new Dictionary<string, Queue<UnityObject>>();
        private readonly Dictionary<string, Queue<UnityObject>> _unloadingScene = new Dictionary<string, Queue<UnityObject>>();

        public StreamingManager(ISceneResourceRequestHandler requestHandler,
                                IStreamingGoManager streamingGo,
                                StreamingData sceneDescription,
                                WorldCompositionParam param,
                                int preloadSceneCount)
        {
            _requestHandler = requestHandler;
            _sceneDescription = sceneDescription;
            _concurrentCount = preloadSceneCount;
            
            _worldComposition = new WorldCompositionManager(this, param);
            _streamingGo = streamingGo ?? new StreamingGoByScene();

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
        }

        public void SetAsapMode(bool value)
        {
            _concurrentLimit = value ? AsapLimit : ConcurrentLimit;
            _streamingGo.SetAsapMode(value);
        }

        #endregion
        
        #region IStreamingResourceHandler

        public void LoadScene(AssetInfo addr)
        {
            _sceneRequestQueue.Enqueue(addr);
            RequestForLoad();
        }

        public void UnloadScene(string sceneName)
        {
            if (_toBeDestroyedGo.ContainsKey(sceneName))
            {
                _unloadingScene.Add(sceneName, _toBeDestroyedGo[sceneName]);
                _toBeDestroyedGo.Remove(sceneName);
            }
            else
                _unloadingScene.Add(sceneName, new Queue<UnityObject>());

            _streamingGo.SceneUnloaded(sceneName);
            RequestForUnload();
        }

        public void LoadGo(int sceneIndex, int goIndex)
        {
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

            RequestForLoad();
        }

        public void UnloadGo(UnityObject go, int sceneIndex)
        {
            var sceneName = _sceneDescription.Scenes[sceneIndex].SceneName;
            if (_unloadingScene.ContainsKey(sceneName))
                _unloadingScene[sceneName].Enqueue(go);
            else
            {
                if (!_toBeDestroyedGo.ContainsKey(sceneName))
                    _toBeDestroyedGo.Add(sceneName, new Queue<UnityObject>());
                
                _toBeDestroyedGo[sceneName].Enqueue(go);
                RequestForUnload();
            }
        }
        
        #endregion
       
        private bool EnoughRoom()
        {
            return _concurrentCount < _concurrentLimit;
        }

        private void RequestForLoad()
        {
            while (EnoughRoom() && _sceneRequestQueue.Count > 0)
            {
                _requestHandler.AddLoadSceneRequest(_sceneRequestQueue.Dequeue());
                ++_concurrentCount;
            }

            while (EnoughRoom() && _goRequestQueue.Count > 0)
            {
                var loadingGo = _goRequestQueue.Dequeue();
                _requestHandler.AddLoadGoRequest(loadingGo.Addr);

                ++_concurrentCount;

                if (!_loadingGoes.ContainsKey(loadingGo.Addr))
                    _loadingGoes.Add(loadingGo.Addr, new Queue<LoadingGo>(16));

                _loadingGoes[loadingGo.Addr].Enqueue(loadingGo);
            }
        }

        private void RequestForUnload()
        {
            foreach (var pair in _toBeDestroyedGo)
            {
                while (_destroyingCount < DestroyLimit && pair.Value.Count > 0)
                {
                    _requestHandler.AddUnloadGoRequest(pair.Value.Dequeue());
                    ++_destroyingCount;
                }
            }

            string emptyScene = null;

            foreach (var pair in _unloadingScene)
            {
                while (_destroyingCount < DestroyLimit && pair.Value.Count > 0)
                {
                    _requestHandler.AddUnloadGoRequest(pair.Value.Dequeue());
                    ++_destroyingCount;
                }

                if (pair.Value.Count <= 0)
                {
                    emptyScene = pair.Key;
                    break;
                }
            }

            if (emptyScene != null)
            {
                _unloadingScene.Remove(emptyScene);
                _requestHandler.AddUnloadSceneRequest(emptyScene);
            }
        }
    
        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var sceneIndex = -1;
            if (_sceneIndex.ContainsKey(scene.name))
                sceneIndex = _sceneIndex[scene.name];
            
            --_concurrentCount;
            RequestForLoad();

            if (sceneIndex != -1)
            {
                _worldComposition.SceneLoaded(scene);
                _streamingGo.SceneLoaded(scene.name,
                    sceneIndex,
                    scene,
                    _sceneDescription.Scenes[sceneIndex],
                    _worldComposition.GetDimensionOfScene(scene.name));
            }
        }
        
        private void SceneUnloaded(Scene scene)
        {
            _worldComposition.SceneUnloaded(scene);
        }

        private void GoLoaded(UnityObject unityObj)
        {
            --_concurrentCount;
            if (_loadingGoes.ContainsKey(unityObj.Address))
            {
                var loadingGo = _loadingGoes[unityObj.Address].Dequeue();
                var sceneName = _sceneDescription.Scenes[loadingGo.SceneIndex].SceneName;

                if (_unloadingScene.ContainsKey(sceneName))
                {
                    _unloadingScene[sceneName].Enqueue(unityObj);

                    RequestForUnload();
                }
                else
                {
                    _streamingGo.GoLoaded(loadingGo.SceneIndex, loadingGo.GoIndex, unityObj);
                    var data = _sceneDescription.Scenes[loadingGo.SceneIndex].Objects[loadingGo.GoIndex];
                    var go = unityObj.AsGameObject;
                    go.transform.localPosition = data.Position;
                    go.transform.localEulerAngles = data.Rotation;
                    go.transform.localScale = data.Scale;

                    RequestForLoad();
                }
            }
        }

        private void GoUnloaded(UnityObject unityObj)
        {
            --_destroyingCount;

            RequestForUnload();
        }
    }
}