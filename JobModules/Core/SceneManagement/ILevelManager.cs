using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace Core.SceneManagement
{
    public interface ILevelManager : IDisposable
    {
        event Action<Scene, LoadSceneMode> SceneLoaded;
        event Action<Scene> SceneUnloaded;
        event Action<UnityObjectWrapper<GameObject>> GoLoaded;
        event Action<UnityObjectWrapper<GameObject>> GoUnloaded;

        OriginStatus UpdateOrigin(Vector3 pos);
        void GoLoadedWrapper(object nul, UnityObjectWrapper<GameObject> go);

        void GetRequests(List<AssetInfo> sceneRequests, List<AssetInfo> goRequests);
        int NotFinishedRequests { get; }
    }
}