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
            var playerEntity = owner.OwnerEntity as PlayerEntity;
           playerEntity.GetWeaponController().Interrupt();
        }
    }
}
