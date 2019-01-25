using System;
using System.Linq.Expressions;
using Core.CameraControl.NewMotor;
using Core.Compare;
using Core.Compensation;
using Core.Components;
using Core.Playback;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using Utils.Utils;
using XmlConfig;
using System.Collections.Specialized;

namespace App.Shared.Components.Player
{
    [Player]
    [Serializable]
    public class CameraStateNewComponent : IUserPredictionComponent
    {
        [DontInitilize] [NetworkProperty] public byte MainNowMode;
        [DontInitilize] [NetworkProperty] public byte MainLastMode;
        [DontInitilize] [NetworkProperty] public int MainModeTime;

        [DontInitilize] [NetworkProperty] public byte ViewNowMode;
        [DontInitilize] [NetworkProperty] public byte ViewLastMode;
        [DontInitilize] [NetworkProperty] public int ViewModeTime;

        [DontInitilize] [NetworkProperty] public byte PeekNowMode;
        [DontInitilize] [NetworkProperty] public byte PeekLastMode;
        [DontInitilize] [NetworkProperty] public int PeekModeTime;

        [DontInitilize] [NetworkProperty] public byte FreeNowMode;
        [DontInitilize] [NetworkProperty] public byte FreeLastMode;
        [DontInitilize] [NetworkProperty] public int FreeModeTime;

        [DontInitilize] [NetworkProperty] public float FreeYaw;
        [DontInitilize] [NetworkProperty] public float FreePitch;

        [DontInitilize] [NetworkProperty] public float LastFreeYaw;
        [DontInitilize] [NetworkProperty] public float LastFreePitch;
        [DontInitilize] [NetworkProperty] public float LastPeekPercent;
        [DontInitilize] [NetworkProperty] public bool CanFire;
        
        [DontInitilize] public ICameraMotorInput LastCameraMotorInput;
        [DontInitilize] public ICameraMotorInput CameraMotorInput;


        public int GetComponentId()
        {
            {
                return (int) EComponentIds.CameraStateNew;
            }
        }

        public void CopyFrom(object rightComponent)
        {
            var r = ((CameraStateNewComponent) rightComponent);
            MainNowMode = r.MainNowMode;
            MainLastMode = r.MainLastMode;
            MainModeTime = r.MainModeTime;
            ViewNowMode = r.ViewNowMode;
            ViewLastMode = r.ViewLastMode;
            ViewModeTime = r.ViewModeTime;
            PeekNowMode = r.PeekNowMode;
            PeekLastMode = r.PeekLastMode;
            PeekModeTime = r.PeekModeTime;
            FreeNowMode = r.FreeNowMode;
            FreeLastMode = r.FreeLastMode;
            FreeModeTime = r.FreeModeTime;
            FreeYaw = r.FreeYaw;
            FreePitch = r.FreePitch;
            LastFreeYaw = r.LastFreeYaw;
            LastFreePitch = r.LastFreePitch;
            LastPeekPercent = r.LastPeekPercent;
            CanFire = r.CanFire;
           
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = (CameraStateNewComponent) right;

            return MainNowMode == r.MainNowMode
                   && MainLastMode == r.MainLastMode
                   && MainModeTime == r.MainModeTime
                   && ViewNowMode == r.ViewNowMode
                   && ViewLastMode == r.ViewLastMode
                   && ViewModeTime == r.ViewModeTime
                   && PeekNowMode == r.PeekNowMode
                   && PeekLastMode == r.PeekLastMode
                   && PeekModeTime == r.PeekModeTime
                   && FreeNowMode == r.FreeNowMode
                   && FreeLastMode == r.FreeLastMode
                   && FreeModeTime == r.FreeModeTime
                   && CompareUtility.IsApproximatelyEqual(FreeYaw, r.FreeYaw)
                   && CompareUtility.IsApproximatelyEqual(FreePitch, r.FreePitch)
                   && CanFire == r.CanFire;
        }

