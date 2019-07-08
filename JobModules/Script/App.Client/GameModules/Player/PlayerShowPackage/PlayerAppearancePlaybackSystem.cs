using App.Shared.Components.Player;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace App.Client.GameModules.Player.PlayerShowPackage
{
    class PlayerAppearancePlaybackSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerAppearancePlaybackSystem));
        
        private readonly CustomProfileInfo _mainInfo;
        private readonly CustomProfileInfo _subSyncInfo;
        private readonly CustomProfileInfo _subTryRewindInfo;
        private readonly CustomProfileInfo _subRebornInfo;
        private readonly CustomProfileInfo _subDeadInfo;

        public PlayerAppearancePlaybackSystem(Contexts contexts) : base(contexts)
        {
            _mainInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("PlayerAppearancePlayback");
            _subSyncInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("AppearancePlaybackSyncFrom");
            _subTryRewindInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("AppearancePlaybackTryRewind");
            _subRebornInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("AppearancePlaybackReborn");
            _subDeadInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("AppearancePlaybackDead");
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.AppearanceInterface,
                PlayerMatcher.GamePlay,
                PlayerMatcher.ThirdPersonAppearance,
                PlayerMatcher.PredictedAppearance,
                PlayerMatcher.LatestAppearance).NoneOf(PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return !entity.isFlagPlayBackFilter;
        }

        protected override void OnPlayBack(PlayerEntity player)
        {
            if(!player.hasAppearanceInterface) return;
            var appearanceInterface = player.appearanceInterface;

            try
            {
                _mainInfo.BeginProfileOnlyEnableProfile();
                CheckPlayerLifeState(player);
                CheckPlayerNeedUnActive(player);
                appearanceInterface.Appearance.CheckP3HaveInit(!player.gamePlay.HasNewRoleIdChangedFlag());
                
                try
                {
                    _subSyncInfo.BeginProfileOnlyEnableProfile();
                    appearanceInterface.Appearance.SyncLatestFrom(player.latestAppearance);
                    appearanceInterface.Appearance.SyncPredictedFrom(player.predictedAppearance);
                    appearanceInterface.Appearance.SyncClientFrom(player.clientAppearance);
                }
                finally
                {
                    _subSyncInfo.EndProfileOnlyEnableProfile();
                }
                
                try
                {
                    _subTryRewindInfo.BeginProfileOnlyEnableProfile();
                    appearanceInterface.Appearance.TryRewind();
                }
                finally
                {
                    _subTryRewindInfo.EndProfileOnlyEnableProfile();
                }
            }
            finally
            {
                _mainInfo.EndProfileOnlyEnableProfile();
            }
        }

        private void CheckPlayerNeedUnActive(PlayerEntity player)
        {
            var appearanceInterface = player.appearanceInterface;
            if (player.gamePlay.ClientVisibility != player.gamePlay.Visibility)
            {
                appearanceInterface.Appearance.PlayerVisibility(player.gamePlay.Visibility);
                player.gamePlay.ClientVisibility = player.gamePlay.Visibility;
            }
        }
        
        #region LifeState

        private void CheckPlayerLifeState(PlayerEntity player)
        {
            if (null == player || null == player.playerGameState) return;
            var gameState = player.playerGameState;
            switch (gameState.CurrentPlayerLifeState)
            {
                case PlayerLifeStateEnum.Reborn:
                case PlayerLifeStateEnum.Revive:
                    Reborn(player);
                    break;
                case PlayerLifeStateEnum.Dead:
                    Dead(player);
                    break;
            }
        }

        private void Reborn(PlayerEntity player)
        {
            try
            {
                _subRebornInfo.BeginProfileOnlyEnableProfile();
                if (null == player) return;
                var appearance = player.appearanceInterface.Appearance;
                if (null == appearance) return;
                appearance.PlayerReborn();
            
                Logger.InfoFormat("PlayerAppearancePlaybackSystem ------  Reborn");
            }
            finally
            {
                _subRebornInfo.EndProfileOnlyEnableProfile();
            }
        }
        
        private void Dead(PlayerEntity player)
        {
            try
            {
                _subDeadInfo.BeginProfileOnlyEnableProfile();
                if (null == player) return;
                var appearance = player.appearanceInterface.Appearance;
                if (null == appearance) return;
                appearance.PlayerDead();

                Logger.InfoFormat("PlayerAppearancePlaybackSystem ------  Dead");
            }
            finally
            {
                _subDeadInfo.EndProfileOnlyEnableProfile();
            }
        }

        #endregion
    }
}
