using Utils.AssetManager;
using Core.Components;
using Core.EntityComponent;
using Core.Interpolate;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using XmlConfig;
using System;
using System.Collections.Generic;

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
        void SetContexts(IContexts contexts);
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
    public class AudioComponent: IComponent
    {
        public int AudioClientEffectType;
        [DontInitilize]
        public int AudioClientEffectArg1;
        [DontInitilize]
        public int AudioClientEffectArg2;
        [DontInitilize]
        public int AudioClientEffectArg3;
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
        [NetworkProperty(SyncFieldScale.Yaw)]
        public float Yaw;
        [NetworkProperty(SyncFieldScale.Pitch)]
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

        [NetworkProperty(SyncFieldScale.Position)]
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

    [ClientEffect]
    public class SprayPaintComponent : IPlaybackComponent 
    {
        [DontInitilize] [NetworkProperty(SyncFieldScale.Position)] public Vector3 SprayPaintPos; /*起始位置*/
        [DontInitilize] [NetworkProperty(1,-1,0.01f)] public Vector3 SprayPaintForward; /*朝向*/
        [DontInitilize] [NetworkProperty(8,0,1)] public int SprayPrintMask; /*掩码*/
        [DontInitilize] [NetworkProperty(360,-360,0.01f)] public Vector3 SprayPrintSize; /*大小*/
        [DontInitilize] [NetworkProperty(8,0,1)] public int SprayPrintType; /*类型*/
        [DontInitilize] [NetworkProperty(32767,0,1)] public int SprayPrintSpriteId; /*贴图*/

        public void CopyFrom(object rightComponent) {
            var r = rightComponent as SprayPaintComponent;
            SprayPaintPos = r.SprayPaintPos;
            SprayPaintForward = r.SprayPaintForward;
            SprayPrintMask = r.SprayPrintMask;
            SprayPrintSize = r.SprayPrintSize;
            SprayPrintType = r.SprayPrintType;
            SprayPrintSpriteId = r.SprayPrintSpriteId;
        }

        public int GetComponentId() {
            return (int)EComponentIds.SprayPaint;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo) {
            CopyFrom(left);
        }

        public bool IsInterpolateEveryFrame() {
            return true;
        }

        T[] FindObjectsOfType<T>() where T : UnityEngine.Object {
            return UnityEngine.Object.FindObjectsOfType<T>();
        }

        public GameObject[] GetAffectedObjects(Bounds bounds, LayerMask affectedLayers)
        {
            MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>();
            List<GameObject> objects = new List<GameObject>();
            foreach (Renderer r in renderers)
            {
                if (!r.enabled) continue;
                // 过滤
                if ((1 << r.gameObject.layer & affectedLayers.value) == 0) continue; 
                /*if (r.GetComponent<Decal>() != null) continue;*/

                if (bounds.Intersects(r.bounds))
                {
                    objects.Add(r.gameObject);
                }
            }
            return objects.ToArray();
        }

    }
}