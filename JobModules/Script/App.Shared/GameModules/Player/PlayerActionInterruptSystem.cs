using App.Shared.GameModules.Weapon;
using App.Shared.Util;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Player
{
    public class PlayerActionInterruptSystem : IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            if (!cmd.IsInterrupt)
            {
                return;
            }
            var controller = GameModuleManagement.Get<PlayerWeaponController>(owner.OwnerEntityKey.EntityId);
            controller.Interrupt();
        }
    }
}
