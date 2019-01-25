using WeaponConfigNs;

namespace App.Shared.GameModules.Bullet
{
    public interface IDamageInfoCollector
    {
        void SetPlayerDamageInfo(PlayerEntity source, PlayerEntity target, float damage, EBodyPart part);
    }
}