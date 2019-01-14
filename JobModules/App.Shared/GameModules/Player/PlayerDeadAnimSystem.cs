using App.Client.GameModules.Player;
using App.Shared.Components.Player;
using App.Shared.Player;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Bag;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using Utils.Appearance;

namespace App.Shared.GameModules.Player
{
    public class PlayerDeadAnimSystem : AbstractGamePlaySystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerDeadAnimSystem));
        private IGroup<PlayerEntity> _players;

        public PlayerDeadAnimSystem(Contexts context) : base(context)
        {
        }


        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.StateInterface,
                PlayerMatcher.AppearanceInterface, PlayerMatcher.GamePlay,PlayerMatcher.PlayerAction));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        protected override void OnGamePlay(PlayerEntity playerEntity)
        {
            var gamePlay = playerEntity.gamePlay;
            var playerState = playerEntity.stateInterface.State;
            var playerAppearance = playerEntity.appearanceInterface.Appearance;
            var characterControllerAppearance = playerEntity.characterControllerInterface.CharacterController;

            if (gamePlay.HasLifeStateChangedFlag())
            {
                if (gamePlay.IsLifeState(EPlayerLifeState.Dead))
                {
                    _logger.InfoFormat("{0} play die", playerEntity.entityKey);
                    playerEntity.appearanceInterface.Appearance.SetThridPerson();
                    playerEntity.appearanceInterface.Appearance.UnmountWeaponFromHandAtOnce();
                    playerEntity.characterBoneInterface.CharacterBone.SetThridPerson();
                    playerEntity.playerAction.Logic.ForceUnmountWeapon();
                    playerAppearance.PlayerDead();
                    characterControllerAppearance.PlayerDead();
                    playerEntity.genericActionInterface.GenericAction.PlayerDead(playerEntity);
                    //playerEntity.RootGo().SendMessage("PlayerDead");
                    playerEntity.RootGo().BroadcastMessage("PlayerDead");
                    //playerState.Die();
                }
                else if (gamePlay.IsLifeState(EPlayerLifeState.Alive))
                {
                    if (gamePlay.IsLastLifeState(EPlayerLifeState.Dying))
                    {
                        _logger.InfoFormat("{0} play revive", playerEntity.entityKey);
                        playerState.Revive();
                    }
                    else if (gamePlay.IsLastLifeState(EPlayerLifeState.Dead))
                    {
                        _logger.InfoFormat("{0} play rebirth", playerEntity.entityKey);
                        playerState.PlayerReborn();
                        playerEntity.genericActionInterface.GenericAction.PlayerReborn(playerEntity);
                        playerAppearance.PlayerReborn();
                        characterControllerAppearance.PlayerReborn();
                        playerEntity.RootGo().BroadcastMessage("PlayerRelive");
                    }
                }
                else if (gamePlay.IsLifeState(EPlayerLifeState.Dying))
                {
                    _logger.InfoFormat("{0} play dying", playerEntity.entityKey);
                    playerState.Dying();
                    playerEntity.playerAction.Logic.ForceUnmountWeapon();
                }
            }

            gamePlay.ClearLifeStateChangedFlag();
        }
    }
}