using App.Shared.GameModules.Bullet;
using Assets.XmlConfig;
using Core.Enums;
using Core.Statistics;

namespace App.Shared.Util
{
    public static class StatisticsDataEx
    {
        public static void SetTarDataByDamageInfo(this StatisticsData data, PlayerDamageInfo damage)
        {
            switch ((EUIDeadType)damage.type)
            {
                case EUIDeadType.Bombing:
                    data.KillByAirBomb = true;
                    break;
                case EUIDeadType.Drown:
                    data.Drown = true;
                    break;
                case EUIDeadType.Fall:
                    data.DropDead = true;
                    break;
                case EUIDeadType.Poison:
                    data.PoisionDead = true;
                    break;
                case EUIDeadType.VehicleHit:
                    data.KillByVehicle = true;
                    break;
                case EUIDeadType.Weapon:
                    data.KillByPlayer = true;
                    break;
                case EUIDeadType.Unarmed:
                    data.KillByPlayer = true;
                    break;
            }
        }

        public static void SetSrcDataByDamageInfo(this StatisticsData data, PlayerDamageInfo damage)
        {
            switch(damage.WeaponType)
            {
                case EWeaponSubType.BurnBomb:
                case EWeaponSubType.Grenade:
                case EWeaponSubType.FlashBomb:
                case EWeaponSubType.FogBomb:
                case EWeaponSubType.Throw:
                    data.KillWithThrowWeapon += 1;
                    break;
                case EWeaponSubType.Hand:
                case EWeaponSubType.Melee:
                    data.KillWithMelee += 1;
                    break;
                case EWeaponSubType.MachineGun:
                    data.KillWithMachineGun += 1;
                    break;
                case EWeaponSubType.Pistol:
                    data.KillWithPistol += 1;
                    break;
                case EWeaponSubType.ShotGun:
                    data.KillWithShotGun += 1;
                    break;
                case EWeaponSubType.Sniper:
                    data.KillWithSniper += 1;
                    break;
                case EWeaponSubType.SubMachineGun:
                    data.KillWithSubmachineGun += 1;
                    break;
                case EWeaponSubType.Rifle:
                    data.KillWithRifle += 1;
                    break;
            }
        } 
    }
}
