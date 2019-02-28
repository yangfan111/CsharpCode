using System.Collections.Generic;
using App.Shared.GameModules.Camera.Utils;
using Assets.App.Shared.GameModules.Camera;
using Core.CameraControl.NewMotor;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Camera.Motor.Pose
{
    class DeadPoseMotor:NormalPoseMotor
    {
        private DeadCameraConfig _config;
        public DeadPoseMotor(ECameraPoseMode modeId, CameraConfig config, HashSet<ECameraPoseMode> excludes, IMotorActive active,DeadCameraConfig deadConfig) : base(modeId, config, excludes, active)
        {
            _config = deadConfig;

            CameraActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.Pose, (int)modeId,
                (player, state) =>
                {
                    player.orientation.Pitch = 0;
                    Debug.Log("Enter dead pose");
                });
            _finalRotation = deadConfig.Roatation;
        }
        private Vector3 _finalRotation = Vector3.zero;
        public override Vector3 FinalEulerAngle
        {
            get { return _finalRotation; }
        }

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state, SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
            
            _finalRotation = _config.Roatation;
            base.CalcOutput(player, input, state, subState, output, last, clientTime);
        }
    }
}