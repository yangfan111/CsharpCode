using Core;
using Core.Utils;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="CommonModeProcessListener" />
    /// </summary>
    public class CommonModeProcessListener : ModeProcessListener
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonModeProcessListener));

        public override void OnExpend(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
            base.OnExpend(controller, slot);
            if (!slot.IsSlotChangeByCost())
            {
                return;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("{0} OnExpend", controller.Owner);
            }
            controller.BagLockState = true;
        }

        public override void OnDrop(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
            Logger.DebugFormat("{0} OnDrop", controller.Owner);
            base.OnDrop(controller, slot);
            controller.BagLockState = true;
        }

        public override void OnWeaponPickup(IPlayerWeaponProcessor controller, EWeaponSlotType slot)
        {
            Logger.DebugFormat("{0} OnPickup", controller.Owner);
            base.OnWeaponPickup(controller,slot);
            controller.BagLockState = true;
        }

        private void LockPlayerBag(IPlayerWeaponProcessor controller)
        {
            Logger.DebugFormat("{0} LockPlayerBag", controller.Owner);
            controller.BagLockState = true;
        }
    }
}
