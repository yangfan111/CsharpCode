using System;
using System.Collections.Generic;
using Core.Interpolate;
using Core.ObjectPool;
using UnityEngine;
using Utils.Appearance;
using XmlConfig;

namespace Core.CameraControl.NewMotor
{

    public interface ICameraMotorInput : IVariableCameraInput
    {
        bool ChangeCamera { get; set; }
        bool FilteredChangeCamera { get; set; }
        bool IsCameraFocus { get; set; }
        bool FilteredCameraFocus { get; set; }
        PostureInConfig NextPostureState { get; set; }
        PostureInConfig CurrentPostureState { get; set; }
        LeanInConfig LeanState { get; set; }
        ActionInConfig ActionState { get; set; }
        ActionKeepInConfig ActionKeepState { get; set; }
        bool IsDriveCar { get; set; }
        bool IsAirPlane { get; set; }
        bool IsDead { get; set; }
        bool CanWeaponGunSight { get; set; }
        bool IsCmdRun { get; set; }
        bool IsCmdMoveVertical { get; set; }
        float ArchorYaw { get; set; }
        float ArchorPitch { get; set; }
        bool IsParachuteAttached { get; set; }
        bool ForceChangeGunSight { get; set; }
        bool ForceInterruptGunSight { get; set; }
        bool IsChange(ICameraMotorInput r);
    }

    public class SubCameraMotorState
    {
        public byte NowMode;
        public byte LastMode;
        public int ModeTime;

        public SubCameraMotorState()
        {
        }

        public void Set(byte nowMode, byte lastMode, int modeTime)
        {
            NowMode = nowMode;
            LastMode = lastMode;
            ModeTime = modeTime;
        }
    }

    public interface ICameraMotorState
    {
        Dictionary<int, SubCameraMotorState> Dict { get; set; }
        SubCameraMotorState Get(SubCameraMotorType type);
        float FreeYaw { get; set; }
        float FreePitch { get; set; }
        float LastFreeYaw { get; set; }
        float LastFreePitch { get; set; }
        float LastPeekPercent { get; set; }
        SubCameraMotorState GetMainMotor();
        CameraConfigItem GetMainConfig();
        bool IsFree();
        bool IsFristPersion();
        ECameraViewMode ViewMode { get; }
        ECameraFreeMode FreeMode { get; }
        ECameraPeekMode PeekMode { get; }
        ECameraViewMode LastViewMode { get; }
    }
}