using Utils.AssetManager;

namespace Core.GameModule.Interface
{
    public interface IResourceLoadSystem:IUserSystem
    {
        void OnLoadResources(IUnityAssetManager assetManager);
    }
}