using System;
using App.Shared.PlayerAutoMove;
using Core.Compare;
using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.UpdateLatest;

using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using Core.Utils;
using Core.CharacterController;
// ReSharper disable PossibleNullReferenceException
namespace App.Shared.Components.Player
{
    [Player]
    public class FlagAutoMoveComponent : IComponent
    {

    }

    [Player]
    [Serializable]
    
    public class PlayerMoveByAnimUpdateComponent : IUpdateComponent
    {
        [NetworkProperty(SyncFieldScale.Position)] [DontInitilize] public FixedVector3 Position;
        [NetworkProperty(SyncFieldScale.Pitch)] [DontInitilize] public float ModelPitch;
        [NetworkProperty(SyncFieldScale.Yaw)] [DontInitilize] public float ModelYaw;
        [NetworkProperty] [DontInitilize] public bool NeedUpdate;

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerMoveByAnimUpdate;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as PlayerMoveByAnimUpdateComponent;
            Position = r.Position;
            ModelPitch = r.ModelPitch;
            ModelYaw = r.ModelYaw;
            NeedUpdate = r.NeedUpdate;
        }

    }

    [Player]
    [Serializable]
    
    public class SkyMoveUpdateComponent : IUpdateComponent
    {
        [NetworkProperty] [DontInitilize] public bool IsMoveEnabled;

        [NetworkProperty(SyncFieldScale.Pitch)] [DontInitilize] public float Pitch;
        [NetworkProperty(SyncFieldScale.Yaw)] [DontInitilize] public float Yaw;
        [NetworkProperty(SyncFieldScale.Roll)] [DontInitilize] public float Roll;

        [NetworkProperty(SyncFieldScale.stage)] [DontInitilize] public int SkyMoveStage;
        [NetworkProperty(SyncFieldScale.stage)] [DontInitilize] public int GameState;
        [NetworkProperty(SyncFieldScale.Position)] [DontInitilize] public FixedVector3 SkyPosition;
        [NetworkProperty(SyncFieldScale.Quaternion)] [DontInitilize] public Quaternion SkyRotation;
        [NetworkProperty(SyncFieldScale.Position)] [DontInitilize] public Vector3 SkyLocalPlayerPosition;
        [NetworkProperty(SyncFieldScale.Quaternion)] [DontInitilize] public Quaternion SkyLocalPlayerRotation;

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerSkyMoveUpdate;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as SkyMoveUpdateComponent;

            IsMoveEnabled = r.IsMoveEnabled;
            SkyMoveStage = r.SkyMoveStage;
            SkyPosition = r.SkyPosition;
            SkyRotation = r.SkyRotation;
            SkyLocalPlayerPosition = r.SkyLocalPlayerPosition;
            SkyLocalPlayerRotation = r.SkyLocalPlayerRotation;
            Pitch = r.Pitch;
            Yaw = r.Yaw;
            Roll = r.Roll;
            GameState = r.GameState;
        }
    }

    [Player]
    [Serializable]
    
    public class MoveUpdateComponent : IUpdateComponent
    {
        [NetworkProperty(100,-100,0.01f)] [DontInitilize] public Vector3 Velocity;
        [NetworkProperty] [DontInitilize] public FixedVector3 LastPosition;
        [NetworkProperty(SyncFieldScale.EularAngle)] [DontInitilize] public Vector3 Rotation;
        [NetworkProperty] [DontInitilize] public Vector3 Dist;
        [NetworkProperty] [DontInitilize] public Vector3 VehicleRideOffOffset;
        
        [NetworkProperty] [DontInitilize] public bool IsGround;
        [NetworkProperty] [DontInitilize] public bool IsCollided;
        [NetworkProperty(90,-90,0.01f)] [DontInitilize] public float TanSteepAngle; //因坡度过大，人物减速
        [NetworkProperty] [DontInitilize] public bool MoveInWater; //涉水状态
        [NetworkProperty] [DontInitilize] public bool BeginDive; //由游泳状态转为潜水状态
        [NetworkProperty] [DontInitilize] public bool NeedUpdate;

        [NetworkProperty] [DontInitilize] public float SpeedRatio;
        [NetworkProperty] [DontInitilize] public float MoveSpeedRatio;
        [NetworkProperty(SyncFieldScale.Pitch)] [DontInitilize] public float ModelPitch;
        [NetworkProperty(SyncFieldScale.Yaw)] [DontInitilize] public float ModelYaw;
        [NetworkProperty(SyncFieldScale.stage)] [DontInitilize] public int MoveType; //移动场景（陆地、空中、水）

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as MoveUpdateComponent;

            Velocity = r.Velocity;
            Rotation = r.Rotation;
            SpeedRatio = r.SpeedRatio;
            MoveSpeedRatio = r.MoveSpeedRatio;
            LastPosition = r.LastPosition;
            IsGround = r.IsGround;
            IsCollided = r.IsCollided;
            TanSteepAngle = r.TanSteepAngle;
            MoveInWater = r.MoveInWater;
            BeginDive = r.BeginDive;
            NeedUpdate = r.NeedUpdate;
            ModelPitch = r.ModelPitch;
            ModelYaw = r.ModelYaw;
            MoveType = r.MoveType;
            Dist = r.Dist;
            VehicleRideOffOffset = r.VehicleRideOffOffset;
        }

        public void Reset()
        {
            BeginDive = false;
            NeedUpdate = false;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerMoveUpdate;
        }
    }


    [Player]
    [Serializable]

    public class PlayerMoveComponent : IComponent
    {

        public Vector3 Velocity;
        public float SpeedAffect;
        public float JumpAffect;
        public bool IsGround;
        public bool IsAutoRun;
        public float SpeedRatio;
        [DontInitilize] public float UpDownValue;
        [DontInitilize] public float MoveSpeedRatio;

        [DontInitilize] public float AirTime;
        [DontInitilize] public float Steep;
        [DontInitilize] public float SteepAverage;
        [DontInitilize] public float MoveVel;
        [DontInitilize] public bool IsCollided;
        [DontInitilize] public bool FirstOnGround;
        [DontInitilize] public bool LastIsCollided;

        [DontInitilize] public Vector3 LastVelocity;

        public override string ToString()
        {
            return string.Format(
                "Velocity: {0}, SpeedAffect: {1}, IsGround: {2}, IsAutoRun: {3}, SpeedRatio: {4}, Steep: {5}, SteepAverage: {6}, UpDownValue: {7}, MoveSpeedRatio:{8}, MoveVel:{9}, JumpAffect: {10}",
                Velocity, SpeedAffect, IsGround, IsAutoRun, SpeedRatio, Steep, SteepAverage, UpDownValue,
                MoveSpeedRatio, MoveVel, JumpAffect);
        }

        public void Reset()
        {
            Velocity = Vector3.zero;
            SpeedAffect = 0f;
            JumpAffect = 0f;
            IsGround = true;
            IsAutoRun = false;
            SpeedRatio = 1.0f;
            AirTime = 0f;
            Steep = 0f;
            SteepAverage = 0f;
            MoveVel = 0f;
            IsCollided = false;
            
        }

        public void ClearState()
        {
            IsCollided = false;
            IsGround = false;

        }

        [DontInitilize]
        public float HorizontalVelocity
        {
            get { return Mathf.Sqrt(Velocity.x * Velocity.x + Velocity.z * Velocity.z); }
        }

    }

    [Player]
    public class AutoMoveInterfaceComponent : IComponent
    {
        public IPlayerAutoMove PlayerAutoMove;
    }
}



