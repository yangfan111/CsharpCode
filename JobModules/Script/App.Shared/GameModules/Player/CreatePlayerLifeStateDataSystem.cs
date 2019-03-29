using App.Shared.Components.Player;
using App.Shared.GameModules.Player.CharacterState;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    public class CreatePlayerLifeStateDataSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(CreatePlayerLifeStateDataSystem));
        public CreatePlayerLifeStateDataSystem()
        {
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var playerEntity = (PlayerEntity) owner.OwnerEntity;
            CreateLifeStateData(playerEntity);
        }
        
        private void CreateLifeStateData(PlayerEntity player)
        {
            if (null == player || !player.hasGamePlay || !player.hasPlayerGameState) return;
            
            var gameState = player.playerGameState;
            gameState.CurrentPlayerLifeState = PlayerLifeStateEnum.NullState;

            var gamePlay = player.gamePlay;
            if (!gamePlay.HasLifeStateChangedFlag()) return;
            
            _logger.InfoFormat("Self PlayerEntity:  {0}  LifeState:  {1}   LastLifeState:  {2}",
                player.entityKey, player.gamePlay.LifeState, player.gamePlay.LastLifeState);

            if (gamePlay.IsLifeState(EPlayerLifeState.Alive) &&
                gamePlay.IsLastLifeState(EPlayerLifeState.Dead))
                gameState.CurrentPlayerLifeState = PlayerLifeStateEnum.Reborn;
            
            if (gamePlay.IsLifeState(EPlayerLifeState.Alive) &&
                gamePlay.IsLastLifeState(EPlayerLifeState.Dying))
                gameState.CurrentPlayerLifeState = PlayerLifeStateEnum.Revive;
            
            if(gamePlay.IsLifeState(EPlayerLifeState.Dying))
                gameState.CurrentPlayerLifeState = PlayerLifeStateEnum.Dying;

            if (gamePlay.IsLifeState(EPlayerLifeState.Dead))
                gameState.CurrentPlayerLifeState = PlayerLifeStateEnum.Dead;
            
            gamePlay.ClearLifeStateChangedFlag();
        }
    }
}