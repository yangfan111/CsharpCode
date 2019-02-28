using System;
using System.Collections.Generic;
using App.Shared.Configuration;
using App.Shared.SceneManagement.Basic;
using App.Shared.SceneManagement.Streaming;
using Core.SceneManagement;
using Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Utils.Singleton;
using Object = System.Object;

namespace App.Shared.SceneManagement
{
    public class LevelManager : ILevelManager, ISceneResourceRequestHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(LevelManager));

        private IUnityAssetManager _assetManager;
        public LevelManager(IUnityAssetManager assetManager)
        {
            _assetManager = assetManager;

            SceneManager.sceneLoaded += SceneLoadedWrapper;
            SceneManager.sceneUnloaded += SceneUnloadedWrapper;
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= SceneLoadedWrapper;
            SceneManager.sceneUnloaded -= SceneUnloadedWrapper;
            SceneLoaded = null;
            SceneUnloaded = null;
            GoLoaded = null;
            AfterGoLoaded = null;
            BeforeGoUnloaded = null;
            GoUnloaded = null;
        }

        #region ILevelManager

        public event Action<Scene, LoadSceneMode> SceneLoaded;
        public event Action<Scene> SceneUnloaded;
        public event Action<UnityObject> GoLoaded;
        public event Action<UnityObject> AfterGoLoaded;
        public event Action<UnityObject> BeforeGoUnloaded;
        public event Action<UnityObject> GoUnloaded;
        
        private OriginStatus _status = new OriginStatus();
        public OriginStatus UpdateOrigin(Vector3 pos)
        {
            _sceneManager.UpdateOrigin(pos, _status);
            return _status;
        }

        public void GoLoadedWrapper(string source, UnityObject unityObj)
        {
            if (GoLoaded != null)
            {
                GoLoaded.Invoke(unityObj);
            }

            if (AfterGoLoaded != null)
            {
                AfterGoLoaded(unityObj);
            }

            --NotFinishedRequests;
        }

        public void GetRequests(List<AssetInfo> sceneRequests, List<AssetInfo> goRequests)
        {
            if (_cachedLoadSceneRequest.Count != 0)
            {
                sceneRequests.AddRange(_cachedLoadSceneRequest);
                _cachedLoadSceneRequest.Clear();
            }

            if (_cachedLoadGoRequest.Count != 0)
            {
                goRequests.AddRange(_cachedLoadGoRequest);
                _cachedLoadGoRequest.Clear();
            }

            ProcessUnloadRequests();
        }

        public int NotFinishedRequests { get; private set; }
        
        #endregion

        private ISceneResourceManager _sceneManager;

        private readonly List<AssetInfo> _cachedLoadSceneRequest = new List<AssetInfo>();
        private readonly List<AssetInfo> _cachedLoadGoRequest = new List<AssetInfo>();
        private readonly Queue<string> _cachedUnloadSceneRequest = new Queue<string>();
        private readonly Queue<UnityObject> _cachedUnloadGoRequest = new Queue<UnityObject>();

        private List<string> _fixedSceneNames;

        public void SetToWorldCompositionLevel(WorldCompositionParam param, IStreamingGoManager streamingGo)
        {
            _fixedSceneNames = param.FixedScenes;
            _sceneManager = new StreamingManager(this, streamingGo, SingletonManager.Get<StreamingLevelStructure>().Data,
                param, _fixedSceneNames.Count);

            RequestForFixedScenes(param.AssetBundleName);
        }

        public void SetToFixedScenesLevel(OnceForAllParam param)
        {
            _fixedSceneNames = param.FixedScenes;
            _sceneManager = new FixedScenesManager(this, param);
            
            RequestForFixedScenes(param.AssetBundleName);
        }

        public void SetAsapMode(bool value)
        {
            _sceneManager.SetAsapMode(value);
        }
        
        #region ISceneResourceRequestHandler
    
        public void AddUnloadSceneRequest(string sceneName)
        {
            _cachedUnloadSceneRequest.Enqueue(sceneName);
            ++NotFinishedRequests;
        }
        
        public void AddLoadSceneRequest(AssetInfo addr)
        {
            _cachedLoadSceneRequest.Add(addr);
            ++NotFinishedRequests;
        }

        public void AddLoadGoRequest(AssetInfo addr)
        {
            _cachedLoadGoRequest.Add(addr);
            ++NotFinishedRequests;
        }

        public void AddUnloadGoRequest(UnityObject unityObj)
        {
            _cachedUnloadGoRequest.Enqueue(unityObj);
            ++NotFinishedRequests;
        }
        
        #endregion

        private void SceneLoadedWrapper(Scene scene, LoadSceneMode mode)
        {
            if (_fixedSceneNames != null && _fixedSceneNames.Contains(scene.name))
                SceneManager.SetActiveScene(scene);

            if (SceneLoaded != null)
                SceneLoaded.Invoke(scene, mode);

            --NotFinishedRequests;

            _logger.InfoFormat("scene loaded {0}", scene.name);
        }

        private void SceneUnloadedWrapper(Scene scene)
        {
            if (SceneUnloaded != null)
                SceneUnloaded.Invoke(scene);
            
            --NotFinishedRequests;

            _logger.InfoFormat("scene unloaded {0}", scene.name);
        }

        private void ProcessUnloadRequests()
        {
            var goCount = _cachedUnloadGoRequest.Count;
            var sceneCount = _cachedUnloadSceneRequest.Count;

            for (int i = 0; i < goCount; i++)
            {
                var go = _cachedUnloadGoRequest.Dequeue();

                if (BeforeGoUnloaded != null)
                    BeforeGoUnloaded(go);

                if (GoUnloaded != null)
                    GoUnloaded.Invoke(go);

                _assetManager.Recycle(go);

                --NotFinishedRequests;
            }

            for (int i = 0; i < sceneCount; i++)
            {
                var scene = _cachedUnloadSceneRequest.Dequeue();
                SceneManager.UnloadSceneAsync(scene);
                _logger.InfoFormat("unload scene {0}", scene);
            }
        }

        private void RequestForFixedScenes(string bundleName)
        {
            foreach (var sceneName in _fixedSceneNames)
            {
                AddLoadSceneRequest(new AssetInfo
                {
                    BundleName = bundleName,
                    AssetName = sceneName
                });
            }
        }
    }
}