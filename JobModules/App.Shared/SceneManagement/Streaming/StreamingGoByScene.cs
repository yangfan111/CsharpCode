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
        private readonly Dictionary<int, StreamingScene> _loadedScenes = new Dictionary<int, StreamingScene>();
        private readonly Dictionary<string, int> _sceneNameToId = new Dictionary<string, int>();

        public void SetResourceHandler(IStreamingResourceHandler handler)
        {
            _streamingResourceHandler = handler;
        }

        public void UpdateOrigin(Vector3 pos, OriginStatus status)
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

            if (!_loadedScenes.ContainsKey(sceneIndex))
            {
                _loadedScenes.Add(sceneIndex, sceneStruct);
                _sceneNameToId.Add(sceneName, sceneIndex);
                
                var count = sceneStruct.Objects.Count;
                for (int i = 0; i < count; i++)
                {
                    _streamingResourceHandler.LoadGo(sceneIndex, i);
                }
            }
        }

        public void SceneUnloaded(string sceneName)
        {
            if (_sceneNameToId.ContainsKey(sceneName))
            {
                var sceneIndex = _sceneNameToId[sceneName];
                var scene = _loadedScenes[sceneIndex];
                var count = scene.Objects.Count;
                for (int i = 0; i < count; i++)
                {
                    _streamingResourceHandler.UnloadGo(sceneIndex, i);
                }
                
                _loadedScenes.Remove(sceneIndex);
                _sceneNameToId.Remove(sceneName);
            }
        }

        public void GoLoaded(int sceneIndex, int goIndex, UnityObjectWrapper<GameObject> obj)
        {
            if (_loadedScenes.ContainsKey(sceneIndex))
            {
                
            }
        }
    }
}