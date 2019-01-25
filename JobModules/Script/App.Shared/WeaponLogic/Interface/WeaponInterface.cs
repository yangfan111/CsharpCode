namespace Core.WeaponLogic
{
    public interface IWeaponLogicFactory
    {
        IWeaponLogic CreateWeaponLogic(int weaponId, IWeaponSoundLogic soundLogic, IWeaponEffectLogic effectLogic);
    }

    public interface IFireConfig { }
}
