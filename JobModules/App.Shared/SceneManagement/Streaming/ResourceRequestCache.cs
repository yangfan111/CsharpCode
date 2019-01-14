using System;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace App.Shared.SceneManagement.Streaming
{
    public interface ResourceRequestCache
    {
        void AddLoadSceneRequest(AssetInfo addr);
        void AddLoadGoRequest(AssetInfo addr);
        void AddUnloadSceneRequest(Scene scene);
        event Action<Scene, LoadSceneMode> SceneLoaded;
        event Action<Scene> SceneUnloaded;
    }
}