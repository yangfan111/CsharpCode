using Utils.AssetManager;
using System;

namespace Utils.Appearance.Weapon
{
    public interface IWeaponModelLoader
    {
        void RegisterLoadedCb(Action<AssetInfo, object> cb);
        void LoadAsset(AssetInfo asset);
        void UnloadAsset(AssetInfo asset);
    }
}