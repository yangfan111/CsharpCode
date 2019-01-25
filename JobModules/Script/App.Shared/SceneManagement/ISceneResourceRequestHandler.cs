using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace App.Shared.SceneManagement
{
    interface ISceneResourceRequestHandler
    {
        void AddLoadSceneRequest(AssetInfo addr);
        void AddLoadGoRequest(AssetInfo addr);
        void AddUnloadSceneRequest(string sceneName);
        void AddUnloadGoRequest(UnityObjectWrapper<GameObject> go);

        event Action<Scene, LoadSceneMode> SceneLoaded;
        event Action<Scene> SceneUnloaded;
        event Action<UnityObjectWrapper<GameObject>> GoLoaded;
        event Action<UnityObjectWrapper<GameObject>> GoUnloaded;
    }
}