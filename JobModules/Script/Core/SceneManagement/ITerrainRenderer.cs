using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace Core.SceneManagement
{
    public interface ITerrainRenderer
    {
        void SceneLoaded(Scene scene, LoadSceneMode mode);
        void SceneUnloaded(Scene scene);
        void GoLoaded(UnityObject obj);
        void GoUnloaded(UnityObject obj);

        void SetCamera(Camera cam);
        void Draw();
    }
}