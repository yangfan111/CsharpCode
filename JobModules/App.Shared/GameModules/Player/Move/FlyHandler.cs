using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Player
{
    public class FlyHandler
    {
        public static void Move(PlayerEntity player, IUserCmd cmd, float deltaTime)
        {
            var moveVertical = cmd.MoveVertical;
            var moveHorizontal = cmd.MoveHorizontal;
            var stage = player.playerSkyMove.MoveStage;
            if (!SharedConfig.IsServer)
            {
                PlayerSkyMoveStateMachine.GetStates()[stage]
                    .Move(player, moveVertical, moveHorizontal, deltaTime);
            }
            else
            {
                PlayerSkyMoveStateMachine.GetStates()[stage]
                    .ServerMove(player, moveVertical, moveHorizontal, deltaTime);
            }
        }
    }
}