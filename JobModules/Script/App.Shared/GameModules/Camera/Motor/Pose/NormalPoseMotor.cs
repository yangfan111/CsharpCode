using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.FreeMove;
using App.Shared.GameModules.Camera.Utils;
using Assets.App.Shared.GameModules.Camera;
using Assets.App.Shared.GameModules.Camera.Motor.Pose;
using Core.CameraControl;
using Core.CameraControl.NewMotor;
using Core.Utils;
using UnityEngine;
using Utils.Compare;
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
            return input.NextPostureState == PostureInConfig.Swim;
        }
    }

    internal class DyingActice : IMotorActive
    {
        public bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.NextPostureState == PostureInConfig.Dying;
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
        private readonly float Epsilon = 0.01f;
        protected float _transitionTime ;

        public NormalPoseMotor(ECameraPoseMode modeId,
            CameraConfig config,
            HashSet<ECameraPoseMode> excludes,
            IMotorActive active
        )
        {
            _modeId = (short)modeId;
            _motorType = SubCameraMotorType.Pose;
            
            this._excludes = new HashSet<short>();
            foreach (var e in excludes)
            {
                this._excludes.Add((short)e);
            }

            _config = config.GetCameraConfigItem(modeId);
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

        public override Vector3 FinalArchorOffset
        {
            get { return _config.AnchorOffset; }
        }

        public override Vector3 FinalArchorPostOffset
        {
            get { return _config.ScreenOffset; }
        }


        public override Vector3 FinalOffset
        {
            get { return new Vector3(0, 0, -_config.Distance); }
        }


        public override float FinalFov
        {
            get { return _config.Fov; }
        }

        public override int Order
        {
            get { return _config.Order; }
        }
        
        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state,
            SubCameraMotorState subState,
            DummyCameraMotorOutput output,
            ICameraNewMotor last, int clientTime)
        {
            
            output.Far = _config.Far;
            output.Near = _config.Near;

            _transitionTime = CameraUtility.GetPostureTransitionTime(_motorType, subState);
            var elapsedPercent = ElapsedPercent(clientTime, subState.ModeTime, _transitionTime);
            var realPercent = EaseInOutQuad(0, 1, elapsedPercent);
            if (state.IsFristPersion())
            {
                //一人称和瞄准相机没有偏移
                output.Fov = Mathf.Lerp(last.FinalFov, FinalFov, realPercent);
            }
            else
            {
                if (last is AirplanePoseMotor || last is DrivePoseMotor)
                {
                    realPercent = 1;
                }
                
                output.ArchorOffset = Vector3.Lerp(last.FinalArchorOffset, FinalArchorOffset, realPercent);
                output.ArchorPostOffset =
                    Vector3.Lerp(last.FinalArchorPostOffset, FinalArchorPostOffset, realPercent);
                output.Offset = Vector3.Lerp(last.FinalOffset, FinalOffset, realPercent);
                output.ArchorEulerAngle = Vector3.Lerp(last.FinalEulerAngle, FinalEulerAngle, realPercent);
                output.Fov = Mathf.Lerp(last.FinalFov, FinalFov, realPercent);
            }
        }


        private bool CanRotatePlayer(ICameraMotorState state)
        {
            if (state.IsFree()) return false;

            return true;
        }

        public override void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state, PlayerEntity player)
        {
            if (!input.IsDead && CanRotatePlayer(state))
            {
                float newDeltaAngle = input.DeltaYaw;

                if (player.playerRotateLimit.LimitAngle)
                {
                    var candidateAngle = YawPitchUtility.Normalize(player.orientation.Yaw) + input.DeltaYaw;
                    candidateAngle = Mathf.Clamp(candidateAngle, player.playerRotateLimit.LeftBound,
                        player.playerRotateLimit.RightBound);
                    player.orientation.Yaw = CalculateFrameVal(candidateAngle, 0f, _config.YawLimit);
                }
                else
                {
                    player.orientation.Yaw = CalculateFrameVal(player.orientation.Yaw, newDeltaAngle, _config.YawLimit);
                }
                
 //               newDeltaAngle = player.characterContoller.Value.PreRotateAngle(player.orientation.ModelView, player.position.Value,input.DeltaYaw, input.FrameInterval * 0.001f);
                
//                Logger.DebugFormat("deltaAngle:{0},prevAngle:{1}, curYaw:{2}, input.DeltaYaw:{4},player.orientationLimit.LimitAngle:{5}",
//                    newDeltaAngle, 
//                    player.orientation.Yaw,
//                    CalculateFrameVal(player.orientation.Yaw, newDeltaAngle, _config.YawLimit),
//                    input.DeltaYaw
//                );

                var deltaPitch = HandlePunchPitch(player, input);
                player.orientation.Pitch =
                    CalculateFrameVal(player.orientation.Pitch, deltaPitch, _config.PitchLimit);
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