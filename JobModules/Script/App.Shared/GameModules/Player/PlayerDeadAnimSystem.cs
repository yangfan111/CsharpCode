using App.Shared.Components.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    public class PlayerDeadAnimSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerDeadAnimSystem));

        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            var playerEntity = (PlayerEntity)getter.OwnerEntity;
            
            if(null == playerEntity || null == playerEntity.playerGameState ||
               null == playerEntity.characterControllerInterface) 
                return;
            
            var gameState = playerEntity.playerGameState;
            switch (gameState.CurrentPlayerLifeState)
            {
                case PlayerLifeStateEnum.Reborn:
                    Reborn(playerEntity);
                    break;
                case PlayerLifeStateEnum.Revive:
                    Revive(playerEntity);
                    break;
                case PlayerLifeStateEnum.Dying:
                    Dying(playerEntity);
                    break;
                case PlayerLifeStateEnum.Dead:
                    Dead(playerEntity);
                    break;
            }
        }
        
        private void Reborn(PlayerEntity player)
        {
            _logger.InfoFormat("{0} play rebirth", player.entityKey);                   
            player.characterControllerInterface.CharacterController.PlayerReborn();
        }

        private void Dead(PlayerEntity player)
        {
            _logger.InfoFormat("{0} play die", player.entityKey);
                    
            player.WeaponController().UnArmWeapon(false);
            player.characterControllerInterface.CharacterController.PlayerDead(player.isFlagSelf);
            _logger.InfoFormat("PlayerDeadAnimDead");
        }

        private void Dying(PlayerEntity player)
        {
            _logger.InfoFormat("{0} play dying", player.entityKey);
            player.WeaponController().UnArmWeapon(false);
        }

        private void Revive(PlayerEntity player)
        {
            _logger.InfoFormat("{0} play revive", player.entityKey);
        }
    }
}
