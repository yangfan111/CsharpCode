using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player.CharacterState;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using Core.Animation;
using App.Shared.GameModules.Player;
using App.Shared.Components.Player;
using App.Shared.Player;
using Core.WeaponAnimation;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.Player.PlayerShowPackage
{
    class PlayerAppearancePlaybackSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAppearancePlaybackSystem));
        private CustomProfileInfo _info;

        public PlayerAppearancePlaybackSystem(Contexts contexts) : base(contexts)
        {
            _info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("PlayerAppearancePlayback");
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
                _info.BeginProfileOnlyEnableProfile();
                
                CheckPlayerLifeState(player);
                
                appearanceInterface.Appearance.SyncLatestFrom(player.latestAppearance);
                appearanceInterface.Appearance.SyncPredictedFrom(player.predictedAppearance);
                appearanceInterface.Appearance.TryRewind();
            }
            finally
            {
                _info.EndProfileOnlyEnableProfile();
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
                    Reborn(player);
                    break;
                case PlayerLifeStateEnum.Dead:
                    Dead(player);
                    break;
            }
        }

        private void Reborn(PlayerEntity player)
        {
            if (null == player) return;
            var appearance = player.appearanceInterface.Appearance;
            if (null == appearance) return;
            appearance.PlayerReborn();
            
            _logger.InfoFormat("PlayerAppearancePlaybackSystem ------  Reborn");
        }
        
        private void Dead(PlayerEntity player)
        {
            if (null == player) return;
            var appearance = player.appearanceInterface.Appearance;
            if (null == appearance) return;
            appearance.PlayerDead();
            
            _logger.InfoFormat("PlayerAppearancePlaybackSystem ------  Dead");
        }

        #endregion
    }
}