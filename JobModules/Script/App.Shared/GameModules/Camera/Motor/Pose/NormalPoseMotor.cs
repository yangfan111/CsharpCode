using App.Shared.GameModules.Camera.Utils;
using Assets.App.Shared.GameModules.Camera;
using Assets.App.Shared.GameModules.Camera.Motor.Pose;
using Core.CameraControl;
using Core.CameraControl.NewMotor;
using System.Collections.Generic;
using Core.Configuration;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Camera.Motor.Pose
{
    class ThirdPersonActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.NextPostureState == PostureInConfig.Stand ||
                    input.NextPostureState == PostureInConfig.Jump ||
                    input.NextPostureState == PostureInConfig.Land ||
                    input.NextPostureState == PostureInConfig.ProneToStand; 
        }
    }

    internal class CrouchActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.NextPostureState == PostureInConfig.Crouch ||
                input.NextPostureState == PostureInConfig.ProneToCrouch;
        }
    }

    internal class ProneActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.NextPostureState == PostureInConfig.Prone ||
                input.NextPostureState == PostureInConfig.ProneTransit;
        }
    }

    internal class SwimActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.NextPostureState == PostureInConfig.Swim || input.NextPostureState == PostureInConfig.Dive;
        }
    }

    internal class DyingActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.NextPostureState == PostureInConfig.Dying ||
                   input.NextPostureState == PostureInConfig.DyingTransition;
        }
    }

    internal class DeadActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.IsDead;
        }
    }

    internal class GlidingActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.ActionState == ActionInConfig.Gliding;
        }
    }

    internal class ParachutingActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.ActionState == ActionInConfig.Parachuting && input.IsParachuteAttached;
        }
    }
    internal class ParachutingOpenActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.ActionState == ActionInConfig.Parachuting && !input.IsParachuteAttached;
        }
    }
    
    internal class ClimbActive : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.CurrentPostureState == PostureInConfig.Climb;
        }
    }
    
    public class RescueActive : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.ActionKeepState == ActionKeepInConfig.Rescue ;
        }
    }

    class NormalPoseMotor : AbstractCameraMainMotor
    {
        private short _modeId;
        private SubCameraMotorType _motorType;
        private HashSet<short> _excludes;
        private IMotorActive _active;
        private int _order;
        private readonly float Epsilon = 0.01f;
        protected float _transitionTime ;

        public NormalPoseMotor(ECameraPoseMode modeId,
            HashSet<ECameraPoseMode> excludes,
            IMotorActive active, Motors m):base(m)
        {
            _modeId = (short)modeId;
            _motorType = SubCameraMotorType.Pose;
            
            this._excludes = new HashSet<short>();
            foreach (var e in excludes)
            {
                this._excludes.Add((short)e);
            }

            _order = SingletonManager.Get<CameraConfigManager>().GetRoleConfig()
                .GetCameraConfigItem((ECameraPoseMode) _modeId).Order;
            
            _active = active;
        }
        
        public override short ModeId
        {
            get { return _modeId; }
        }

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return _active.IsActive(input, state);
        }

        public override int Order 
        {
            get { return _order; }
        }
        
        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state,
            SubCameraMotorState subState,
            DummyCameraMotorOutput output,
            ICameraNewMotor last, int clientTime)
        {
            
            output.Far = input.GetPoseConfig(_modeId).Far;
            output.Near = input.GetPoseConfig(_modeId).Near;
            output.ForbidDetect = input.GetPoseConfig(_modeId).ForbidDetect;

            _transitionTime = CameraUtility.GetPostureTransitionTime(_motorType, subState);
            var elapsedPercent = ElapsedPercent(clientTime, subState.ModeTime, _transitionTime);
            var realPercent = EaseInOutQuad(0, 1, elapsedPercent);

            if (state.IsFristPersion())
            {
                //一人称和瞄准相机没有偏移
                output.Fov = Mathf.Lerp(input.GetPoseConfig(last.ModeId).Fov,
                    input.GetPoseConfig(_modeId).Fov, realPercent);
            }
            else
            {
                if (last is AirplanePoseMotor || last is DrivePoseMotor )
                {
                    realPercent = 1;
                }

                output.ArchorOffset = Vector3.Lerp(input.GetPoseConfig(last.ModeId).AnchorOffset,
                    input.GetPoseConfig(_modeId).AnchorOffset, realPercent);
                output.ArchorPostOffset =
                    Vector3.Lerp(input.GetPoseConfig(last.ModeId).ScreenOffset,
                        input.GetPoseConfig(_modeId).ScreenOffset, realPercent);
                output.Offset = Vector3.Lerp(new Vector3(0, 0, -input.GetPoseConfig(last.ModeId).Distance),
                    new Vector3(0, 0, -input.GetPoseConfig(_modeId).Distance), realPercent);
                output.ArchorEulerAngle = Vector3.Lerp(last.FinalEulerAngle, FinalEulerAngle, realPercent);
                output.Fov = Mathf.Lerp(input.GetPoseConfig(last.ModeId).Fov,
                    input.GetPoseConfig(_modeId).Fov, realPercent);
            }

        }
        
        private bool CanRotatePlayer(ICameraMotorState state)
        {
            if (state.IsFristPersion() && state.GetMainMotor().NowMode == (byte) ECameraPoseMode.Climb)
                return false;
            if (state.IsFree()) return false;
            return true;
        }

        public override void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state, PlayerEntity player)
        {
            if ((!input.IsDead || input.IsDead && player.gamePlay.IsObserving()) && CanRotatePlayer(state))
            {
                float newDeltaAngle = input.DeltaYaw;
                if (player.playerRotateLimit.LimitAngle)
                {
                    var candidateAngle = YawPitchUtility.Normalize(player.orientation.Yaw) + input.DeltaYaw;
                    candidateAngle = Mathf.Clamp(candidateAngle, player.playerRotateLimit.LeftBound,
                        player.playerRotateLimit.RightBound);
                    player.orientation.Yaw = CalculateFrameVal(candidateAngle, 0f, input.GetPoseConfig(_modeId).YawLimit);
                }
                else
                {
                    player.orientation.Yaw = CalculateFrameVal(player.orientation.Yaw, newDeltaAngle,input.GetPoseConfig(_modeId).YawLimit);
                }
               
                var deltaPitch = HandlePunchPitch(player, input);
                player.orientation.Pitch =
                    CalculateFrameVal(player.orientation.Pitch, deltaPitch, input.GetPoseConfig(_modeId).PitchLimit);
            }
        }
        
        private float HandlePunchPitch(PlayerEntity player, ICameraMotorInput input)
        {
            if(player.orientation.PunchPitch > -0.001 || input.DeltaPitch < 0.001)
            {
                return input.DeltaPitch;
            }

            var newPitch = input.DeltaPitch + player.orientation.PunchPitch;
            if(newPitch < 0)
            {
                player.orientation.PunchPitch = newPitch;
                return 0; 
            }
            player.orientation.PunchPitch = 0;
            return newPitch;
        }

        public override HashSet<short> ExcludeNextMotor()
        {
            return _excludes;
        }

        public override void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state)
        {
            UpdatePlayerRotation(input, state, player);
        }
    }
}