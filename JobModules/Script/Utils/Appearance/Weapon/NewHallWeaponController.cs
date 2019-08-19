using Utils.Appearance.Weapon.WeaponShowPackage;
using Utils.AssetManager;

namespace Utils.Appearance.Weapon
{
    public class NewHallWeaponController : NewWeaponControllerBase
    {
        protected override AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHanlder)
        {
            return LoadRequestFactory.Create<object>(assetInfo, loadedHanlder.OnLoadSuccess);
        }
    }
}
