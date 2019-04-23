namespace Utils.Appearance.Weapon
{
    public interface IWeaponModelController
    {
        void SetWeapon(int weaponId);
        void SetPart(int partId);
        void RemovePart(int partId);
        void Clear();
    }
}
