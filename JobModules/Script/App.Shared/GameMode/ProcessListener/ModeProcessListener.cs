using App.Shared.GameModules.Weapon;
using Core;
using Core.EntityComponent;
using Core.Fsm;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="ModeProcessListener" />
    /// </summary>
    public class ModeProcessListener : IModeProcessListener
    {
        public virtual void OnExpend(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
           
        }

     
        public virtual void OnPickup(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
        }

        public virtual void OnDrop(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
        }

      
        public void OnSwitch(IPlayerWeaponProcessor controller, int weaponId, EInOrOff op)
        {
            if(controller.AudioController != null)
                controller.AudioController.PlaySwitchAuido(weaponId, op);
        }
    }
}
