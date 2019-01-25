using System;
using Assets.XmlConfig;
using Core.Components;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.Components.Throwing
{

    [Throwing]
    [Serializable]
    
    public class ThrowingDataComponent : IPlaybackComponent 
    {
        public int GetComponentId() { { return (int)EComponentIds.ThrowingData; } }

        [NetworkProperty] public Vector3 Velocity;

        [NetworkProperty] public bool IsThrow;

        [NetworkProperty] public bool IsFly;

        public int RemainFrameTime;
        public int ServerTime;
        public bool IsInWater;

        public float InitVelocity;
        public ThrowingConfig Config;
        [NetworkProperty] public int WeaponSubType;

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as ThrowingDataComponent;
            Velocity = r.Velocity;
            IsThrow = r.IsThrow;
            IsFly = r.IsFly;
            WeaponSubType = r.WeaponSubType;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    [Throwing]
    [Serializable]
    
    public class ThrowingGameObjectComponent : SingleAssetComponent
    {
        public override int GetComponentId() { { return (int)EComponentIds.ThrowingGameObject; } }
    }
    
}