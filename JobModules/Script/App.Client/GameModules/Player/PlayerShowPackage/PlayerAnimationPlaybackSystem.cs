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
using Core.CharacterState;
using Core.WeaponAnimation;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.Player.PlayerShowPackage
{
    class PlayerAnimationPlaybackSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAnimationPlaybackSystem));
        private CustomProfileInfo _info;
        private Contexts _context;

        public PlayerAnimationPlaybackSystem(Contexts contexts) : base(contexts)
        {
            _context = contexts;
            _info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("PlayerAnimationPlayback");
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.AppearanceInterface,
                PlayerMatcher.GamePlay,
                PlayerMatcher.NetworkAnimator,
                PlayerMatcher.ThirdPersonAnimator).NoneOf(PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return !entity.isFlagPlayBackFilter;
        }

        protected override void OnPlayBack(PlayerEntity player)
        {
            try
            {
                _info.BeginProfileOnlyEnableProfile();

                CheckPlayerLifeState(player);

                var networkAnimator = player.networkAnimator;
                if (null == networkAnimator) return;
                if (!player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
                {
                    OverridePlayBackNetworkAnimator(player, networkAnimator.AnimatorLayers);

//                    流量优化用
//                    if (networkAnimator.AnimatorParameters == null)
//                    {
//                        networkAnimator.SetAnimatorParamsWithoutChangeData(
//                            NetworkAnimatorUtil.GetAnimatorParams(player.thirdPersonAnimator.UnityAnimator));
//                        networkAnimator.ConvertCompressDataToStructureData();
//                    }
                    AnimatorPoseReplayer.ReplayPose(networkAnimator.AnimatorLayers, networkAnimator.AnimatorParameters,
                        player.thirdPersonAnimator.UnityAnimator);
                }
            }
            finally
            {
                _info.EndProfileOnlyEnableProfile();
            }
        }

        /// <summary>
        /// 直接修改NetworkAnimator的动画数据
        /// </summary>
        private void OverridePlayBackNetworkAnimator(PlayerEntity player,
            List<NetworkAnimatorLayer> networkAnimatorAnimatorLayers)
        {
            var currentTime = _context.session.currentTimeObject.CurrentTime;
            //覆盖受伤动画
            if (player.hasOverrideNetworkAnimator && player.overrideNetworkAnimator.IsInjuryAnimatorActive)
            {
                var normalizeTime = (currentTime - player.overrideNetworkAnimator.InjuryTriigerTime) * 0.001f /
                                    AnimatorParametersHash.InjureyStateDuration;
                _logger.DebugFormat("player:{0}, normalizeTime:{1}, trigger time:{2} ,current time:{3}",
                    player.entityKey.Value, normalizeTime, player.overrideNetworkAnimator.InjuryTriigerTime,
                    _context.session.currentTimeObject.CurrentTime);
                if (normalizeTime < 0)
                {
                    _logger.DebugFormat("trigger time:{0} is lager than currentTime:{1}",
                        player.overrideNetworkAnimator.InjuryTriigerTime, currentTime);
                }
                else if (normalizeTime < 1)
                {
                    NetworkAnimatorUtil.ForceToInjureState(networkAnimatorAnimatorLayers, normalizeTime);
                }
                else
                {
                    player.overrideNetworkAnimator.IsInjuryAnimatorActive = false;
                    player.overrideNetworkAnimator.InjuryTriigerTime = -1;
                }
            }
        }

        protected override void BeforeOnPlayback()
        {
            base.BeforeOnPlayback();

            Animator.ClearAnimatorJobContainer();
        }

        protected override void AfterOnPlayback()
        {
            base.AfterOnPlayback();

            Animator.BatchUpdate();
            Animator.ClearAnimatorJobContainer();
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
            _logger.InfoFormat("PlayerEntity: {0}  PlayerAnimationPlaybackSystem ------  Reborn", player.entityKey);
        }

        private void Dead(PlayerEntity player)
        {
            _logger.InfoFormat("PlayerEntity: {0}  PlayerAnimationPlaybackSystem ------  Dead", player.entityKey);
        }

        #endregion
    }
}