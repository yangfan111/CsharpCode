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
    
    public class PlayerMoveByAnimUpdateComponent : IUpdateComponent, IUserPredictionComponent
    {
        [NetworkProperty] [DontInitilize] public Vector3 Position;
        [NetworkProperty] [DontInitilize] public float ModelPitch;
        [NetworkProperty] [DontInitilize] public float ModelYaw;
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

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as PlayerMoveByAnimUpdateComponent;
            return Position.Equals(r.Position) &&
                   NeedUpdate == r.NeedUpdate &&
                   CompareUtility.IsApproximatelyEqual(ModelPitch, r.ModelPitch) &&
                   CompareUtility.IsApproximatelyEqual(ModelYaw, r.ModelYaw);
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    [Serializable]
    
    public class SkyMoveUpdateComponent : IUpdateComponent, IUserPredictionComponent
    {
        [NetworkProperty] [DontInitilize] public bool IsMoveEnabled;

        [NetworkProperty] [DontInitilize] public float Pitch;
        [NetworkProperty] [DontInitilize] public float Yaw;
        [NetworkProperty] [DontInitilize] public float Roll;

        [NetworkProperty] [DontInitilize] public int SkyMoveStage;
        [NetworkProperty] [DontInitilize] public int GameState;
        [NetworkProperty] [DontInitilize] public Vector3 SkyPosition;
        [NetworkProperty] [DontInitilize] public Quaternion SkyRotation;
        [NetworkProperty] [DontInitilize] public Vector3 SkyLocalPlayerPosition;
        [NetworkProperty] [DontInitilize] public Quaternion SkyLocalPlayerRotation;

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerSkyMoveUpdate;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as SkyMoveUpdateComponent;
            return SkyMoveStage == r.SkyMoveStage &&
                   IsMoveEnabled == r.IsMoveEnabled &&
                   SkyPosition.Equals(r.SkyPosition) &&
                   SkyRotation.Equals(r.SkyRotation) &&
                   SkyLocalPlayerPosition.Equals(r.SkyLocalPlayerPosition) &&
                   SkyLocalPlayerRotation.Equals(r.SkyLocalPlayerRotation) &&
                   CompareUtility.IsApproximatelyEqual(Pitch, r.Pitch) &&
                   CompareUtility.IsApproximatelyEqual(Yaw, r.Yaw) &&
                   CompareUtility.IsApproximatelyEqual(Roll, r.Roll) &&
                   GameState == r.GameState;
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
        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    [Serializable]
    
    public class MoveUpdateComponent : IUpdateComponent, IUserPredictionComponent
    {
        [NetworkProperty] [DontInitilize] public Vector3 Velocity;
        [NetworkProperty] [DontInitilize] public Vector3 LastPosition;
        [NetworkProperty] [DontInitilize] public Vector3 Rotation;
        [NetworkProperty] [DontInitilize] public Vector3 Dist;
        [NetworkProperty] [DontInitilize] public Vector3 VehicleRideOffOffset;
        
        [NetworkProperty] [DontInitilize] public bool IsGround;
        [NetworkProperty] [DontInitilize] public bool IsCollided;
        [NetworkProperty] [DontInitilize] public bool ExceedSteepLimit; //因坡度过大，人物减速
        [NetworkProperty] [DontInitilize] public bool MoveInWater; //涉水状态
        [NetworkProperty] [DontInitilize] public bool BeginDive; //由游泳状态转为潜水状态
        [NetworkProperty] [DontInitilize] public bool NeedUpdate;

        [NetworkProperty] [DontInitilize] public float SpeedRatio;
        [NetworkProperty] [DontInitilize] public float MoveSpeedRatio;
        [NetworkProperty] [DontInitilize] public float ModelPitch;
        [NetworkProperty] [DontInitilize] public float ModelYaw;
        [NetworkProperty] [DontInitilize] public int MoveType; //移动场景（陆地、空中、水）


        public bool IsApproximatelyEqual(object rightComponent)
        {
            var r = rightComponent as MoveUpdateComponent;
            return LastPosition.Equals(r.LastPosition) &&
                   Velocity.Equals(r.Velocity) &&
                   Rotation.Equals(r.Rotation) &&
                   Dist.Equals(r.Dist) &&
                   IsGround == r.IsGround &&
                   IsCollided == r.IsCollided &&
                   ExceedSteepLimit == r.ExceedSteepLimit &&
                   MoveInWater == r.MoveInWater &&
                   BeginDive == r.BeginDive &&
                   NeedUpdate == r.NeedUpdate &&
                   MoveType == r.MoveType &&
                   CompareUtility.IsApproximatelyEqual(SpeedRatio, r.SpeedRatio) &&
                   CompareUtility.IsApproximatelyEqual(MoveSpeedRatio, r.MoveSpeedRatio) &&
                   CompareUtility.IsApproximatelyEqual(ModelPitch, r.ModelPitch) &&
                   CompareUtility.IsApproximatelyEqual(ModelYaw, r.ModelYaw);
        }

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
            ExceedSteepLimit = r.ExceedSteepLimit;
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
        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }


    [Player]
    [Serializable]
    
    public class PlayerMoveComponent : IUserPredictionComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerMoveComponent));

        public int GetComponentId()
        {
            {
                return (int) EComponentIds.PlayerMove;
            }
        }

        [NetworkProperty] public Vector3 Velocity;
        [NetworkProperty] public float SpeedAffect;
        [NetworkProperty] public bool IsGround;
		public bool IsAutoRun;
        [NetworkProperty] public float SpeedRatio;
        [NetworkProperty] [DontInitilize] public float UpDownValue;
        [NetworkProperty] [DontInitilize] public float MoveSpeedRatio;
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
                "Velocity: {0}, SpeedAffect: {1}, IsGround: {2}, IsAutoRun: {3}, SpeedRatio: {4}, Steep: {5}, SteepAverage: {6}, UpDownValue: {7}, MoveSpeedRatio:{8}, MoveVel:{9}",
                Velocity, SpeedAffect, IsGround, IsAutoRun, SpeedRatio, Steep, SteepAverage, UpDownValue, MoveSpeedRatio, MoveVel);
        }

        public void ClearState()
        {
            IsCollided = false;
            IsGround = false;
        }
        
        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        [DontInitilize]
        public float HorizontalVelocity
        {
            get { return Mathf.Sqrt(Velocity.x * Velocity.x + Velocity.z * Velocity.z); }
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as PlayerMoveComponent;
            return
                CompareUtility.IsApproximatelyEqual(Velocity, r.Velocity)
                && CompareUtility.IsApproximatelyEqual(SpeedAffect, r.SpeedAffect)
                && CompareUtility.IsApproximatelyEqual(UpDownValue, r.UpDownValue)
                && CompareUtility.IsApproximatelyEqual(MoveSpeedRatio, r.MoveSpeedRatio)
                ;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as PlayerMoveComponent;
            Velocity = r.Velocity;
            SpeedAffect = r.SpeedAffect;
            SpeedRatio = r.SpeedRatio;
            IsGround = r.IsGround;
            Steep = r.Steep;
            SteepAverage = r.SteepAverage;
            MoveVel = r.MoveVel;
            UpDownValue = r.UpDownValue;
            MoveSpeedRatio = r.MoveSpeedRatio;
        }
    }
    
    [Player]
    public class AutoMoveInterfaceComponent : IComponent
    {
        public IPlayerAutoMove PlayerAutoMove;
    }
}



