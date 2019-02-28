using System.Collections.Generic;
using App.Shared;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.GameModules.Weapon;
using Assets.App.Shared.GameModules.Camera;
using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace Core.CameraControl.NewMotor.View
{
    public class GunSightMotor:AbstractCameraMotor
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GunSightMotor));
        private Contexts _contexts;

        public GunSightMotor(Contexts contexts)
        {
            _contexts = contexts;
            CameraActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.View, (int)ModeId, (player, state) =>
            {

                if (player.hasAppearanceInterface)
                {
                    if (!player.appearanceInterface.Appearance.IsFirstPerson)
                    {
                        player.appearanceInterface.Appearance.SetFirstPerson();
                        player.characterBoneInterface.CharacterBone.SetFirstPerson();
                    }
                    var speed = player.WeaponController().HeldWeaponAgent.FocusSpeed;
                    player.stateInterface.State.SetSight(speed);
                }

            });

            CameraActionManager.AddAction(CameraActionType.Leave, SubCameraMotorType.View, (int)ModeId,
                (player, state) =>
                {
                    var speed = player.WeaponController().HeldWeaponAgent.FocusSpeed;
                    player.stateInterface.State.CancelSight(speed);
                });
        }

        public override short ModeId
        {
            get { return (short)ECameraViewMode.GunSight;  }
        }
        public override int Order
        {
            get
            {
                return 3;
            }
        }

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            if (input.IsDead) return false;
            if (!input.CanWeaponGunSight) return false;
            if (state.IsFree()) return false;
            if (!state.GetMainConfig().CanSwitchView) return false;

            if (state.ViewMode == ECameraViewMode.GunSight &&
                (input.FilteredCameraFocus || input.ForceChangeGunSight || input.ForceInterruptGunSight))
            {
                if(input.ForceInterruptGunSight)
                {
                    if(Logger.IsDebugEnabled)
                    {
                        Logger.Debug("ForceInterruptGunSight");
                    }
                }
                return false;
            }

            //if (state.ViewMode.Equals(ECameraViewMode.ThirdPerson) &&  input.IsCameraFocus)
            if (state.ViewMode==ECameraViewMode.ThirdPerson && !input.ForceInterruptGunSight &&  (input.FilteredCameraFocus || input.ForceChangeGunSight))
            {
                return true;
            }

            //if (state.ViewMode.Equals(ECameraViewMode.FirstPerson) && input.IsCameraFocus)
            if (state.ViewMode==ECameraViewMode.FirstPerson && !input.ForceInterruptGunSight && (input.FilteredCameraFocus || input.ForceChangeGunSight))
            {
                return true;
            }

            return state.ViewMode == ECameraViewMode.GunSight;

        }

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state, SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
            //  output.Rotation = Quaternion.EulerAngles(player.orientation.Pitch,0,player.orientation.Roll);
            if (input.ChangeCamera)
            {
                subState.LastMode = (byte)(subState.LastMode==(int)ECameraViewMode.FirstPerson
                    ? ECameraViewMode.ThirdPerson
                    : ECameraViewMode.FirstPerson);
            }
            var fov = player.WeaponController().HeldWeaponAgent.GetGameFov(player.oxygenEnergyInterface.Oxygen.InShiftState);
            if(fov <= 0)
            {
                Logger.ErrorFormat("Illegal fov value {0}", fov);
                return;
            }
            output.Fov = fov;
           
        }

        public override HashSet<short> ExcludeNextMotor()
        {
            return EmptyHashSet;  
        }

        public override void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state)
        {
           
        }

        public override void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state, PlayerEntity player)
        {
            
        }
    }
}