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
using Core.CameraControl;
using Core.Interpolate;
using Core.SyncLatest;

namespace App.Shared.Components.Player
{
    [Player]
    [Serializable]
    public class CameraStateNewComponent : IComponent
    {
        [DontInitilize] public byte MainNowMode;
        [DontInitilize] public byte MainLastMode;
        [DontInitilize] public int MainModeTime;

        [DontInitilize] public byte ViewNowMode;
        [DontInitilize] public byte ViewLastMode; 
        [DontInitilize] public int ViewModeTime;

        [DontInitilize] public byte PeekNowMode;
        [DontInitilize] public byte PeekLastMode;
        [DontInitilize] public int PeekModeTime;

        [DontInitilize] public byte FreeNowMode;
        [DontInitilize] public byte FreeLastMode;
        [DontInitilize] public int FreeModeTime;

        [DontInitilize] public float FreeYaw;
        [DontInitilize] public float FreePitch;

        [DontInitilize] public float LastFreeYaw;
        [DontInitilize] public float LastFreePitch;
        [DontInitilize] public float LastPeekPercent;
        [DontInitilize] public bool CanFire;
        
        [DontInitilize] public ICameraMotorInput LastCameraMotorInput;
        [DontInitilize] public ICameraMotorInput CameraMotorInput;

        public override string ToString()
        {
            return string.Format(
                "MainNowMode: {0}, MainLastMode: {1}, MainModeTime: {2}, PeekNowMode: {3}, PeekLastMode: {4}, PeekModeTime: {5}, FreeNowMode: {6}, FreeLastMode: {7}, FreeModeTime: {8}, FreeYaw: {9}, FreePitch: {10}, CanFiew{11}",
                MainNowMode, MainLastMode, MainModeTime, PeekNowMode, PeekLastMode, PeekModeTime, FreeNowMode,
                FreeLastMode, FreeModeTime, FreeYaw, FreePitch, CanFire);
        }
    }
    
    [Player]
    [Serializable]
    public class CameraConfigNow : IComponent
    {
        [DontInitilize] public PoseCameraConfig Config;
        [DontInitilize] public PeekCameraConfig PeekConfig;
        [DontInitilize] public DeadCameraConfig DeadConfig;
    }
    
    public enum ECameraArchorType{
        Third,
        AirPlane,
        Car,
        Parachuting,
        FollowEntity,
        Witness, // 自由视角
        Default
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
        [DontInitilize] public bool Active;
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
        [DontInitilize] public bool NeedDetectDistance { get; set; }
        [DontInitilize] public float LastPitchWhenAlive { get; set; }
    }

    [Player]
    [Serializable]
    public class CameraFireInfo : IUpdateComponent
    {
        [NetworkProperty][DontInitilize()] public Vector3 PlayerFocusPosition;
        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as CameraFireInfo;
            PlayerFocusPosition = r.PlayerFocusPosition;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.CameraFireInfo;
        }
    }
    
    [Player]
    [Serializable]
    public class CameraFinalOutputNewComponent : IComponent
    {
      
        [DontInitilize] public Vector3 Position;
        [DontInitilize] public Vector3 PlayerFocusPosition;
        [DontInitilize] public Vector3 EulerAngle;
        [DontInitilize] public float Fov;
        [DontInitilize] public float Far;
        [DontInitilize] public float Near;

        public override string ToString()
        {
            return string.Format("Position: {0}, EulerAngle: {1}, Fov: {2}, Far: {3}, Near: {4}", Position.ToStringExt(), EulerAngle.ToStringExt(), Fov, Far, Near);
        }
    }
    
    [Player]
    [Serializable]
    public class ObserveCameraComponent : ISelfLatestComponent
    {
        [DontInitilize][NetworkProperty] public byte MainNowMode;
        [DontInitilize][NetworkProperty] public byte PeekNowMode;
        [DontInitilize][NetworkProperty] public byte FreeNowMode;
        
        [DontInitilize][NetworkProperty(SyncFieldScale.Yaw)] public float FreeYaw;
        [DontInitilize][NetworkProperty(SyncFieldScale.Pitch)] public float FreePitch;

        [DontInitilize][NetworkProperty] public int VehicleId;

        [DontInitilize] public Vector3 ObservedPlayerPosition;
        
        [DontInitilize] public PlayerEntity ObservedPlayer;
        [DontInitilize] public VehicleEntity ControllVehicle;
        
        public int GetComponentId()
        {
            return (int) EComponentIds.ObserveCamera;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as ObserveCameraComponent;

            MainNowMode = r.MainNowMode;
            PeekNowMode = r.PeekNowMode;
            FreeNowMode = r.FreeNowMode;

            FreeYaw = r.FreeYaw;
            FreePitch = r.FreePitch;

            VehicleId = r.VehicleId;
        }
        
        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
    
    [Player]
    [Serializable]
    public class CameraStateUploadComponent: IUpdateComponent
    {
        [DontInitilize] [NetworkProperty] public Vector3 Position;
        [DontInitilize] [NetworkProperty(SyncFieldScale.EularAngle)] public Vector3 EulerAngle;
        [DontInitilize] [NetworkProperty(180,0,0.01f)] public float Fov;
        [DontInitilize] [NetworkProperty(10000,0,1f)] public float Far;
        [DontInitilize] [NetworkProperty(1,0,0.001f)] public float Near;

        [DontInitilize] [NetworkProperty] public byte MainNowMode;
        [DontInitilize] [NetworkProperty] public byte ViewNowMode;
        [DontInitilize] [NetworkProperty] public byte PeekNowMode;
        [DontInitilize] [NetworkProperty] public byte FreeNowMode;
        [DontInitilize] [NetworkProperty(SyncFieldScale.Yaw)] public float FreeYaw;
        [DontInitilize] [NetworkProperty(SyncFieldScale.Pitch)] public float FreePitch;

        [DontInitilize] [NetworkProperty] public bool CanFire;
        [DontInitilize] [NetworkProperty] public int EnterActionCode;
        [DontInitilize] [NetworkProperty] public int LeaveActionCode;
        
        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as CameraStateUploadComponent;

            Position = r.Position;
            EulerAngle = r.EulerAngle;
            Fov = r.Fov;
            Far = r.Far;
            Near = r.Near;

            MainNowMode = r.MainNowMode;
            ViewNowMode = r.ViewNowMode;
            PeekNowMode = r.PeekNowMode;
            FreeNowMode = r.FreeNowMode;
            FreeYaw = r.FreeYaw;
            FreePitch = r.FreePitch;
            CanFire = r.CanFire;

            EnterActionCode = r.EnterActionCode;
            LeaveActionCode = r.LeaveActionCode;

        }

        public int GetComponentId()
        {
            return (int) EComponentIds.CameraStateUpload;
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