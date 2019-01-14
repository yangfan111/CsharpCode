using Core.Bag;
using Core.GameModeLogic;
using Entitas;
using WeaponConfigNs;

namespace App.Shared.GameModeLogic.ReservedBulletLogic
{
    public class LocalReservedBulletLogic : IReservedBulletLogic
    {
        public int GetReservedBullet(Entity entity, EWeaponSlotType slot)
        {
            var playerEntity = entity as PlayerEntity;
            var weaponComp = playerEntity.GetWeaponComponentBySlot(slot);
            return weaponComp.ReservedBullet;
        }

        public int GetReservedBullet(Entity playerEntity, EBulletCaliber caliber)
        {
            return 0;
        }

        public int SetReservedBullet(Entity entity, EWeaponSlotType slot, int count)
        {
            var playerEntity = entity as PlayerEntity;
            var weaponComp = playerEntity.GetWeaponComponentBySlot(slot);
            weaponComp.ReservedBullet = count;
            return count;
        }

        public int SetReservedBullet(Entity playerEntity, EBulletCaliber caliber, int count)
        {
            return count;
        }
    }
}
