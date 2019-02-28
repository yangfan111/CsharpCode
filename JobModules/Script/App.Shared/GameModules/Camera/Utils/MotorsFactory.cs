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
        public static Motors CraeteMotors(Contexts contexts, CameraConfig config)
        {
            Motors motors = new Motors();

            var pose = motors.GetDict(SubCameraMotorType.Pose);

//            config.GetCameraConfigItem(ECameraViewMode.ThirdPerson)
            pose[(int) ECameraPoseMode.Stand] = new NormalPoseMotor(ECameraPoseMode.Stand, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new ThirdPersonActice());
            pose[(int) ECameraPoseMode.Crouch] = new NormalPoseMotor(ECameraPoseMode.Crouch, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new CrouchActice());
            pose[(int) ECameraPoseMode.Prone] = new NormalPoseMotor(ECameraPoseMode.Prone, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new ProneActice());
            pose[(int) ECameraPoseMode.Swim] = new NormalPoseMotor(ECameraPoseMode.Swim, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new SwimActice());
            pose[(int) ECameraPoseMode.Dying] = new NormalPoseMotor(ECameraPoseMode.Dying, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new DyingActice());
            pose[(int) ECameraPoseMode.Dead] = new DeadPoseMotor(ECameraPoseMode.Dead, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new DeadActice(), config.DeadConfig);
            pose[(int) ECameraPoseMode.Parachuting] = new NormalPoseMotor(ECameraPoseMode.Parachuting, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new ParachutingActice());

            pose[(int) ECameraPoseMode.ParachutingOpen] = new NormalPoseMotor(ECameraPoseMode.ParachutingOpen, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new ParachutingOpenActice());
            
            pose[(int) ECameraPoseMode.Gliding] = new GlidingPoseMotor(ECameraPoseMode.Gliding, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new GlidingActice());
            pose[(int) ECameraPoseMode.DriveCar] = new DrivePoseMotor(ECameraPoseMode.DriveCar, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance), contexts.vehicle, contexts.freeMove);
            pose[(int) ECameraPoseMode.AirPlane] = new AirplanePoseMotor(ECameraPoseMode.AirPlane, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance), contexts.vehicle, contexts.freeMove);

            pose[(int) ECameraPoseMode.Rescue] = new NormalPoseMotor(ECameraPoseMode.Rescue, config, new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),  new RescueActive());

            var free = motors.GetDict(SubCameraMotorType.Free);
            free[(int) ECameraFreeMode.On] = new FreeOnMotor();
            free[(int) ECameraFreeMode.Off] = new FreeOffMotor(config.FreeConfig.TransitionTime);

            var peek = motors.GetDict(SubCameraMotorType.Peek);
            peek[(int) ECameraPeekMode.Off] = new PeekOffMotor(config.PeekConfig.TransitionTime);
            peek[(int) ECameraPeekMode.Left] = new PeekOnMotor(false,config.PeekConfig);
            peek[(int) ECameraPeekMode.Right] = new PeekOnMotor(true,config.PeekConfig);

            var view = motors.GetDict(SubCameraMotorType.View);
            view[(int)ECameraViewMode.FirstPerson] = new FirstViewMotor();
            view[(int)ECameraViewMode.GunSight] = new GunSightMotor(contexts);
            view[(int)ECameraViewMode.ThirdPerson] = new ThirdViewMotor();

            CameraActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.Pose,
                (int) ECameraPoseMode.Parachuting, (player, state) =>
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
