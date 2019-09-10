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

            pose[(int) ECameraPoseMode.Stand] = new NormalPoseMotor(ECameraPoseMode.Stand,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),
                new ThirdPersonActice(), motors);
            pose[(int) ECameraPoseMode.Crouch] = new NormalPoseMotor(ECameraPoseMode.Crouch,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),
                new CrouchActice(), motors);
            pose[(int) ECameraPoseMode.Prone] = new NormalPoseMotor(ECameraPoseMode.Prone,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),
                new ProneActice(), motors);
            pose[(int) ECameraPoseMode.Swim] = new NormalPoseMotor(ECameraPoseMode.Swim,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance), new SwimActice(),
                motors);
            pose[(int) ECameraPoseMode.Dying] = new NormalPoseMotor(ECameraPoseMode.Dying,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),
                new DyingActice(), motors);
            pose[(int) ECameraPoseMode.Dead] = new DeadPoseMotor(ECameraPoseMode.Dead,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance), new DeadActice(),
                config.GetRoleConfig().DeadConfig, motors);
            pose[(int) ECameraPoseMode.Parachuting] = new NormalPoseMotor(ECameraPoseMode.Parachuting,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),
                new ParachutingActice(), motors);
            pose[(int) ECameraPoseMode.ParachutingOpen] = new NormalPoseMotor(ECameraPoseMode.ParachutingOpen,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),
                new ParachutingOpenActice(), motors);
            pose[(int) ECameraPoseMode.Gliding] = new GlidingPoseMotor(ECameraPoseMode.Gliding,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),
                new GlidingActice(), motors);
            pose[(int) ECameraPoseMode.DriveCar] = new DrivePoseMotor(ECameraPoseMode.DriveCar,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance), contexts.vehicle,
                contexts.freeMove, motors);
            pose[(int) ECameraPoseMode.AirPlane] = new AirplanePoseMotor(ECameraPoseMode.AirPlane,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance), contexts.vehicle,
                contexts.freeMove, motors);
            pose[(int) ECameraPoseMode.Climb] = new NormalPoseMotor(ECameraPoseMode.Climb,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),
                new ClimbActive(), motors);
            pose[(int) ECameraPoseMode.Rescue] = new NormalPoseMotor(ECameraPoseMode.Rescue,
                new HashSet<ECameraPoseMode>(CommonIntEnumEqualityComparer<ECameraPoseMode>.Instance),
                new RescueActive(), motors);

            var free = motors.GetDict(SubCameraMotorType.Free);
            free[(int) ECameraFreeMode.On] = new FreeOnMotor(motors);
            free[(int) ECameraFreeMode.Off] = new FreeOffMotor(config.GetRoleConfig().FreeConfig.TransitionTime, motors);

            var peek = motors.GetDict(SubCameraMotorType.Peek);
            peek[(int) ECameraPeekMode.Off] = new PeekOffMotor(config.GetRoleConfig().PeekConfig.TransitionTime, motors);
            peek[(int) ECameraPeekMode.Left] = new PeekOnMotor(false,config.GetRoleConfig().PeekConfig, motors);
            peek[(int) ECameraPeekMode.Right] = new PeekOnMotor(true,config.GetRoleConfig().PeekConfig, motors);

            var view = motors.GetDict(SubCameraMotorType.View);
            view[(int)ECameraViewMode.FirstPerson] = new FirstViewMotor(motors);
            view[(int)ECameraViewMode.GunSight] = new GunSightMotor(motors);
            view[(int)ECameraViewMode.ThirdPerson] = new ThirdViewMotor(motors);

            motors.ActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.Pose,
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
