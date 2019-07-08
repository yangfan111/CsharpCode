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
        event Action<UnityObject> GoLoaded;
        event Action<UnityObject> AfterGoLoaded;
        event Action<UnityObject> BeforeGoUnloaded;
        event Action<UnityObject> GoUnloaded;

        void UpdateOrigin(Vector3 pos);

        void GetRequests(List<AssetInfo> sceneRequests, List<AssetInfo> goRequests, List<IEnumerable<AssetInfoEx<MeshRenderer>>> lightmapsRequests);
        int NotFinishedRequests { get; }
        void LoadResource(string visioncenterupdatesystem, IUnityAssetManager assetManager, AssetInfo request);
    }
}