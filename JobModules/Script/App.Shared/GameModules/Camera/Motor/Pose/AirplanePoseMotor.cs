using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CameraControl.NewMotor;
using UnityEngine;
using XmlConfig;

namespace Assets.App.Shared.GameModules.Camera.Motor.Pose
{
    class AirplanePoseMotor:DrivePoseMotor
    {
       

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.IsAirPlane;
        }

        public AirplanePoseMotor(ECameraPoseMode modeId, CameraConfig config, HashSet<ECameraPoseMode> excludes, VehicleContext vehicleContext, FreeMoveContext freeMoveContext) :
            base(modeId, config, excludes, vehicleContext, freeMoveContext)
        {
        }
    }
}
