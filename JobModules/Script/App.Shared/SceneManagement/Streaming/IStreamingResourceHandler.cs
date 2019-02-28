using UnityEngine;
using Utils.AssetManager;

namespace App.Shared.SceneManagement.Streaming
{
    public interface IStreamingResourceHandler
    {
        void LoadScene(AssetInfo addr);
        void UnloadScene(string sceneName);
        void LoadGo(int sceneIndex, int goIndex);
        void UnloadGo(UnityObject unityObj, int sceneIndex);
    }
}