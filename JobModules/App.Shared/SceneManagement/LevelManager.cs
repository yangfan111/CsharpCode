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

        public LevelManager()
        {
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
            GoUnloaded = null;
        }

        #region ILevelManager

        public event Action<Scene, LoadSceneMode> SceneLoaded;
        public event Action<Scene> SceneUnloaded;
        public event Action<UnityObjectWrapper<GameObject>> GoLoaded;
        public event Action<UnityObjectWrapper<GameObject>> GoUnloaded;
        
        private OriginStatus _status = new OriginStatus();
        public OriginStatus UpdateOrigin(Vector3 pos)
        {
            _sceneManager.UpdateOrigin(pos, _status);
            return _status;
        }

        public void GoLoadedWrapper(object nul, UnityObjectWrapper<GameObject> go)
        {
            if (GoLoaded != null)
            {
                GoLoaded.Invoke(go);
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

            DestroyGo();
        }

        public int NotFinishedRequests { get; private set; }
        
        #endregion

        private ISceneResourceManager _sceneManager;

        private List<AssetInfo> _cachedLoadSceneRequest = new List<AssetInfo>();
        private List<AssetInfo> _cachedLoadGoRequest = new List<AssetInfo>();
        private Queue<UnityObjectWrapper<GameObject>> _cachedUnloadGoRequest = new Queue<UnityObjectWrapper<GameObject>>();

        private List<string> _fixedSceneNames;

        public void SetToWorldCompositionLevel(WorldCompositionParam param, IStreamingGoManager streamingGo)
        {
            // compromise for deadline
            _sceneManager = new StreamingManager(this, null, streamingGo, SingletonManager.Get<StreamingLevelStructure>().Data, param);

            _fixedSceneNames = param.FixedScenes;
            RequestForFixedScenes(param.AssetBundleName);
        }

        public void SetToFixedScenesLevel(OnceForAllParam param)
        {
            _sceneManager = new FixedScenesManager(this, param);
            
            _fixedSceneNames = param.FixedScenes;
            RequestForFixedScenes(param.AssetBundleName);
        }

        public void SetAsapMode(bool value)
        {
            _sceneManager.SetAsapMode(value);
        }
        
        #region ISceneResourceRequestHandler
    
        public void AddUnloadSceneRequest(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
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

        public void AddUnloadGoRequest(UnityObjectWrapper<GameObject> go)
        {
            _cachedUnloadGoRequest.Enqueue(go);
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
        }

        private void SceneUnloadedWrapper(Scene scene)
        {
            if (SceneUnloaded != null)
                SceneUnloaded.Invoke(scene);
            
            --NotFinishedRequests;
        }

        private void DestroyGo()
        {
            var count = _cachedUnloadGoRequest.Count;
            for (int i = 0; i < count; i++)
            {
                var go = _cachedUnloadGoRequest.Dequeue();

                if (go.Value != null)
                {
                    if (GoUnloaded != null)
                        GoUnloaded.Invoke(go);

                    UnityEngine.Object.Destroy(go.Value);
                }

                --NotFinishedRequests;
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