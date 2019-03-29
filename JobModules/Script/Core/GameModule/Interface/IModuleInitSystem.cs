using Utils.AssetManager;

namespace Core.GameModule.Interface
{
    public interface IModuleInitSystem
    {
        void OnInitModule(IUnityAssetManager assetManager);
    }

}