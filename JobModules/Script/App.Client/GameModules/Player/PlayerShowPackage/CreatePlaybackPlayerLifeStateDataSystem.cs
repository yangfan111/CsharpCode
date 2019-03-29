using App.Shared.Components.Player;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace App.Client.GameModules.Player.PlayerShowPackage
{
    public class CreatePlaybackPlayerLifeStateDataSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(CreatePlaybackPlayerLifeStateDataSystem));
        private CustomProfileInfo _info;
        
        public CreatePlaybackPlayerLifeStateDataSystem(Contexts contexts) : base(contexts)
        {
            _info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CreatePlaybackPlayerLifeStateDataSystem");
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.GamePlay,
                PlayerMatcher.PlayerGameState).NoneOf(PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        protected override void OnPlayBack(PlayerEntity player)
        {
            try
            {
                _info.BeginProfileOnlyEnableProfile();
                CreateLifeStateData(player);
            }
            finally
            {
                _info.EndProfileOnlyEnableProfile();
            }
        }

        private void CreateLifeStateData(PlayerEntity player)
        {
            if (null == player || !player.hasGamePlay || !player.hasPlayerGameState) return;
            
            var gameState = player.playerGameState;
            gameState.CurrentPlayerLifeState = PlayerLifeStateEnum.NullState;

            var gamePlay = player.gamePlay;
            if (!gamePlay.HasLifeStateChangedFlag()) return;
            
            _logger.InfoFormat("Playback PlayerEntity:  {0}  LifeState:  {1}   LastLifeState:  {2}",
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