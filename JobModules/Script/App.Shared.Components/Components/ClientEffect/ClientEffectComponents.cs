using Utils.AssetManager;
using Core.Components;
using Core.EntityComponent;
using Core.Interpolate;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using XmlConfig;

namespace App.Shared.Components.ClientEffect
{
    public interface IEffectLogic
    {
        AssetInfo[] AssetInfos { get; }
        int SoundId { get; }
        void Render(ClientEffectEntity entity);
        void Initialize(ClientEffectEntity entity);
        //void OnCreate(Contexts contexts, int subType);
        void OnCreate(EClientEffectType type, int effectId);
    }

    [ClientEffect]
    
    public class AssetsComponent : MultiAssetComponent
    {
        public bool IsLoadSucc;

        public bool IsInitialized;

        public override int GetComponentId() { { return (int)EComponentIds.ClientEffectAssets; } }
    }

    [ClientEffect]
    public class LogicComponent : IComponent
    {
        public IEffectLogic EffectLogic;
    }

    [ClientEffect]
    
    public class EffectIdComponent : IPlaybackComponent 
    {
        [NetworkProperty]
        public int Value;

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as EffectIdComponent;
            Value = r.Value;
        }
        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
       
        public int GetComponentId() { { return (int)EComponentIds.ClientEffectSubType; } }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    [ClientEffect]
    
    public class EffectRotationComponent : IPlaybackComponent 
    {
        [NetworkProperty]
        public float Yaw;
        [NetworkProperty]
        public float Pitch;

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as EffectRotationComponent;
            Yaw = r.Yaw;
            Pitch = r.Pitch;
        }
       

      
        public int GetComponentId() { { return (int)EComponentIds.ClientEffectRotation; } }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    [ClientEffect]
    
    public class EffectTypeComponent : IPlaybackComponent 
    {
        [NetworkProperty]
        public int Value;

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as EffectTypeComponent;
            Value = r.Value;
        }
       
        public int GetComponentId() { { return (int)EComponentIds.ClientEffectType; } }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    [ClientEffect, ]
    public class AttachParentComponent : IPlaybackComponent 
    {
        [NetworkProperty]
        public EntityKey ParentKey;
        [NetworkProperty]
        public Vector3 Offset;
        [NetworkProperty, DontInitilize]
        public int FragmentId;

        public int GetComponentId()
        {
            return (int)EComponentIds.ClientEffectAttachParent;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }

        public void CopyFrom(object rightComponent)
        {
            var comp = rightComponent as AttachParentComponent;
            ParentKey = comp.ParentKey;
            Offset = comp.Offset;
            FragmentId = comp.FragmentId;
        }
        
    }

    [ClientEffect]
    
    public class DamageHintComponent : IPlaybackComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.DamageHint; } }

        [NetworkProperty]
        public int Damage;

        [NetworkProperty]
        public bool DamageAdd;

        [NetworkProperty]
        public bool HeadShoot;

        [NetworkProperty]
        public Vector3 PlayerPos;

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as DamageHintComponent;
            Damage = r.Damage;
            DamageAdd = r.DamageAdd;
            HeadShoot = r.HeadShoot;
            PlayerPos = r.PlayerPos;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = left as DamageHintComponent;
            var r = right as DamageHintComponent;
            Damage = r.Damage;
            DamageAdd = r.DamageAdd;
            HeadShoot = r.HeadShoot;

            PlayerPos = InterpolateUtility.Interpolate(l.PlayerPos,
                r.PlayerPos, interpolationInfo);
        }
    }
}