using Utils.AssetManager;

namespace Core.GameModule.Interface
{
    public interface IModuleInitSystem:IUserSystem
    {
        void OnInitModule(IUnityAssetManager assetManager);
    }

}