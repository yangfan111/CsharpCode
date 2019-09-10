using System.Collections.Generic;
using App.Shared.GameModules.Camera.Utils;
using Assets.App.Shared.GameModules.Camera;
using Core.CameraControl.NewMotor;
using Core.Configuration;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Camera.Motor.Pose
{
    class DeadPoseMotor:NormalPoseMotor
    {
        public DeadPoseMotor(ECameraPoseMode modeId, HashSet<ECameraPoseMode> excludes, IMotorActive active,DeadCameraConfig deadConfig, Motors m) : base(modeId, excludes, active,m)
        {
            _motors.ActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.Pose, (int)modeId,
                (player, state) =>
                {
                    player.cameraStateOutputNew.LastPitchWhenAlive = player.cameraFinalOutputNew.EulerAngle.x;
                    //中途加入时不切三人称
                    if(player.gamePlay.IsObserving())
                        player.characterBoneInterface.CharacterBone.SetThridPerson();
                });
        }

        private Vector3 _finalRotation = Vector3.zero;

        public override Vector3 FinalEulerAngle
        {
            get { return _finalRotation; }
        }

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state, SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
            _finalRotation = input.GetDeadConfig().Roatation;
            _finalRotation.x -= player.cameraStateOutputNew.LastPitchWhenAlive;
            base.CalcOutput(player, input, state, subState, output, last, clientTime);
        }
    }
}