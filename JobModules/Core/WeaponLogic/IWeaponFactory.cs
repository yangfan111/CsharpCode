using Core.CharacterState;

namespace Core.WeaponLogic
{
    public interface IWeaponFactory
    {
        void Prepare(int weaponId);
        IWeaponLogic GetWeaponLogic();
        IWeaponSoundLogic GetWeaponSoundLogic();
        IWeaponEffectLogic GetWeaponEffectLogic();
        IPlayerWeaponState GetPlayerWeaponState();
        void ClearCache();
    }
}