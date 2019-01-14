using System;
using System.Collections.Generic;
using Core.Utils;
using Shared.Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace App.Shared.SceneManagement.Streaming
{
    public class StreamingManager : StreamingSceneGoInstantiation
    {
        struct LoadingGo
        {
            public AssetInfo Addr;
            public int SceneIndex;
            public int GoIndex;
        }
        
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(StreamingManager));
        
        private readonly ResourceRequestCache _requestHandler;
        private readonly StreamingData _sceneDescription;

        private readonly Dictionary<string, int> _sceneIndex = new Dictionary<string, int>();
        private readonly Dictionary<string, StreamingScene> _scenes = new Dictionary<string, StreamingScene>();

        private bool _asap;
        private int _concurrentLimit = 2;
        private int _currentConcurrentCount;
        
        private Queue<LoadingGo> _goRequestQueue = new Queue<LoadingGo>();
        private Queue<AssetInfo> _sceneRequestQueue = new Queue<AssetInfo>();
        private List<LoadingGo> _loadingGoes = new List<LoadingGo>();

        public event Action<GameObject> GoInstantiated;

        public StreamingManager(ResourceRequestCache requestHandler, StreamingData sceneDescription)
        {
            _requestHandler = requestHandler;
            _sceneDescription = sceneDescription;
            
            var count = _sceneDescription.Scenes.Count;
            for (int i = 0; i < count; i++)
                _sceneIndex.Add(_sceneDescription.Scenes[i].SceneName, i);

            _requestHandler.SceneLoaded += SceneLoaded;
            _requestHandler.SceneUnloaded += SceneUnloaded;
        }

        public void Reset()
        {
            _requestHandler.SceneLoaded -= SceneLoaded;
            _requestHandler.SceneUnloaded -= SceneUnloaded;
        }

        public void SetConcurrentLimit(int value)
        {
            _concurrentLimit = value;
        }

        public void SetAsapMode(bool value)
        {
            _asap = value;
        }
        
        public void LoadScene(AssetInfo addr)
        {
            if (!_scenes.ContainsKey(addr.AssetName))
                _sceneRequestQueue.Enqueue(addr);
            else
                Logger.WarnFormat("load loaded scene: {0}", addr.AssetName);
        }

        public void UnloadScene(string sceneName)
        {
            if (_scenes.ContainsKey(sceneName))
                _requestHandler.AddUnloadSceneRequest(_scenes[sceneName].Scene);
            else
                Logger.WarnFormat("unload not loaded scene: {0}", sceneName);
        }

        public bool LoadGo(string sceneName, int goIndex)
        {
            if (!_scenes.ContainsKey(sceneName))
                return false;

            var sceneIndex = _sceneIndex[sceneName];
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

            return true;
        }

        public void UnloadGo(string sceneName, int goIndex)
        {
            if (_scenes.ContainsKey(sceneName))
                _scenes[sceneName].RemoveGo(goIndex);
        }

        public void Update()
        {
            RequestForScene();
            RequestForGo();
        }

        public void GoLoaded(GameObject go, AssetInfo addr)
        {
            var count = _loadingGoes.Count;
            for (int i = 0; i < count; i++)
            {
                if (_loadingGoes[i].Addr.Equals(addr))
                {
                    var sceneName = _sceneDescription.Scenes[_loadingGoes[i].SceneIndex].SceneName;
                    if (_scenes.ContainsKey(sceneName))
                    {
                        _scenes[sceneName].AddGo(go, _loadingGoes[i].GoIndex);
                        if (GoInstantiated != null)
                            GoInstantiated.Invoke(go);
                    }

                    _currentConcurrentCount--;
                    _loadingGoes.RemoveAt(i);
                    break;
                }
            }
        }
        
        private bool EnoughRoom()
        {
            return _asap || _currentConcurrentCount < _concurrentLimit;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!_scenes.ContainsKey(scene.name))
            {
                _scenes.Add(scene.name, new StreamingScene(scene));
                _currentConcurrentCount--;
            }
            else
                Logger.WarnFormat("load registered scene: {0}", scene.name);
        }

        private void SceneUnloaded(Scene scene)
        {
            if (_scenes.ContainsKey(scene.name))
            {
                var streamingScene = _scenes[scene.name];
                streamingScene.Clear();
                _scenes.Remove(scene.name);
            }
            else
                Logger.WarnFormat("unload not registered scene: {0}", scene.name);
        }

        private void RequestForScene()
        {
            while (EnoughRoom() && _sceneRequestQueue.Count > 0)
            {
                _requestHandler.AddLoadSceneRequest(_sceneRequestQueue.Dequeue());
                _currentConcurrentCount++;
            }
        }
        
        private void RequestForGo()
        {
            while (EnoughRoom() && _goRequestQueue.Count > 0)
            {
                var loadingGo = _goRequestQueue.Dequeue();
                _requestHandler.AddLoadGoRequest(loadingGo.Addr);
                _currentConcurrentCount++;

                if (!_asap)
                    _loadingGoes.Add(loadingGo);
            }
        }
    }
}