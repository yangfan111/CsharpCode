using Core.SceneManagement;
using Shared.Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace App.Shared.SceneManagement.Streaming
{
    public interface IStreamingGoManager
    {
        void SetResourceHandler(IStreamingResourceHandler handler);
        void UpdateOrigin(Vector3 pos, OriginStatus status);

        void SetAsapMode(bool value);
        
        void SceneLoaded(string sceneName, int sceneIndex, Scene scene, StreamingScene sceneStruct, Vector4 sceneDimension);
        void SceneUnloaded(string sceneName);

        void GoLoaded(int sceneIndex, int goIndex, UnityObject obj);
    }
}