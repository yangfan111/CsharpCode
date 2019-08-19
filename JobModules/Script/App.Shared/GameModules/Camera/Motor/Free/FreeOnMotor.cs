using System.Collections.Generic;
using App.Shared.Components.Player;
using Core.CameraControl;
using Core.CameraControl.NewMotor;
using UnityEngine;
using XmlConfig;

namespace Assets.App.Shared.GameModules.Camera.Motor.Free
{
    class FreeOnMotor : AbstractCameraMotor
    {
        private float _transitionTime ;
        private SubCameraMotorType _motorType;
        
        public FreeOnMotor(Motors m): base(m)
        {
            _motorType = SubCameraMotorType.View;
        }

        public override short ModeId
        {
            get { return (short) ECameraFreeMode.On; }
        }

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            if (state.IsFristPersion()) return false;
            if (state.GetMainMotor().NowMode == (short)ECameraPoseMode.Dead) return false;
            if (state.PeekMode != ECameraPeekMode.Off) return false;
            return state.GetMainMotor().NowMode == (short)ECameraPoseMode.AirPlane ||
                   state.GetMainMotor().NowMode == (short)ECameraPoseMode.Parachuting ||
                   state.GetMainMotor().NowMode == (short)ECameraPoseMode.ParachutingOpen ||
                   input.IsCameraFree || input.IsDriveCar ;
        }

        public override int Order
        {
            get { return 1; }
        }

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state, SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
            output.EulerAngle = new Vector3(state.FreePitch, state.FreeYaw, 0);
        }

        public override void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state, PlayerEntity player)
        {
        }

        public override HashSet<short> ExcludeNextMotor()
        {
            return EmptyHashSet;
        }

        public override void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state)
        {
            var config = input.GetPoseConfig(state.GetMainMotor().NowMode);
            float yaw = state.FreeYaw + input.ArchorYaw;
            float pitch = state.FreePitch + input.ArchorPitch;

            yaw = CalculateFrameVal(yaw, input.DeltaYaw, config.YawLimit);
            pitch = CalculateFrameVal(pitch, input.DeltaPitch, config.PitchLimit);

            state.LastFreeYaw = player.cameraStateNew.FreeYaw;
            state.LastFreePitch = player.cameraStateNew.FreePitch;
            state.FreeYaw = YawPitchUtility.Normalize(yaw - input.ArchorYaw);
            state.FreePitch = YawPitchUtility.Normalize(pitch - input.ArchorPitch);
        }
    }
}