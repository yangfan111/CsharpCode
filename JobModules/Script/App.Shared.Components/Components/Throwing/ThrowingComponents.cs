using System;
using Core.Components;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.Components.Throwing
{
    [Throwing]
    [Serializable]
    public class ThrowingDataComponent : IPlaybackComponent
    {
        public ThrowingConfig ThrowConfig;
        public WeaponResConfigItem WeaonConfig;
        public float InitVelocity;

        [DontInitilize, NetworkProperty] public bool IsFly;
        [DontInitilize]public bool IsInWater;

        [DontInitilize,NetworkProperty] public bool IsThrow;

        public int RemainFrameTime;
        public int ServerTime;


        [NetworkProperty] public Vector3 Velocity;
        [NetworkProperty] public int WeaponSubType;

        public int GetComponentId()
        {
            {
                return (int) EComponentIds.ThrowingData;
            }
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as ThrowingDataComponent;
            Velocity      = r.Velocity;
            IsThrow       = r.IsThrow;
            IsFly         = r.IsFly;
            WeaponSubType = r.WeaponSubType;
        }

        public bool IsInterpolateEveryFrame()
        {
            return false;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    [Throwing]
    [Serializable]
    public class ThrowingGameObjectComponent : SingleAssetComponent
    {
        public override int GetComponentId()
        {
            {
                return (int) EComponentIds.ThrowingGameObject;
            }
        }
    }
}