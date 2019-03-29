using System;
using System.Collections.Generic;
using Core.BulletSimulation;
using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using WeaponConfigNs;

// ReSharper disable PossibleNullReferenceException
namespace App.Shared.Components.Bullet
{
    [Bullet]
    public class BulletEntityAdapterComponent : IComponent
    {
        public IBulletEntity Adapter;
    }

    [Bullet]
    [Serializable]
    
    public class BulletDataComponent : IPlaybackComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.BulletData; } }

        [NetworkProperty] public Vector3 Velocity;
        [NetworkProperty] public float Distance;

        public float Gravity;
        public int RemainFrameTime;
        public int ServerTime;
        public float MaxDistance;
        public int PenetrableLayerCount;
        public float BaseDamage;
        public float PenetrableThickness;
        public BulletConfig DefaultBulletConfig;
        public float VelocityDecay;
        public EBulletCaliber Caliber;
        public int WeaponId;
        [DontInitilize] public Vector3 StartPoint;
        [DontInitilize] public Vector3 EmitPoint;
        [DontInitilize] public Vector3 StartDir;
        [DontInitilize] public Vector3 HitPoint;
        [DontInitilize] public EHitType HitType;
        [DontInitilize] public int CmdSeq;

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as BulletDataComponent;
            Distance = r.Distance;
            Velocity = r.Velocity;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = left as BulletDataComponent;
            var r = right as BulletDataComponent;
            Distance = InterpolateUtility.Interpolate(l.Distance, r.Distance, interpolationInfo);
            Velocity = r.Velocity;
        }
    }

    [Bullet]
    [Serializable]
    
    public class BulletGameObjectComponent : SingleAssetComponent 
    {
        public override int GetComponentId() { { return (int)EComponentIds.BulletGameObject; } }
    }

    [Bullet, UniquePrefix("is")]
    public class NewComponent : IComponent
    {

    }

    [Bullet]
    public class EmitPositionComponent : IComponent
    {
        public Vector3 Value; 
    }

    [Bullet]
    public class PenetrateInfoComponent : IComponent
    {
        public List<EEnvironmentType> Layers;
    }
}