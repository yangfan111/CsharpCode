using Utils.AssetManager;

namespace Core.GameModule.Interface
{
    public interface IResourceLoadSystem
    {
        void OnLoadResources(IUnityAssetManager assetManager);
    }
}