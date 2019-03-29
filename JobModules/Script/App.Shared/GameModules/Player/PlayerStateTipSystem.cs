using App.Shared.Components.Player;
using Core.Common;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Player
{
    public class PlayerStateTipSystem: IUserCmdExecuteSystem
    {
        public PlayerStateTipSystem(Contexts context)
        {
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                return;
            }

            ShowPlayerStateTip(player);
        }

        private void ShowPlayerStateTip(PlayerEntity player)
        {
            // 窒息状态
            if (player.hasOxygenEnergyInterface && player.oxygenEnergyInterface.Oxygen.InDivingDeffState)
            {
                player.tip.TipType = ETipType.OutOfOxygen;
            }
            
            // 打断救援
            if (player.hasGamePlay && player.gamePlay.IsInteruptSave)
            {
                player.tip.TipType = ETipType.CanNotRescure;
                player.gamePlay.IsInteruptSave = false;
            }
        }
    }
}