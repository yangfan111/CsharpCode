using XmlConfig;

namespace Utils.Appearance.Weapon
{
    public interface IWeaponModelAssemblyController
    {
        void Attach(WeaponPartLocation partType, object partGo, object weaponGo);
        void ShowWeapon(object weaponGo);
    }
}
