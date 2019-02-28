using App.Shared.GameModules.Weapon;
using Core;
using WeaponConfigNs;

namespace App.Shared.GameModeLogic.ReservedBulletLogic
{
    /// <summary>
    /// Defines the <see cref="LocalReservedBulletLogic" />
    /// </summary>
    public class LocalReservedBulletLogic : IReservedBulletLogic
    {
        private Contexts _contexts;

        public LocalReservedBulletLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public int GetReservedBullet(IPlayerWeaponGetter controller, EWeaponSlotType slot)
        {
            return GetReservedBullet((PlayerWeaponController)controller, slot);
        }

        private int GetReservedBullet(PlayerWeaponController controller, EWeaponSlotType slot)
        {
            return controller.HeldWeaponAgent.BaseComponent.ReservedBullet;
        }

        public int GetReservedBullet(IPlayerWeaponGetter controller, EBulletCaliber caliber)
        {
            return 0;
        }

        public int SetReservedBullet(IPlayerWeaponGetter controller, EWeaponSlotType slot, int count)
        {
            return SetReservedBullet((PlayerWeaponController)controller, slot, count);
        }

        private int SetReservedBullet(PlayerWeaponController controller, EWeaponSlotType slot, int count)
        {
            controller.HeldWeaponAgent.BaseComponent.ReservedBullet = count;
            return count;
        }

        public int SetReservedBullet(IPlayerWeaponGetter controller, EBulletCaliber caliber, int count)
        {
            return count;
        }
    }
}
