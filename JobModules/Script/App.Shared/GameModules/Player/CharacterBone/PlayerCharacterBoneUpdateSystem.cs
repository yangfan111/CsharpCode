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
using Core.Compare;
using Shared.Scripts;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.CharacterBone
{
    class PlayerCharacterBoneUpdateSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerCharacterBoneUpdateSystem));
        private readonly FsmOutputBaseSystem _fsmOutputs = new FsmOutputBaseSystem();

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            CheckPlayerLifeState(player);
            
            if(null != player && player.gamePlay.IsLifeState(EPlayerLifeState.Dead)) return;

            SightUpdate(player, cmd.FrameInterval);
            SyncSightComponent(player);
            BoneUpdate(player);
        }

        private void SyncSightComponent(PlayerEntity player)
        {
            var toComponent = player.firstPersonAppearanceUpdate;
            var fromComponent = player.firstPersonAppearance;
            toComponent.SightVerticalShift = fromComponent.SightVerticalShift;
            toComponent.SightHorizontalShift = fromComponent.SightHorizontalShift;
            toComponent.SightVerticalShiftRange = fromComponent.SightVerticalShiftRange;
            toComponent.SightHorizontalShiftDirection = fromComponent.SightHorizontalShiftDirection;
            toComponent.SightVerticalShiftDirection = fromComponent.SightVerticalShiftDirection;
            toComponent.SightRemainVerticalPeriodTime = fromComponent.SightRemainVerticalPeriodTime;
            toComponent.RandomSeed = fromComponent.RandomSeed;
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
            var action = player.stateInterface.State.GetActionState();
            var keepAction = player.stateInterface.State.GetActionKeepState();
            var posture = player.stateInterface.State.GetCurrentPostureState();
            var nextPosture = player.stateInterface.State.GetNextPostureState();
            var movement = player.stateInterface.State.GetCurrentMovementState();
            var postureType = ThirdPersonAppearanceUtils.GetPosture(posture);

            UpdateOffsetData(player);

            var param = new CodeRigBoneParam
            {
                PitchAmplitude = player.orientation.Pitch - player.orientation.WeaponPunchPitch * 2,
                OverlayAnimationWeight = animator.GetLayerWeight(NetworkAnimatorLayer.PlayerUpperBodyOverlayLayer),
                PostureWhenOverlay = postureType,
                IsSight = state.GetActionKeepState() == ActionKeepInConfig.Sight || state.GetNextActionKeepState() == ActionKeepInConfig.Sight,
                SightHorizontalShift = appearanceP1.SightShift.Buff * player.firstPersonAppearance.SightHorizontalShift,
                SightVerticalShift = appearanceP1.SightShift.Buff * player.firstPersonAppearance.SightVerticalShift,
                SightShiftBuff = player.oxygenEnergyInterface.Oxygen.SightShiftBuff,
                IKActive = IKFilter.FilterPlayerIK(action, keepAction, posture, nextPosture, movement),
                HeadPitch = player.characterBone.PitchHeadAngle,
                HeadYaw = player.characterBone.RotHeadAngle,
                HandPitch = player.characterBone.PitchHandAngle,
                HeadRotProcess = player.characterBone.HeadRotProcess,
                IsHeadRotCW = player.characterBone.IsHeadRotCW,
                WeaponRot = player.characterBone.WeaponRot,
                IsProne = posture == PostureInConfig.Prone,
                IsServer = SharedConfig.IsServer,
                
                FirstPersonPositionOffset = player.characterBone.FirstPersonPositionOffset,
                FirstPersonRotationOffset = player.characterBone.FirstPersonRotationOffset,
                FirstPersonSightOffset = player.characterBone.FirstPersonSightOffset
            };
            
            if (!SharedConfig.IsServer)
                characterBone.WeaponRotUpdate(param);
            else
                characterBone.WeaponRotPlayback(param);
            characterBone.Update(param);
        }

        private static void UpdateOffsetData(PlayerEntity player)
        {
            if (SharedConfig.IsServer || player.appearanceInterface.Appearance.IsEmptyHand()) return;
            if(!GetNeedChangeOffset(player)) return;

            var realWeaponIdInHand = player.characterBone.RealWeaponId;
            var screenRatio = player.characterBone.ScreenRatio;
            
            player.characterBone.FirstPersonPositionOffset = SingletonManager.Get<FirstPersonOffsetConfigManager>()
                .GetFirstPersonOffsetByScreenRatio(realWeaponIdInHand,
                    screenRatio);
            // 一人称腰射旋转偏移
            player.characterBone.FirstPersonRotationOffset = SingletonManager.Get<FirstPersonOffsetConfigManager>()
                .GetFirstPersonRotationOffsetByScreenRatio(realWeaponIdInHand,
                    screenRatio);
            // 一人称人物肩射位置偏移
            player.characterBone.FirstPersonSightOffset = SingletonManager.Get<FirstPersonOffsetConfigManager>()
                .GetSightOffsetByScreenRatio(realWeaponIdInHand,
                    screenRatio);
        }

        private static bool GetNeedChangeOffset(PlayerEntity player)
        {
            var screenRatio = Screen.width / (float) Screen.height;
            var realWeaponId = GetRealWeaponId(player);
            var needChanged = !CompareUtility.IsApproximatelyEqual(screenRatio, player.characterBone.ScreenRatio) ||
                              !CompareUtility.IsApproximatelyEqual(realWeaponId, player.characterBone.RealWeaponId) ||
                              FirstPersonOffsetScript.UpdateOffset;
            
            player.characterBone.ScreenRatio = screenRatio;
            player.characterBone.RealWeaponId = realWeaponId;
            player.characterBone.NeedChangeOffset = needChanged;

            return needChanged;
        }
        
        private static int GetRealWeaponId(PlayerEntity player)
        {
            var weaponIdInHand =  player.appearanceInterface.Appearance.GetWeaponIdInHand();
            var realWeaponIdInHand = weaponIdInHand;
            var avatarConfig = SingletonManager.Get<WeaponAvatarConfigManager>().GetConfigById(weaponIdInHand);
            if (null != avatarConfig)
            {
                //当前武器为皮肤武器
                if (avatarConfig.ApplyWeaponsId > 0 && avatarConfig.ApplyWeaponsId != weaponIdInHand)
                {
                    realWeaponIdInHand = avatarConfig.ApplyWeaponsId;
                }
            }

            return realWeaponIdInHand;
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
            
            var characterBone = player.characterBoneInterface.CharacterBone;
            if (null != characterBone)
                characterBone.Reborn();
            Logger.InfoFormat("PlayerCharacterBoneReborn");
        }
        
        private void Dead(PlayerEntity player)
        {
            if (null == player) return;
            
            var characterBone = player.characterBoneInterface.CharacterBone;
            if (null != characterBone)
                characterBone.Dead();
            Logger.InfoFormat("PlayerCharacterBoneDead");
        }

        #endregion
    }
}
