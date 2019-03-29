using App.Shared.Components.Player;
using App.Shared.GameModules.Vehicle;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    public class PlayerBuffUpdateSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerBuffUpdateSystem));

        private static float HPLossRate = 0.01f;
        private Contexts _contexts;

        public PlayerBuffUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity) owner.OwnerEntity;
            //受伤状态自动掉血（非被救援状态）
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dying) && player.gamePlay.InHurtedHp > 0
                && !player.gamePlay.IsBeSave)
            {
                player.gamePlay.BuffRemainTime += cmd.FrameInterval;
                int secs = player.gamePlay.BuffRemainTime / 1000;

                if (secs > 0)
                {
                    player.gamePlay.BuffRemainTime %= 1000;
                    float damage = player.gamePlay.InHurtedCount * player.gamePlay.MaxHp * HPLossRate * secs;
                    VehicleDamageUtility.DoPlayerDamage(_contexts, null, player, damage, EUIDeadType.NoHelp);
                }
            }
        }
    }
}
