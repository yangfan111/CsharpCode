using Core;
using App.Shared.WeaponLogic;
using Core.GameModeLogic;
using Entitas;
using WeaponConfigNs;

namespace App.Shared.GameModeLogic.ReservedBulletLogic
{
    public class LocalReservedBulletLogic : IReservedBulletLogic
    {
        private Contexts _contexts;
        public LocalReservedBulletLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public int GetReservedBullet(Entity entity, EWeaponSlotType slot)
        {
            var playerEntity = entity as PlayerEntity;
            var weaponComp = playerEntity.GetWeaponData(_contexts, slot);
            return weaponComp.ReservedBullet;
        }

        public int GetReservedBullet(Entity playerEntity, EBulletCaliber caliber)
        {
            return 0;
        }

        public int SetReservedBullet(Entity entity, EWeaponSlotType slot, int count)
        {
            var playerEntity = entity as PlayerEntity;
            var weaponData = playerEntity.GetWeaponData(_contexts, slot);
            weaponData.ReservedBullet = count;
            return count;
        }

        public int SetReservedBullet(Entity playerEntity, EBulletCaliber caliber, int count)
        {
            return count;
        }
    }
}
