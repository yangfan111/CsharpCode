using System.Collections.Generic;
using Assets.App.Shared.GameModules.Camera;
using UnityEngine;
using XmlConfig;

namespace Core.CameraControl.NewMotor.View
{
    public class FirstViewMotor:AbstractCameraMotor
    {
        public FirstViewMotor()
        {
            CameraActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.View, (int)ModeId, (player, state) =>
            {
                if (player.hasAppearanceInterface && !player.appearanceInterface.Appearance.IsFirstPerson)
                {
                    player.appearanceInterface.Appearance.SetFirstPerson();
                    player.characterBoneInterface.CharacterBone.SetFirstPerson();
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
            if (state.IsFree()) return false;
            if (!state.GetMainConfig().CanSwitchView) return false;
            //if (state.ViewMode.Equals(ECameraViewMode.FirstPerson) && (input.ChangeCamera))
            if (state.ViewMode == ECameraViewMode.FirstPerson && input.FilteredChangeCamera)
            {
                return false;
            }
            //if (state.ViewMode.Equals(ECameraViewMode.ThirdPerson) &&  input.ChangeCamera)
            if (state.ViewMode  == ECameraViewMode.ThirdPerson &&  input.FilteredChangeCamera)
            {
                return true;
            }

            //if (state.ViewMode.Equals(ECameraViewMode.GunSight) && input.IsCameraFocus && state.LastViewMode.Equals(ECameraViewMode.FirstPerson))
            if (state.ViewMode == ECameraViewMode.GunSight && (input.FilteredCameraFocus || input.ForceChangeGunSight || input.ForceInterruptGunSight) && state.LastViewMode == ECameraViewMode.FirstPerson)
            {
                return true;
            }

            return state.ViewMode == ECameraViewMode.FirstPerson;
        }

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state, SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
            //output.Rotation = Quaternion.EulerAngles(player.orientation.Pitch,0,0);
            return;
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