using App.Shared.GameModules.Weapon;
using Core;
using WeaponConfigNs;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="LocalReservedBulletHandler" />
    /// </summary>
    public class LocalReservedBulletHandler : IReservedBulletHandler
    {
        public int GetReservedBullet(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
            return controller.HeldWeaponAgent.BaseComponent.ReservedBullet;
        }

        public int GetReservedBullet(IPlayerWeaponProcessor controller, EBulletCaliber caliber)
        {
            return 0;
        }

        public int SetReservedBullet(IPlayerWeaponProcessor controller, EWeaponSlotType slot, int count)
        {
        controller.HeldWeaponAgent.BaseComponent.ReservedBullet = count;
            return count;
            }

      

        public int SetReservedBullet(IPlayerWeaponProcessor controller, EBulletCaliber caliber, int count)
        {
            return count;
        }
    }
}
