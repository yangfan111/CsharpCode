using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.Player
{
    public class PlayerWeaponSwitchSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponSwitchSystem));
        /// <summary>
        /// 槽位切换&模式切换
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="cmd"></param>
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {    
            if (cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsSwitchWeapon) && cmd.CurWeapon != (int) EWeaponSlotType.None)
            {
                var newSlot = owner.OwnerEntityKey.ModeController().GetSlotByIndex(cmd.CurWeapon);
                owner.OwnerEntityKey.WeaponController().SwitchIn(newSlot);
                return;
            }
            if (cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsSwitchFireMode))
            {
                owner.OwnerEntityKey.WeaponController().SwitchFireMode();
                return;
            }
        }
    }
}