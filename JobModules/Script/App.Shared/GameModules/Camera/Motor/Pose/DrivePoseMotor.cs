﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core.CameraControl;
using Core.CameraControl.NewMotor;
using Core.Configuration;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace Assets.App.Shared.GameModules.Camera.Motor.Pose
{
    class DrivePoseMotor : AbstractCameraMainMotor
    {
        private short _modeId;
        private HashSet<short> excludes;
     
        private float transitionTime = 200f;
        private int _order;
        private Motors _motors;

        public DrivePoseMotor(ECameraPoseMode modeId,
            HashSet<ECameraPoseMode> excludes,
            VehicleContext vehicleContext,
            FreeMoveContext freeMoveContext,
            Motors m
        ):base(m)
        {
            _modeId = (short) modeId;
            _motors = m;
          
            this.excludes = new HashSet<short>();
            foreach (var e in excludes)
            {
                this.excludes.Add((short) e);
            }

            _order = SingletonManager.Get<CameraConfigManager>().GetRoleConfig()
                .GetCameraConfigItem((ECameraPoseMode) _modeId).Order;
            
            _motors.ActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.Pose, (int)modeId, 
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
            _motors.ActionManager.AddAction(CameraActionType.Leave, SubCameraMotorType.Pose, (int) modeId,
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

            output.Far = input.GetPoseConfig(_modeId).Far;
            output.ArchorOffset = input.GetPoseConfig(_modeId).AnchorOffset + cameraAnchorOffset;
            output.ArchorPostOffset = input.GetPoseConfig(_modeId).ScreenOffset;
            output.Offset = new Vector3(0, 0, -input.GetPoseConfig(_modeId).Distance);
            output.Offset.z -= cameraDistance;
            output.ArchorEulerAngle = FinalEulerAngle;
            output.Fov = input.GetPoseConfig(_modeId).Fov;

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

        public override int Order
        {
            get { return _order; }
        }
    }
}