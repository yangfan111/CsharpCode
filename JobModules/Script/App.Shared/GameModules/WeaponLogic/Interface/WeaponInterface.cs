namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="IWeaponLogicFactory" />
    /// </summary>
    public interface IWeaponLogicFactory
    {
        IWeaponLogic CreateWeaponLogic(int weaponId, IWeaponSoundLogic soundLogic, IWeaponEffectLogic effectLogic);
    }

    /// <summary>
    /// Defines the <see cref="IFireConfig" />
    /// </summary>
    public interface IFireConfig
    {
    }
}
