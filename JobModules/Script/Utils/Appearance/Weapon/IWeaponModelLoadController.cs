using Utils.AssetManager;
using XmlConfig;

namespace Utils.Appearance.Weapon
{
    public interface IWeaponModelLoadController<T> where T : class
    {
        void OnLoaded(AssetInfo info, object go);
        void LoadWeapon(AssetInfo asset);
        void LoadPart(AssetInfo asset, WeaponPartLocation location);
        void Unload(AssetInfo asset);
        void Clear();
    }
}
