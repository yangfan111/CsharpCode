using System.Collections.Generic;
using Core.SceneManagement;
using Shared.Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace App.Shared.SceneManagement.Streaming
{
    class StreamingGoByScene : IStreamingGoManager
    {
        private IStreamingResourceHandler _streamingResourceHandler;
        private readonly Dictionary<string, int> _sceneNameToId = new Dictionary<string, int>();
        private readonly Dictionary<int, List<UnityObject>> _loadedGoes = new Dictionary<int, List<UnityObject>>();
        private readonly Dictionary<int, GameObject> _goRoot = new Dictionary<int, GameObject>();

        public void SetResourceHandler(IStreamingResourceHandler handler)
        {
            _streamingResourceHandler = handler;
        }

        public void UpdateOrigin(Vector3 pos, OriginStatus status)
        {
        }

        public void SetAsapMode(bool value)
        {
        }

        public void SceneLoaded(string sceneName,
                                int sceneIndex,
                                Scene scene,
                                StreamingScene sceneStruct,
                                Vector4 sceneDimension)
        {
            if (sceneIndex < 0)
                return;

            if (!_loadedGoes.ContainsKey(sceneIndex))
            {
                _loadedGoes.Add(sceneIndex, new List<UnityObject>());
                _sceneNameToId.Add(sceneName, sceneIndex);
                
                var count = sceneStruct.Objects.Count;
                for (int i = 0; i < count; i++)
                {
                    _streamingResourceHandler.LoadGo(sceneIndex, i);
                }
                
                var go = new GameObject("StreamingRoot");
                SceneManager.MoveGameObjectToScene(go, scene);
                _goRoot.Add(sceneIndex, go);
            }
        }

        public void SceneUnloaded(string sceneName)
        {
            if (_sceneNameToId.ContainsKey(sceneName))
            {
                var sceneIndex = _sceneNameToId[sceneName];
                var goes = _loadedGoes[sceneIndex];

                var count = goes.Count;
                for (int i = 0; i < count; i++)
                {
                    _streamingResourceHandler.UnloadGo(goes[i], sceneIndex);
                }
                
                _sceneNameToId.Remove(sceneName);
                _loadedGoes.Remove(sceneIndex);
                _goRoot.Remove(sceneIndex);
            }
        }

        public void GoLoaded(int sceneIndex, int goIndex, UnityObject unityObj)
        {
            if (_loadedGoes.ContainsKey(sceneIndex))
            {
                _loadedGoes[sceneIndex].Add(unityObj);
                unityObj.AsGameObject.transform.SetParent(_goRoot[sceneIndex].transform);
            }
        }
    }
}