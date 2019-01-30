using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Camera.Motor.Peek;
using App.Shared.GameModules.Camera.Motor.Pose;
using App.Shared.GameModules.Camera.Utils;
using Assets.App.Shared.GameModules.Camera.Motor.Free;
using Assets.App.Shared.GameModules.Camera.Motor.Pose;
using Core.CameraControl.NewMotor;
using Core.CameraControl.NewMotor.View;
using UnityEngine;
using Utils.Utils;
using XmlConfig;
using Random = UnityEngine.Random;

namespace Assets.App.Shared.GameModules.Camera.Utils
{
    public class MotorsFactory
    {
        public static Motors CraeteMotors(CameraConfig config,VehicleContext vehicle, FreeMoveContext freeMoveContext)
        {
            Motors motors = new Motors();

            var pose = motors.GetDict(SubCameraMotorType.Pose);

            pose[(int) ECameraPoseMode.Stand] = new NormalPoseMotor(ECameraPoseMode.Stand, config.GerCameraConfigItem(ECameraConfigType.ThirdPerson), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new ThirdPersonActice());
            pose[(int) ECameraPoseMode.Crouch] = new NormalPoseMotor(ECameraPoseMode.Crouch, config.GerCameraConfigItem(ECameraConfigType.Crouch), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new CrouchActice());
            pose[(int) ECameraPoseMode.Prone] = new NormalPoseMotor(ECameraPoseMode.Prone, config.GerCameraConfigItem(ECameraConfigType.Prone), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new ProneActice());
            pose[(int) ECameraPoseMode.Swim] = new NormalPoseMotor(ECameraPoseMode.Swim, config.GerCameraConfigItem(ECameraConfigType.Swim), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new SwimActice());
            pose[(int) ECameraPoseMode.Dying] = new NormalPoseMotor(ECameraPoseMode.Dying, config.GerCameraConfigItem(ECameraConfigType.Dying), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new DyingActice());
            pose[(int) ECameraPoseMode.Dead] = new DeadPoseMotor(ECameraPoseMode.Dead, config.GerCameraConfigItem(ECameraConfigType.Dead), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new DeadActice(), config.DeadConfig);
            pose[(int) ECameraPoseMode.Parachuting] = new NormalPoseMotor(ECameraPoseMode.Parachuting, config.GerCameraConfigItem(ECameraConfigType.Parachuting), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new ParachutingActice(),400);

            pose[(int) ECameraPoseMode.ParachutingOpen] = new NormalPoseMotor(ECameraPoseMode.ParachutingOpen, config.GerCameraConfigItem(ECameraConfigType.ParachutingOpen), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new ParachutingOpenActice(),300);
            
            pose[(int) ECameraPoseMode.Gliding] = new GlidingPoseMotor(ECameraPoseMode.Gliding, config.GerCameraConfigItem(ECameraConfigType.Gliding), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new GlidingActice());
            pose[(int) ECameraPoseMode.DriveCar] = new DrivePoseMotor(ECameraPoseMode.DriveCar, config.GerCameraConfigItem(ECameraConfigType.DriveCar), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),vehicle,freeMoveContext);
            pose[(int) ECameraPoseMode.AirPlane] = new AirplanePoseMotor(ECameraPoseMode.AirPlane, config.GerCameraConfigItem(ECameraConfigType.AirPlane), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),vehicle,freeMoveContext);

            pose[(int) ECameraPoseMode.Rescue] = new NormalPoseMotor(ECameraPoseMode.Rescue, config.GerCameraConfigItem(ECameraConfigType.Rescue), new HashSet<ECameraPoseMode>(CommonEnumEqualityComparer<ECameraPoseMode>.Instance),  new RescueActive());

            var free = motors.GetDict(SubCameraMotorType.Free);
            free[(int) ECameraFreeMode.On] = new FreeOnMotor();
            free[(int) ECameraFreeMode.Off] = new FreeOffMotor(config.FreeConfig.TransitionTime);

            var peek = motors.GetDict(SubCameraMotorType.Peek);
            peek[(int) ECameraPeekMode.Off] = new PeekOffMotor(config.PeekConfig.TransitionTime);
            peek[(int) ECameraPeekMode.Left] = new PeekOnMotor(false,config.PeekConfig);
            peek[(int) ECameraPeekMode.Right] = new PeekOnMotor(true,config.PeekConfig);

            var view = motors.GetDict(SubCameraMotorType.View);
            view[(int)ECameraViewMode.FirstPerson] = new FirstViewMotor();
            view[(int)ECameraViewMode.GunSight] = new GunSightMotor();
            view[(int)ECameraViewMode.ThirdPerson] = new ThirdViewMotor();

            CameraActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.Pose,
                (int) ECameraConfigType.Parachuting, (player, state) =>
                {
                    var cameraEulerAngle = player.cameraFinalOutputNew.EulerAngle;
                    var carEulerAngle = player.cameraArchor.ArchorEulerAngle;
                    var t = cameraEulerAngle - carEulerAngle;
                    state.FreeYaw = t.y;
                    state.FreePitch = t.x;
                });

            return motors;
        }
    }

  
}
