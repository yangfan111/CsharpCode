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
        void AddUnloadGoRequest(UnityObject go);

        event Action<Scene, LoadSceneMode> SceneLoaded;
        event Action<Scene> SceneUnloaded;
        event Action<UnityObject> GoLoaded;
        event Action<UnityObject> AfterGoLoaded;
        event Action<UnityObject> BeforeGoUnloaded;
        event Action<UnityObject> GoUnloaded;
    }
}