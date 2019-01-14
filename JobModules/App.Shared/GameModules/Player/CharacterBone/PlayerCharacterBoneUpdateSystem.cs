using App.Shared.Components.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.Animation;
using Utils.Appearance;
using Core.Appearance;
using App.Shared.GameModules.Player.CharacterState;
using Core.CharacterState;
using UnityEngine;
using XmlConfig;
using App.Shared.Player;
using Core.CharacterBone;
using App.Shared.GameModules.Player.Appearance;

namespace App.Shared.GameModules.Player.CharacterBone
{
    class PlayerCharacterBoneUpdateSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerCharacterBoneUpdateSystem));
        private readonly FsmOutputBaseSystem _fsmOutputs = new FsmOutputBaseSystem();

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            if (null != player && (player.gamePlay.IsLifeState(EPlayerLifeState.Dead)   || player.gamePlay.IsLastLifeState(EPlayerLifeState.Dead)))
            {
                return;
            }

            SightUpdate(player, cmd.FrameInterval);
            BoneUpdate(player);
        }

        private static void SightUpdate(PlayerEntity player, int deltaTime)
        {
            var state = player.stateInterface.State;
            var sight = player.appearanceInterface.FirstPersonAppearance.SightShift;

            if (state.GetActionKeepState() == ActionKeepInConfig.Sight || state.GetNextActionKeepState() == ActionKeepInConfig.Sight)
            {
                sight.SetProne(state.GetCurrentPostureState() == PostureInConfig.Prone);
                sight.Update(deltaTime);
            }
            else
            {
                sight.Clear();
            }
        }

        private void BoneUpdate(PlayerEntity player)
        {
            var appearanceP1 = player.appearanceInterface.FirstPersonAppearance;
            var characterBone = player.characterBoneInterface.CharacterBone;
            _fsmOutputs.ResetOutput();
            characterBone.Execute(_fsmOutputs.AddOutput);
            _fsmOutputs.SetOutput(player);

            if (!SharedConfig.IsServer)
            {
                FollowRotHelper.Player = player;
                var rotParam = new FollowRotParam
                {
                    CameraFreeYaw = player.cameraStateNew.FreeYaw,
                    CameraFreeNowMode = player.cameraStateNew.FreeNowMode,
                    CameraEulerAngle = player.cameraFinalOutputNew.EulerAngle,
                    ClientTime = player.time.ClientTime
                };
                characterBone.PreUpdate(rotParam, player.characterBoneInterface.CharacterBone);
                characterBone.SyncTo(player.characterBone);
            }

            var animator = player.thirdPersonAnimator.UnityAnimator;
            var state = player.stateInterface.State;
            var postureType = ThirdPersonAppearanceUtils.GetPosture(state);
            var action = player.stateInterface.State.GetActionState();
            var keepAction = player.stateInterface.State.GetActionKeepState();
            var posture = player.stateInterface.State.GetCurrentPostureState();

            CodeRigBoneParam param = new CodeRigBoneParam
            {
                PitchAmplitude = player.orientation.Pitch - player.orientation.WeaponPunchPitch * 2,
                OverlayAnimationWeight = animator.GetLayerWeight(NetworkAnimatorLayer.PlayerUpperBodyOverlayLayer),
                PostureWhenOverlay = postureType,
                IsSight = state.GetActionKeepState() == ActionKeepInConfig.Sight || state.GetNextActionKeepState() == ActionKeepInConfig.Sight,
                SightHorizontalShift = appearanceP1.SightShift.Buff * player.firstPersonAppearance.SightHorizontalShift,
                SightVerticalShift = appearanceP1.SightShift.Buff * player.firstPersonAppearance.SightVerticalShift,
                SightShiftBuff = player.oxygenEnergyInterface.Oxygen.SightShiftBuff,
                IKActive = IKFilter.FilterPlayerIK(action, keepAction, posture),
                HeadPitch = player.characterBone.PitchHeadAngle,
                HeadYaw = player.characterBone.RotHeadAngle,
                HandPitch = player.characterBone.PitchHandAngle,
                HeadRotProcess = player.characterBone.HeadRotProcess,
                IsHeadRotCW = player.characterBone.IsHeadRotCW,
                WeaponPitch = player.characterBone.WeaponPitch,
                IsServer = SharedConfig.IsServer
            };

            characterBone.Update(param);
            if (!SharedConfig.IsServer)
                characterBone.WeaponRotUpdate(param);
        }
    }
}
