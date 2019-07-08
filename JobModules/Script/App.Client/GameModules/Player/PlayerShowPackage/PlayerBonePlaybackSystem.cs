using Core.Utils;
using Entitas;
using Core.Animation;
using App.Shared.GameModules.Player;
using App.Shared.Components.Player;
using Core.CharacterController;
using Utils.Appearance;
using Utils.Appearance.Bone;
using Utils.Singleton;

namespace App.Client.GameModules.Player.PlayerShowPackage
{
    class PlayerBonePlaybackSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerBonePlaybackSystem));
        private readonly CustomProfileInfo _mainInfo;
        private readonly CustomProfileInfo _subCharacterBoneUpdateInfo;
        private readonly CustomProfileInfo _subCharacterControllerUpdateInfo;
        private readonly CustomProfileInfo _subRebornInfo;
        private readonly CustomProfileInfo _subDeadInfo;

        public PlayerBonePlaybackSystem(Contexts contexts) : base(contexts)
        {
            _mainInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("PlayerBonePlayback");
            _subCharacterBoneUpdateInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CharacterBoneUpdate");
            _subCharacterControllerUpdateInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CharacterControllerUpdate");
            _subRebornInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("PlayerBonePlaybackReborn");
            _subDeadInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("PlayerBonePlaybackDead");
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
                _mainInfo.BeginProfileOnlyEnableProfile();
                
                CheckPlayerLifeState(player);
                if (!player.gamePlay.IsLastLifeState(EPlayerLifeState.Dead))
                    UpdateBones(player);
            }
            finally
            {
                _mainInfo.EndProfileOnlyEnableProfile();
            }
        }

        private void UpdateBones(PlayerEntity player)
        {
            var orientation = player.orientation;
            var networkAnimator = player.networkAnimator;
            var characterBoneInterface = player.characterBoneInterface;
            var thirdPersonAppearance = player.thirdPersonAppearance;
            var characterBone = player.characterBone;
            var characterControllerInterface = player.characterControllerInterface;
            
            characterBoneInterface.CharacterBone.Peek(thirdPersonAppearance.PeekDegree);
            try
            {
                _subCharacterBoneUpdateInfo.BeginProfileOnlyEnableProfile();
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
                    CurrentHandPitch = characterBone.CurrentPitchHandAngle,
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
            }
            finally
            {
                _subCharacterBoneUpdateInfo.EndProfileOnlyEnableProfile();
            }
            
            try
            {
                _subCharacterControllerUpdateInfo.BeginProfileOnlyEnableProfile();
                player.characterContoller.Value.SetCurrentControllerType(ThirdPersonPostureTool.ConvertToPostureInConfig(thirdPersonAppearance.Posture));
                // 更新包围盒
                if (thirdPersonAppearance.NeedUpdateController)
                {
                    characterControllerInterface.CharacterController.SetCharacterControllerHeight(thirdPersonAppearance.CharacterHeight, player.characterContoller.Value.GetCurrentControllerType() == CharacterControllerType.UnityCharacterController, thirdPersonAppearance.CharacterStandHeight);
                    characterControllerInterface.CharacterController.SetCharacterControllerCenter(thirdPersonAppearance.CharacterCenter, player.characterContoller.Value.GetCurrentControllerType() == CharacterControllerType.UnityCharacterController);
                    characterControllerInterface.CharacterController.SetCharacterControllerRadius(thirdPersonAppearance.CharacterRadius, player.characterContoller.Value.GetCurrentControllerType() == CharacterControllerType.UnityCharacterController);
                }
            }
            finally
            {
                _subCharacterControllerUpdateInfo.EndProfileOnlyEnableProfile();
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
            try
            {
                _subRebornInfo.BeginProfileOnlyEnableProfile();
                if (null == player) return;
                var characterController = player.characterControllerInterface.CharacterController;
                if (null != characterController)
                    characterController.PlayerReborn();
            
                Logger.InfoFormat("PlayerBonePlaybackSystem ------  Reborn");
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
                var characterController = player.characterControllerInterface.CharacterController;
                if (null != characterController)
                    characterController.PlayerDead(player.isFlagSelf);

                Logger.InfoFormat("PlayerBonePlaybackSystem ------  Dead");
            }
            finally
            {
                _subDeadInfo.EndProfileOnlyEnableProfile();
            }
        }

        #endregion
    }
}
