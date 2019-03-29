using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    public class ServerPlayerSkyMoveStateUpdateSystem: IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerPlayerSkyMoveStateUpdateSystem));
        private Contexts _contexts;
        public ServerPlayerSkyMoveStateUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;
            
            var playerSkyMove = player.playerSkyMove;
            SynFromInterVar(player);
            playerSkyMove.IsMoveEnabled = PlayerSkyMoveState.IsSkyMoveEnabled(player);

            if (playerSkyMove.IsMoveEnabled)
            {
                //var stage = playerSkyMove.MoveStage;
                var stage = playerSkyMove.MoveStage;
                //Logger.InfoFormat("ServerUpdate move enabled!!!playerSkyMove stage:{0}, game state:{1}, seq:{2}",stage, player.gamePlay.GameState, cmd.Seq);
                PlayerSkyMoveStateMachine.GetStates()[(int)stage].ServerUpdate(_contexts, player, cmd); 
            }
        }
        
        private void SynFromInterVar(PlayerEntity player)
        {
            player.playerSkyMove.SyncFrom(player.playerSkyMoveInterVar);
        }
    }
}