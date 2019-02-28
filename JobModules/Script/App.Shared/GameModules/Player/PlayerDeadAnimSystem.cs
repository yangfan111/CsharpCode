using App.Client.GameModules.Player;
using App.Shared.Components.Player;
using App.Shared.GameModules.Weapon;
using App.Shared.Player;
using Core.Utils;
using Entitas;

namespace App.Shared.GameModules.Player
{
    public class PlayerDeadAnimSystem : AbstractGamePlaySystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerDeadAnimSystem));
        private Contexts _contexts;

        public PlayerDeadAnimSystem(Contexts context) : base(context)
        {
            _contexts = context;
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.StateInterface,
                PlayerMatcher.AppearanceInterface, PlayerMatcher.GamePlay));//PlayerMatcher.PlayerAction));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        protected override void OnGamePlay(PlayerEntity playerEntity)
        {
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
                    
            player.WeaponController().ForceUnArmHeldWeapon();
            player.characterControllerInterface.CharacterController.PlayerDead();
            _logger.InfoFormat("PlayerDeadAnimDead");
        }

        private void Dying(PlayerEntity player)
        {
            _logger.InfoFormat("{0} play dying", player.entityKey);
            player.WeaponController().ForceUnArmHeldWeapon();
        }

        private void Revive(PlayerEntity player)
        {
            _logger.InfoFormat("{0} play revive", player.entityKey);
        }
    }
}