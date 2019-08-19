using System.Collections.Generic;
using System.Data;
using App.Shared;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.Player;
using Assets.App.Shared.GameModules.Camera;
using Core.Utils;
using UnityEngine;
using Utils.Appearance.Effects;
using XmlConfig;

namespace Core.CameraControl.NewMotor.View
{
    public class FirstViewMotor:AbstractCameraMotor
    {
        public FirstViewMotor(Motors m):base(m)
        {
            _motors.ActionManager.BindKeyAction(CameraActionType.Enter, SubCameraMotorType.View, (int)ModeId, (player, state) =>
            {
                
                if (player.hasAppearanceInterface && !player.appearanceInterface.Appearance.IsFirstPerson)
                {
                    player.appearanceInterface.Appearance.SetFirstPerson();
                    player.characterBoneInterface.CharacterBone.SetFirstPerson();
                    player.UpdateCameraArchorPostion();
                    var playerUtils = EffectUtility.GetEffect(player.RootGo(), "PlayerUtils");
                    if (playerUtils != null)
                        playerUtils.SetParam("FirstViewBegin", (object)player.RootGo().gameObject);
                }
            });
        }

        public override short ModeId
        {
            get { return (short)ECameraViewMode.FirstPerson; }
        }

        public override int Order
        {
            get
            {
                return 2;
            }
        }

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            var config = input.GetPoseConfig(state.GetMainMotor().NowMode);
            if (!config.CanSwitchView) 
                return false;
            if (input.IsVariant) return false;
            if (!SharedConfig.IsOffline && input.LockViewByRoom && input.ModelLoaded && !input.IsDead) return true;
            
            if (state.IsFree()) 
                return false;
            
            if (state.ViewMode == ECameraViewMode.FirstPerson && input.FilteredChangeCamera)
            {
                return false;
            }

            if (state.ViewMode == ECameraViewMode.ThirdPerson && input.FilteredChangeCamera)
            {
                return true;
            }

            if (state.ViewMode == ECameraViewMode.GunSight &&
                (input.FilteredCameraFocus || input.InterruptCameraFocus) &&
                state.LastViewMode == ECameraViewMode.FirstPerson)
            {
                return true;
            }

            if (input.LastViewByOrder == (short) ECameraViewMode.FirstPerson)
            {
                return true;

            }
            return state.ViewMode == ECameraViewMode.FirstPerson;
        }

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state, SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {

        }

        public override HashSet<short> ExcludeNextMotor()
        {
            return EmptyHashSet;
        }

        public override void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state)
        {
            return;
        }

        public override void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state, PlayerEntity player)
        {
            return;
        }
    }
}