        public override string ToString()
        {
            return string.Format(
                "MainNowMode: {0}, MainLastMode: {1}, MainModeTime: {2}, PeekNowMode: {3}, PeekLastMode: {4}, PeekModeTime: {5}, FreeNowMode: {6}, FreeLastMode: {7}, FreeModeTime: {8}, FreeYaw: {9}, FreePitch: {10}, CanFiew{11}",
                MainNowMode, MainLastMode, MainModeTime, PeekNowMode, PeekLastMode, PeekModeTime, FreeNowMode,
                FreeLastMode, FreeModeTime, FreeYaw, FreePitch, CanFire);
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
    
    [Player]
    [Serializable]
    public class CameraConfigNow : IComponent
    {
        [DontInitilize] public CameraConfigItem Config;
        [DontInitilize] public PeekCameraConfig PeekConfig;
        [DontInitilize] public DeadCameraConfig DeadConfig;
    }
    public enum ECameraArchorType{
        Third,
        AirPlane,
        Car,
        Parachuting,
        FollowEntity
    
    }
    public interface ICameraMotorOutput
    {
      
        Vector3 ArchorPosition { get; set; }
        Vector3 ArchorEulerAngle { get; set; }
        Vector3 ArchorOffset { get; set; }
        Vector3 ArchorPostOffset { get; set; }
        Vector3 EulerAngle { get; set; }
        Vector3 Offset { get; set; }
        Vector3 PostOffset { get; set; }
        float Fov { get; set; }
        float FreeYaw { get; set; }
        float FreePitch { get; set; }
        float Far { get; set; }
        float Near { get; set; }
    }


    [Player]
    [Serializable]
    public class CameraArchorComponent : IComponent
    {
        [DontInitilize] public ECameraArchorType ArchorType;
        [DontInitilize] public Vector3 ArchorPosition;
        [DontInitilize] public Vector3 ArchorEulerAngle;
        [DontInitilize] public ECameraArchorType LastArchorType;
        [DontInitilize] public Vector3 ArchorTransitionPosition;
        [DontInitilize] public Vector3 ArchorTransitionOffsetPosition;
        [DontInitilize] public int EnterTime;
    }

    [Player]
    [Serializable]
    public class CameraStateOutputNewComponent : IComponent, ICameraMotorOutput
    {
       
        [DontInitilize] public Vector3 ArchorPosition { get; set; }
        [DontInitilize] public Vector3 FinalArchorPosition { get; set; }
        [DontInitilize] public Vector3 ArchorEulerAngle { get; set; }
        [DontInitilize] public Vector3 ArchorOffset { get; set; }
        [DontInitilize] public Vector3 ArchorPostOffset { get; set; }
        [DontInitilize] public Vector3 EulerAngle { get; set; }
        [DontInitilize] public Vector3 Offset { get; set; }
        [DontInitilize] public Vector3 PostOffset { get; set; }
        [DontInitilize] public float Fov { get; set; }
        [DontInitilize] public float FreeYaw { get; set; }
        [DontInitilize] public float FreePitch { get; set; }
        [DontInitilize] public float Far { get; set; }
        [DontInitilize] public float Near { get; set; }
        
    }

    [Player]
    [Serializable]
    
    public class CameraFinalOutputNewComponent : IComponent
    {
      
        [NetworkProperty] [DontInitilize] public Vector3 Position;
        [NetworkProperty] [DontInitilize] public Vector3 PlayerFocusPosition;
        [NetworkProperty] [DontInitilize] public Vector3 EulerAngle;
        [NetworkProperty] [DontInitilize] public float Fov;
        [NetworkProperty] [DontInitilize] public float Far;
        [NetworkProperty] [DontInitilize] public float Near;


        public int GetComponentId()
        {
            return (int) EComponentIds.CameraOutput;
        }

        public void RewindTo(object rightComponent)
        {
            var r = ((CameraFinalOutputNewComponent) rightComponent);
          
            Position = r.Position;
            EulerAngle = r.EulerAngle;
            Fov = r.Fov;
            Far = r.Far;
            Near = r.Near;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = ((CameraFinalOutputNewComponent) right);
            return CompareUtility.IsApproximatelyEqual(Position, r.Position)
                   && CompareUtility.IsApproximatelyEqual(EulerAngle, r.EulerAngle)
                   && CompareUtility.IsApproximatelyEqual(Fov, r.Fov)
                   && CompareUtility.IsApproximatelyEqual(Far, r.Far)
                   && CompareUtility.IsApproximatelyEqual(Near, r.Near);

        }

        public override string ToString()
        {
            return string.Format("Position: {0}, EulerAngle: {1}, Fov: {2}, Far: {3}, Near: {4}", Position.ToStringExt(), EulerAngle.ToStringExt(), Fov, Far, Near);
        }
    }

    [Player]
    [Serializable]
    
    public class CameraStateUploadComponent: IUpdateComponent
    {
        [NetworkProperty] [DontInitilize] public Vector3 Position;
        [NetworkProperty] [DontInitilize] public Vector3 EulerAngle;
        [NetworkProperty] [DontInitilize] public Vector3 PlayerFocusPosition;
        [NetworkProperty] [DontInitilize] public float Fov;
        [NetworkProperty] [DontInitilize] public float Far;
        [NetworkProperty] [DontInitilize] public float Near;

        [DontInitilize] [NetworkProperty] public byte MainNowMode;
        [DontInitilize] [NetworkProperty] public byte MainLastMode;
        [DontInitilize] [NetworkProperty] public int MainModeTime;

        [DontInitilize] [NetworkProperty] public byte ViewNowMode;
        [DontInitilize] [NetworkProperty] public byte ViewLastMode;
        [DontInitilize] [NetworkProperty] public int ViewModeTime;

        [DontInitilize] [NetworkProperty] public byte PeekNowMode;
        [DontInitilize] [NetworkProperty] public byte PeekLastMode;
        [DontInitilize] [NetworkProperty] public int PeekModeTime;

        [DontInitilize] [NetworkProperty] public byte FreeNowMode;
        [DontInitilize] [NetworkProperty] public byte FreeLastMode;
        [DontInitilize] [NetworkProperty] public int FreeModeTime;

        [DontInitilize] [NetworkProperty] public float FreeYaw;
        [DontInitilize] [NetworkProperty] public float FreePitch;

        [DontInitilize] [NetworkProperty] public float LastFreeYaw;
        [DontInitilize] [NetworkProperty] public float LastFreePitch;
        [DontInitilize] [NetworkProperty] public float LastPeekPercent;
        [DontInitilize] [NetworkProperty] public bool CanFire;

        [DontInitilize] [NetworkProperty] public int EnterActionCode;
        [DontInitilize] [NetworkProperty] public int LeaveActionCode;

        [DontInitilize] [NetworkProperty] public Byte ArchorType;

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as CameraStateUploadComponent;

            Position = r.Position;
            EulerAngle = r.EulerAngle;
            Fov = r.Fov;
            Far = r.Far;
            Near = r.Near;

            MainNowMode = r.MainNowMode;
            MainLastMode = r.MainLastMode;
            MainModeTime = r.MainModeTime;
            ViewNowMode = r.ViewNowMode;
            ViewLastMode = r.ViewLastMode;
            ViewModeTime = r.ViewModeTime;
            PeekNowMode = r.PeekNowMode;
            PeekLastMode = r.PeekLastMode;
            PeekModeTime = r.PeekModeTime;
            FreeNowMode = r.FreeNowMode;
            FreeLastMode = r.FreeLastMode;
            FreeModeTime = r.FreeModeTime;
            FreeYaw = r.FreeYaw;
            FreePitch = r.FreePitch;
            LastFreeYaw = r.LastFreeYaw;
            LastFreePitch = r.LastFreePitch;
            LastPeekPercent = r.LastPeekPercent;
            CanFire = r.CanFire;

            EnterActionCode = r.EnterActionCode;
            LeaveActionCode = r.LeaveActionCode;
            PlayerFocusPosition = r.PlayerFocusPosition;

            ArchorType = r.ArchorType;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.CameraStateUpload;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as CameraStateUploadComponent;

            return
                Position == r.Position &&
                EulerAngle == r.EulerAngle &&
                Fov == r.Fov &&
                Far == r.Far &&
                Near == r.Near &&
                MainNowMode == r.MainNowMode &&
                MainLastMode == r.MainLastMode &&
                MainModeTime == r.MainModeTime &&
                ViewNowMode == r.ViewNowMode &&
                ViewLastMode == r.ViewLastMode &&
                ViewModeTime == r.ViewModeTime &&
                PeekNowMode == r.PeekNowMode &&
                PeekLastMode == r.PeekLastMode &&
                PeekModeTime == r.PeekModeTime &&
                FreeNowMode == r.FreeNowMode &&
                FreeLastMode == r.FreeLastMode &&
                FreeModeTime == r.FreeModeTime &&
                FreeYaw == r.FreeYaw &&
                FreePitch == r.FreePitch &&
                LastFreeYaw == r.LastFreeYaw &&
                LastFreePitch == r.LastFreePitch &&
                LastPeekPercent == r.LastPeekPercent &&
                CanFire == r.CanFire &&
                EnterActionCode == r.EnterActionCode &&
                LeaveActionCode == r.LeaveActionCode &&
                ArchorType == r.ArchorType &&
                PlayerFocusPosition == r.PlayerFocusPosition;
        }
    }


    [Player]
    [Unique]
    public class CameraObjComponent : IComponent
    {
        [DontInitilize] public Camera EffectCamera;
        [DontInitilize] public Camera FPCamera;
        [DontInitilize] public Camera MainCamera;
        [DontInitilize] public GameObject Hand;
    }

    [Player]
    public class CameraFxComponent : IComponent
    {
        [DontInitilize] public GameObject Poison;
    }
}