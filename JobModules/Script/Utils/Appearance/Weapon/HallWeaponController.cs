using Utils.AssetManager;

namespace Utils.Appearance.Weapon
{
    public class HallWeaponController : WeaponControllerBase
    {
        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHanlder)
        {
            return LoadRequestFactory.Create<object>(assetInfo, loadedHanlder.OnLoadSucc);
        }
    }
}
