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
using Core.CharacterController;
using Core.WeaponAnimation;
using UnityEngine;
using Utils.Appearance;
using Utils.Singleton;

namespace App.Client.GameModules.Player.PlayerShowPackage
{
    class PlayerBonePlaybackSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerBonePlaybackSystem));
        private CustomProfileInfo _info;

        public PlayerBonePlaybackSystem(Contexts contexts) : base(contexts)
        {
            _info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("PlayerBonePlayback");
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.AppearanceInterface,
                PlayerMatcher.GamePlay,
                PlayerMatcher.NetworkAnimator,
                PlayerMatcher.ThirdPersonAnimator,
                PlayerMatcher.Orientation,
                PlayerMatcher.ThirdPersonAppearance,
                PlayerMatcher.CharacterControllerInterface).NoneOf(PlayerMatcher.FlagSelf));
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

                if (!player.gamePlay.IsLastLifeState(EPlayerLifeState.Dead))
                    UpdateBones(player);
            }
            finally
            {
                _info.EndProfileOnlyEnableProfile();
            }
        }

        private static void UpdateBones(PlayerEntity player)
        {
            var orientation = player.orientation;
            var networkAnimator = player.networkAnimator;
            var characterBoneInterface = player.characterBoneInterface;
            var thirdPersonAppearance = player.thirdPersonAppearance;
            var characterBone = player.characterBone;
            var characterControllerInterface = player.characterControllerInterface;
            
            characterBoneInterface.CharacterBone.Peek(thirdPersonAppearance.PeekDegree);
            var param = new CodeRigBoneParam
            {
                PitchAmplitude = orientation.Pitch,
                OverlayAnimationWeight = networkAnimator.AnimatorLayers[NetworkAnimatorLayer.PlayerUpperBodyOverlayLayer].Weight,
                PostureWhenOverlay = thirdPersonAppearance.Posture,
                // 预测时，IK不生效
                IKActive = PlayerEntityUtility.ActiveIK(thirdPersonAppearance.Action, 
                    thirdPersonAppearance.Posture, thirdPersonAppearance.NextPosture, thirdPersonAppearance.Movement),
                HeadPitch = characterBone.PitchHeadAngle,
                HeadYaw = characterBone.RotHeadAngle,
                HandPitch = characterBone.PitchHandAngle,
                HeadRotProcess = characterBone.HeadRotProcess,
                WeaponRot = characterBone.WeaponRot,
                IsProne = thirdPersonAppearance.Posture == ThirdPersonPosture.Prone,
                IsHeadRotCW = characterBone.IsHeadRotCW,
                
                FirstPersonPositionOffset = characterBone.FirstPersonPositionOffset,
                FirstPersonRotationOffset = characterBone.FirstPersonRotationOffset,
                FirstPersonSightOffset = characterBone.FirstPersonSightOffset
            };
            // code controlled pose
            characterBoneInterface.CharacterBone.WeaponRotPlayback(param);
            characterBoneInterface.CharacterBone.Update(param);

            player.characterContoller.Value.SetCurrentControllerType(ThirdPersonPostureTool.ConvertToPostureInConfig(thirdPersonAppearance.Posture));
            
            // 更新包围盒

            if (thirdPersonAppearance.NeedUpdateController)
            {
                characterControllerInterface.CharacterController.SetCharacterControllerHeight(thirdPersonAppearance.CharacterHeight, player.characterContoller.Value.GetCurrentControllerType() == CharacterControllerType.UnityCharacterController, thirdPersonAppearance.CharacterStandHeight);
                characterControllerInterface.CharacterController.SetCharacterControllerCenter(thirdPersonAppearance.CharacterCenter, player.characterContoller.Value.GetCurrentControllerType() == CharacterControllerType.UnityCharacterController);
                characterControllerInterface.CharacterController.SetCharacterControllerRadius(thirdPersonAppearance.CharacterRadius, player.characterContoller.Value.GetCurrentControllerType() == CharacterControllerType.UnityCharacterController);
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
            var characterController = player.characterControllerInterface.CharacterController;
            if (null != characterController)
                characterController.PlayerReborn();
            
            var go = player.RootGo();
            if(null != go)
                go.BroadcastMessage("PlayerRelive");
            
            _logger.InfoFormat("PlayerBonePlaybackSystem ------  Reborn");
        }
        
        private void Dead(PlayerEntity player)
        {
            if (null == player) return;
            var characterController = player.characterControllerInterface.CharacterController;
            if (null != characterController)
                characterController.PlayerDead(player.isFlagSelf);

            var go = player.RootGo();
            if(null != go)
                go.BroadcastMessage("PlayerDead");
            
            _logger.InfoFormat("PlayerBonePlaybackSystem ------  Dead");
        }

        #endregion
    }
}