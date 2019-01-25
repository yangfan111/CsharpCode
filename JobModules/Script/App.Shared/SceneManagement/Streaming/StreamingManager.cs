using System;
using System.Collections.Generic;
using Core.SceneManagement;
using Core.Utils;
using Shared.Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Object = UnityEngine.Object;
using GoWrapper = Utils.AssetManager.UnityObjectWrapper<UnityEngine.GameObject>;

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
        private const int DestroyLimit = 10;
        
        private readonly Queue<LoadingGo> _goRequestQueue = new Queue<LoadingGo>();
        private readonly Queue<AssetInfo> _sceneRequestQueue = new Queue<AssetInfo>();
        private readonly Dictionary<AssetInfo, Queue<LoadingGo>> _loadingGoes = new Dictionary<AssetInfo, Queue<LoadingGo>>();

        private readonly Queue<GoWrapper> _toBeDestroyedGo = new Queue<GoWrapper>();
        private readonly Dictionary<string, Queue<GoWrapper>> _unloadingScene = new Dictionary<string, Queue<GoWrapper>>();

        public StreamingManager(ISceneResourceRequestHandler requestHandler,
                                IStreamingGoManager streamingGo,
                                StreamingData sceneDescription,
                                WorldCompositionParam param)
        {
            _requestHandler = requestHandler;
            _sceneDescription = sceneDescription;
            
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
            RequestForScene();
        }

        public void UnloadScene(string sceneName)
        {
            _unloadingScene.Add(sceneName, new Queue<GoWrapper>());
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

            RequestForGo();
        }

        public void UnloadGo(GoWrapper go, int sceneIndex)
        {
            var sceneName = _sceneDescription.Scenes[sceneIndex].SceneName;
            if (_unloadingScene.ContainsKey(sceneName))
                _unloadingScene[sceneName].Enqueue(go);
            else
            {
                _toBeDestroyedGo.Enqueue(go);
                RequestForUnload();
            }
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
                ++_concurrentCount;
            }
        }
        
        private void RequestForGo()
        {
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
            while (_destroyingCount < DestroyLimit && _toBeDestroyedGo.Count > 0)
            {
                var go = _toBeDestroyedGo.Dequeue();
                _requestHandler.AddUnloadGoRequest(go);
                ++_destroyingCount;
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
            RequestForScene();

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

        private void GoLoaded(UnityObjectWrapper<GameObject> go)
        {
            --_concurrentCount;
            if (_loadingGoes.ContainsKey(go.Address))
            {
                var loadingGo = _loadingGoes[go.Address].Dequeue();
                var sceneName = _sceneDescription.Scenes[loadingGo.SceneIndex].SceneName;

                if (_unloadingScene.ContainsKey(sceneName))
                {
                    _toBeDestroyedGo.Enqueue(go);

                    RequestForUnload();
                }
                else
                {
                    _streamingGo.GoLoaded(loadingGo.SceneIndex, loadingGo.GoIndex, go);
                    var data = _sceneDescription.Scenes[loadingGo.SceneIndex].Objects[loadingGo.GoIndex];
                    go.Value.transform.localPosition = data.Position;
                    go.Value.transform.localEulerAngles = data.Rotation;
                    go.Value.transform.localScale = data.Scale;

                    RequestForGo();
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