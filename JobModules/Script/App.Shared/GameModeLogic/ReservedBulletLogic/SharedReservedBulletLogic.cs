using Assets.Utils.Configuration;
using Core;
using Core.GameModeLogic;
using Core.Utils;
using Entitas;
using WeaponConfigNs;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModeLogic.ReservedBulletLogic
{
    public class SharedReservedBulletLogic : IReservedBulletLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SharedReservedBulletLogic));
        private IWeaponConfigManager _weaponConfigManager;
        private const int BulletLimit = int.MaxValue;
        private Contexts _contexts;
        public SharedReservedBulletLogic(Contexts contexts, IWeaponConfigManager weaponConfigManager)
        {
            _contexts = contexts;
            _weaponConfigManager = weaponConfigManager;
        }

        public int SetReservedBullet(Entity playerEntity, EBulletCaliber caliber, int count)
        {
            var player = playerEntity as PlayerEntity;
            switch (caliber)
            {
                case EBulletCaliber.E12No:
                    return DoSetReserveBullet(ref player.weaponState.ReservedBullet12, count);
                case EBulletCaliber.E300Mag:
                    return DoSetReserveBullet(ref player.weaponState.ReservedBullet300, count);
                case EBulletCaliber.E45apc:
                    return DoSetReserveBullet(ref player.weaponState.ReservedBullet45, count);
                case EBulletCaliber.E50AE:
                    return DoSetReserveBullet(ref player.weaponState.ReservedBullet50, count);
                case EBulletCaliber.E556mm:
                    return DoSetReserveBullet(ref player.weaponState.ReservedBullet556, count);
                case EBulletCaliber.E762mm:
                    return DoSetReserveBullet(ref player.weaponState.ReservedBullet762, count);
                case EBulletCaliber.E9mm:
                    return DoSetReserveBullet(ref player.weaponState.ReservedBullet9, count);
                case EBulletCaliber.E57mm:
                    return DoSetReserveBullet(ref player.weaponState.ReservedBullet57, count);
                default:
                    Logger.ErrorFormat("Illegal caliber {0}", caliber);
                    return 0;
            }
        }

        public int SetReservedBullet(Entity playerEntity, EWeaponSlotType slot, int count)
        {
            var player = playerEntity as PlayerEntity;
            var caliber = GetCaliber(playerEntity, slot);
            return SetReservedBullet(playerEntity, caliber, count);
        }

        private int DoSetReserveBullet( ref int bagReservedBullet,  int count)
        {
            if(count > BulletLimit)
            {
                bagReservedBullet = BulletLimit;
                return count - BulletLimit;
            }
            else
            {
                bagReservedBullet = count;
                return 0;
            }
        }

        public int GetReservedBullet(Entity entity, EBulletCaliber caliber)
        {
            var playerEntity = entity as PlayerEntity;
            switch (caliber)
            {
                case EBulletCaliber.E12No:
                    return playerEntity.weaponState.ReservedBullet12;
                case EBulletCaliber.E300Mag:
                    return playerEntity.weaponState.ReservedBullet300;
                case EBulletCaliber.E45apc:
                    return playerEntity.weaponState.ReservedBullet45;
                case EBulletCaliber.E50AE:
                    return playerEntity.weaponState.ReservedBullet50;
                case EBulletCaliber.E556mm:
                    return playerEntity.weaponState.ReservedBullet556;
                case EBulletCaliber.E762mm:
                    return playerEntity.weaponState.ReservedBullet762;
                case EBulletCaliber.E9mm:
                    return playerEntity.weaponState.ReservedBullet9;
                case EBulletCaliber.E57mm:
                    return playerEntity.weaponState.ReservedBullet57;
                default:
                    Logger.ErrorFormat("Illegal caliber {0}", caliber);
                    return 0;
            }
        }

        public int GetReservedBullet(Entity entity, EWeaponSlotType slot)
        {
            var playerEntity = entity as PlayerEntity;
            var caliber = GetCaliber(playerEntity, slot);
            return GetReservedBullet(playerEntity, caliber);
        }

        private EBulletCaliber GetCaliber(Entity entity, EWeaponSlotType slot)
        {
            var playerEntity = entity as PlayerEntity;
            var weapon = playerEntity.WeaponController().GetSlotWeaponInfo(_contexts, slot);
            var weaponId = weapon.ConfigId;
            if (weaponId > 0)
            {
                var cfg = _weaponConfigManager.GetConfigById(weaponId);
                return (EBulletCaliber)cfg.Caliber;
            }
            Logger.ErrorFormat("no weapon in slot {0} !!", slot);
            return EBulletCaliber.Length;
        }
    }
}
