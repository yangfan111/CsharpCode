using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.App.Shared.GameModules.Camera;
using Core.CameraControl;
using Core.CameraControl.NewMotor;
using UnityEngine;
using XmlConfig;
using Random = UnityEngine.Random;

namespace App.Shared.GameModules.Camera.Motor.Pose
{
    class GlidingPoseMotor : NormalPoseMotor
    {
        BezierUtil _bezierUtil = new BezierUtil();
        public GlidingPoseMotor(ECameraPoseMode modeId,
            CameraConfig config,
            HashSet<ECameraPoseMode> excludes,
            IMotorActive active
        ) : base(modeId, config, excludes, active)
        {
            _bezierUtil.CreateRandomPoints(1f,1.5f,-2,2,-0.3f,0.3f,4);
        }


        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state,
            SubCameraMotorState subState,
            DummyCameraMotorOutput output,
            ICameraNewMotor last, int clientTime)
        {
            base.CalcOutput(player, input, state, subState, output, last, clientTime);
            int diffTime = clientTime - subState.ModeTime;
            var r = _bezierUtil.GetResult(diffTime / 1000f);
            output.EulerAngle.z = r.y;
          
            output.PostOffset.x += r.z;
        }
    }
}