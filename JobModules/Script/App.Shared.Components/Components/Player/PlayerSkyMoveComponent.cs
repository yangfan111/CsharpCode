using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Compare;
using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Player
{
    
    [Player]
    [Serializable]
    
    public class PlayerSkyMoveComponent : IPlaybackComponent, IUpdateComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.PlayerSkyMove; } }
        
        [NetworkProperty]
        public bool IsMoveEnabled;

        [DontInitilize]
        public bool IsMoving;

        [DontInitilize]
        [NetworkProperty(SyncFieldScale.stage)]
        public int MoveStage;
        
        [NetworkProperty]
        public int GameState;

        [DontInitilize]
        public float RemainDeltaTime;

        [DontInitilize]
        public float MotionLessTime;

        [DontInitilize]
        public float ParachuteTime;

        [DontInitilize]
        [NetworkProperty(SyncFieldScale.Position)]
        public FixedVector3 Position;

        [DontInitilize]
        [NetworkProperty]
        public Quaternion Rotation;

        [DontInitilize]
        [NetworkProperty(SyncFieldScale.Position)]
        public Vector3 LocalPlayerPosition;

        [DontInitilize]
        [NetworkProperty]
        public Quaternion LocalPlayerRotation;

        [DontInitilize]
        public float SwingVelocity;

        [DontInitilize]
        public float ExtraVerticalVelocity;
        
        [DontInitilize]
        [NetworkProperty]
        public Vector3 Velocity;
        
        [DontInitilize]
        public bool IsWaitForAttach;
        [DontInitilize]
        public bool IsParachuteAttached;
        [DontInitilize]
        public bool IsParachuteLoading;
        [DontInitilize]
        public Transform Parachute;
        [DontInitilize]
        public Transform ParachuteAnchor;

        public override string ToString()
        {
            return string.Format("IsMoveEnabled: {0}, IsMoving: {1}, MoveStage: {2}, GameState: {3}, MotionLessTime: {4}, ParachuteTime: {5}, Position: {6}, Rotation: {7}, LocalPlayerPosition: {8}, LocalPlayerRotation: {9}, SwingVelocity: {10}, ExtraVerticalVelocity: {11}, Velocity: {12}", IsMoveEnabled, IsMoving, MoveStage, GameState, MotionLessTime, ParachuteTime, Position, Rotation, LocalPlayerPosition, LocalPlayerRotation, SwingVelocity, ExtraVerticalVelocity, Velocity);
        }

        public bool IsReadyForAttachParachute()
        {
            return Parachute != null && !IsWaitForAttach && !IsParachuteAttached;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as PlayerSkyMoveComponent;

            IsMoving = r.IsMoving;
            IsMoveEnabled = r.IsMoveEnabled;
            MoveStage = r.MoveStage;

            Position = r.Position;
            Rotation = r.Rotation;

            LocalPlayerPosition = r.LocalPlayerPosition;
            LocalPlayerRotation = r.LocalPlayerRotation;

            GameState = r.GameState;
            Velocity = r.Velocity;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = left as PlayerSkyMoveComponent;
            var r = right as PlayerSkyMoveComponent;
            var ratio = interpolationInfo.Ratio;
            IsMoveEnabled = l.IsMoveEnabled;
            IsMoving = l.IsMoving;
            MoveStage = l.MoveStage;
            RemainDeltaTime = l.RemainDeltaTime;
            ParachuteTime = InterpolateUtility.Interpolate(l.ParachuteTime, r.ParachuteTime, ratio);
            MotionLessTime = InterpolateUtility.Interpolate(l.MotionLessTime, r.MotionLessTime, ratio);
            SwingVelocity = InterpolateUtility.Interpolate(l.SwingVelocity, r.SwingVelocity, ratio);
            ExtraVerticalVelocity =
                InterpolateUtility.Interpolate(l.ExtraVerticalVelocity, r.ExtraVerticalVelocity, ratio);
            Position = InterpolateUtility.Interpolate(l.Position, r.Position, ratio);
            Rotation = InterpolateUtility.Interpolate(l.Rotation, r.Rotation, ratio);

            LocalPlayerPosition = l.LocalPlayerPosition;
            LocalPlayerRotation = l.LocalPlayerRotation;
            GameState = l.GameState;
            Velocity = r.Velocity;
        }
        
        public void SyncFrom(PlayerSkyMoveInterVarComponent r)
        {
            ParachuteTime = r.ParachuteTime;
            Position = r.Position;
            Rotation = r.Rotation;
            SwingVelocity = r.SwingVelocity;
            ExtraVerticalVelocity = r.ExtraVerticalVelocity;
            Velocity = r.Velocity;
        }
    }

    [Player]
    
    public class PlayerSkyMoveInterVarComponent : IUpdateComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.PlayerSkyMoveInterVar; } }
        
        [DontInitilize]
        [NetworkProperty]
        public float ParachuteTime;

        [DontInitilize]
        [NetworkProperty]
        public FixedVector3 Position;

        [DontInitilize]
        [NetworkProperty]
        public Quaternion Rotation;

        [DontInitilize]
        [NetworkProperty]
        public float SwingVelocity;

        [DontInitilize]
        [NetworkProperty]
        public float ExtraVerticalVelocity;
        
        [DontInitilize]
        [NetworkProperty]
        public Vector3 Velocity;
        

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as PlayerSkyMoveInterVarComponent;
            ParachuteTime = r.ParachuteTime;
            Position = r.Position;
            Rotation = r.Rotation;
            SwingVelocity = r.SwingVelocity;
            ExtraVerticalVelocity = r.ExtraVerticalVelocity;
            
            Velocity = r.Velocity;
        }

        public void SyncFrom(PlayerSkyMoveComponent r)
        {
            ParachuteTime = r.ParachuteTime;
            Position = r.Position;
            Rotation = r.Rotation;
            SwingVelocity = r.SwingVelocity;
            ExtraVerticalVelocity = r.ExtraVerticalVelocity;
            Velocity = r.Velocity;
        }
    }
}
