using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core.CameraControl;
using Core.CameraControl.NewMotor;
using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace Assets.App.Shared.GameModules.Camera.Motor.Pose
{
    class DrivePoseMotor : AbstractCameraMainMotor
    {
        private short _modeId;
        private HashSet<short> excludes;
     
        private float transitionTime = 200f;
     

        public DrivePoseMotor(ECameraPoseMode modeId,
            CameraConfig config,
            HashSet<ECameraPoseMode> excludes,
            VehicleContext vehicleContext,
            FreeMoveContext freeMoveContext     
        )
        {
            _modeId = (short) modeId;
          
            this.excludes = new HashSet<short>();
            foreach (var e in excludes)
            {
                this.excludes.Add((short) e);
            }

            _config = config.GetCameraConfigItem(modeId);

            CameraActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.Pose, (int)modeId, 
                (player, state) =>
                {
                    if (player.IsOnVehicle())
                    {
                        var vehicle = vehicleContext.GetEntityWithEntityKey(player.controlledVehicle.EntityKey);
                        player.controlledVehicle.CameraAnchorOffset = vehicle.vehicleAssetInfo.CameraAnchorOffset;
                        player.controlledVehicle.CameraDistance = vehicle.vehicleAssetInfo.CameraDistance;
                        player.controlledVehicle.CameraRotationDamping = vehicle.vehicleAssetInfo.CameraRotationDamping;
                    }

                    var cameraEulerAngle = player.cameraFinalOutputNew.EulerAngle;

                    var carEulerAngle = player.cameraArchor.ArchorEulerAngle;

                    var t = cameraEulerAngle - carEulerAngle;
                    state.FreeYaw = t.y;
                    state.FreePitch = t.x;
                });
            CameraActionManager.AddAction(CameraActionType.Leave, SubCameraMotorType.Pose, (int) modeId,
                (player, state) =>
                {
                    var rotation = player.cameraFinalOutputNew.EulerAngle;
                    player.orientation.Yaw = YawPitchUtility.Normalize(rotation.y);
                    player.orientation.Pitch = YawPitchUtility.Normalize(rotation.x);

                    state.LastFreePitch = 0;
                    state.LastFreeYaw = 0;
                    state.FreeYaw = 0f;
                    state.FreePitch = 0f;
                });
        }


        public override short ModeId
        {
            get { return _modeId; }
        }

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return input.IsDriveCar;
        }

    

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state,
            SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
           
            var cameraAnchorOffset = Vector3.zero;
            var cameraDistance = 0.0f;
            if (player.IsOnVehicle())
            {
                var controlledVehicle = player.controlledVehicle;
                cameraAnchorOffset = controlledVehicle.CameraAnchorOffset;
                cameraDistance = controlledVehicle.CameraDistance;
            }

            output.Far = _config.Far;
            output.ArchorOffset = FinalArchorOffset + cameraAnchorOffset;
            output.ArchorPostOffset = FinalArchorPostOffset;
            output.Offset = FinalOffset;
            output.Offset.z -= cameraDistance;
            output.ArchorEulerAngle = FinalEulerAngle;
            output.Fov = FinalFov;
           
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
            get { return new Vector3(0,0,-_config.Distance); }
        }

      

        public override float FinalFov
        {
            get { return _config.Fov; }
        }

        public override int Order
        {
            get { return _config.Order; }
        }
    }
}