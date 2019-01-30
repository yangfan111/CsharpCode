using Utils.AssetManager;

namespace App.Shared.SceneManagement.Streaming
{
    public interface IStreamingResourceHandler
    {
        void LoadScene(AssetInfo addr);
        void UnloadScene(string sceneName);
        bool LoadGo(int sceneIndex, int goIndex);
        void UnloadGo(int sceneIndex, int goIndex);
    }
}