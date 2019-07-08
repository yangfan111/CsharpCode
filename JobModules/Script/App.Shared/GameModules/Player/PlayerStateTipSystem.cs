
using Core;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Player
{
    public class PlayerStateTipSystem: IUserCmdExecuteSystem
    {
        private Contexts _contexts;

        public PlayerStateTipSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;
            if (!player.gamePlay.IsDead())
            {
                if (player.hasOxygenEnergyInterface && player.oxygenEnergyInterface.Oxygen.InDivingDeffState)
                {
                    player.tip.TipType = ETipType.OutOfOxygen;
                }
            }
        }
    }
}