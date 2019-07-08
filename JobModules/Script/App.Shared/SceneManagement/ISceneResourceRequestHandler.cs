using System.Collections.Generic;
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
        void AddLoadLightmapsRequest(IEnumerable<AssetInfoEx<MeshRenderer>> infos);
        void AddUnloadLightmapsRequest(IEnumerable<UnityObject> uObjs);

        event Action<Scene, LoadSceneMode> SceneLoaded;
        event Action<Scene> SceneUnloaded;
        event Action<UnityObject> GoLoaded;
        event Action<UnityObject> AfterGoLoaded;
        event Action<UnityObject> BeforeGoUnloaded;
        event Action<UnityObject> GoUnloaded;
        event Action<MeshRenderer, IEnumerable<UnityObject>> LightmapLoaded;
        event Action<IEnumerable<UnityObject>> LightmapUnloaded;
    }
}