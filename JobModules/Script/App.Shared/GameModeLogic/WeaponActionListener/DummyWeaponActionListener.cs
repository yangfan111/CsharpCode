using Core;
using Core;
using Core.EntityComponent;
using Entitas;

namespace App.Shared.GameModeLogic.WeaponActionListener
{
    public class DummyWeaponActionListener : IWeaponProcessListener
    {
        public void OnExpend(IPlayerWeaponGetter controller, EWeaponSlotType slot)
        {
            //DO NOTHING
        }

    

        public void OnPickup(IPlayerWeaponGetter controller, EWeaponSlotType slot)
        {
            //DO NOTHING
        }

        public void OnDrop(IPlayerWeaponGetter controller, EWeaponSlotType slot, EntityKey dropedWeapon)
        {
        }
    }
}
