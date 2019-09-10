using Core.Attack;
using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Core;
using UnityEngine;
using WeaponConfigNs;

// ReSharper disable PossibleNullReferenceException
namespace App.Shared.Components.Bullet
{
    [Bullet]
    public class BulletRuntimeComponent : IComponent
    {
        [DontInitilize]public List<EEnvironmentType> Layers;
        public IBulletEntityAgent BulletAgent;
        public bool IsNew;

    }
    [Bullet]
    [Serializable]
    
    public class BulletDataComponent : IPlaybackComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.BulletData; } }

        [DontInitilize,NetworkProperty] public PrecisionsVector3 Velocity;
        [DontInitilize,NetworkProperty(SyncFieldScale.Position)] public float Distance;
        [DontInitilize] public bool IsAimShoot;
        [DontInitilize] public float Gravity;
        [DontInitilize] public int RemainFrameTime;
        [DontInitilize] public int ServerTime;
        [DontInitilize] public float MaxDistance;
        [DontInitilize] public int PenetrableLayerCount;
        [DontInitilize] public float BaseDamage;
            //穿透厚度
            [DontInitilize] public float PenetrableThickness;
        [DontInitilize] public BulletConfig DefaultBulletConfig;
        [DontInitilize] public float VelocityDecay;
        [DontInitilize] public EBulletCaliber Caliber;
        [DontInitilize] public int WeaponId;
        [DontInitilize] public float DistanceDecay;
        [DontInitilize] public PrecisionsVector3 StartPoint;
        [DontInitilize] public Vector3 EmitPoint;
        [DontInitilize] public PrecisionsVector3 StartDir;
        [DontInitilize] public PrecisionsVector3 HitPoint;
        [DontInitilize] public EHitType HitType;
        [DontInitilize] public int CmdSeq;
        [DontInitilize] public BulletStatisticsInfo StatisticsInfo;

        /*public override string ToString()
        {
            return string.Format("Velocity:{0},Distance:{1},Gravity:{2},RemainFrameTime:{3},ServerTime:{4},MaxDistance:{5},PenetrableLayerCount:{6},BaseDamage:{7},PenetrableThickness:{8},VelocityDecay:{9},Caliber:{10},WeaponId:{11},DistanceDecay:{12},StartPoint:{13},Ignore:{14},StartDir:{15},HitPoint:{16}",Velocity.ToString(), Distance.ToString("f2"),Gravity, RemainFrameTime, ServerTime, MaxDistance.ToString("f2"), PenetrableLayerCount, BaseDamage, PenetrableThickness, VelocityDecay.ToString("f2"),Caliber, WeaponId, DistanceDecay.ToString("f2"), StartPoint, 0, StartDir,HitPoint);
       
        }*/
        public string ToBaseString(StringBuilder sb)
        {
            sb.Length = 0;
            sb.AppendFormat("MaxDistance:{0},VelocityDecay:{1}\nWeaponId:{2},DistanceDecay:{3},StartPoint:{4},StartDir:{5}", MaxDistance.ToString("f2"), VelocityDecay.ToString("f2"), WeaponId, DistanceDecay.ToString("f2"), StartPoint,StartDir);
            return sb.ToString();
        }

        public string ToDynamicString(StringBuilder sb)
        {
            sb.Length = 0;
            sb.AppendFormat("Position:{0},Velocity:{1},Distance:{2},NextFrameTime:{3}", StartPoint, Velocity, Distance,
                RemainFrameTime);
            return sb.ToString();
        }
      
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
    
    public class BulletAssetComponent : SingleAssetComponent 
    {
        public override int GetComponentId() { { return (int)EComponentIds.BulletAsset; } }
    }

//    [Bullet, UniquePrefix("is")]
//    public class NewComponent : IComponent
//    {
//
//    }
//
//    [Bullet]
//    public class EmitPositionComponent : IComponent
//    {
//        public Vector3 Value; 
//    }
//
//    [Bullet]
//    public class PenetrateInfoComponent : IComponent
//    {
//        public List<EEnvironmentType> Layers;
//    }
}