﻿using Core;
using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.SceneTriggerObject;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.AssetManager;
using XmlConfig;

namespace App.Shared.Components.SceneObject
{
    [SceneObject]
    public class SizeComponent : IComponent
    {
        public float Value;
    }

    [SceneObject]
    public class AudioComponent : IPlaybackComponent
    {
        public EAudioUniqueId uniqueId;


        public int GetComponentId()
        {
            return (int)EComponentIds.SceneObjectAudioObject;

        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as AudioComponent;
            uniqueId = remote.uniqueId;

        }

        public bool IsInterpolateEveryFrame() { return false; }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    [SceneObject]
    public class UnityObjectComponent : SingleAssetComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.SceneObjectGameObject;
        }
    }

    [SceneObject]
    public class MultiUnityObjectComponent : MultiAssetComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.SceneObjectMultiGameObjects;
        }
    }

    public struct PartAssetData
    {
        public GameObject UnityObject;
        public int PartId;
        public int PartSlotType;
        public WeaponPartLocation PartLocation;
    }
    [SceneObject]
    public class WeaponAttachmentComponent : IComponent,IResetableComponent
    {
        [DontInitilize]
        public Dictionary<AssetInfo, PartAssetData> PartAssetDict
        {
            get;
            private set;
        }

        public bool ContainsPart(AssetInfo assetInfo)
        {
            PartAssetData assetData;
            PartAssetDict.TryGetValue(assetInfo, out assetData);
            return assetData.PartId > 0;
        }
//        [DontInitilize]
//        public Dictionary<AssetInfo, UnityObject> EffectAssetDict
//        {
//            get;
//            private set;
//        }
        [DontInitilize]
        public List<UnityObject> EffectAssetList { get; private set; }

        public WeaponAttachmentComponent()
        {
            PartAssetDict = new Dictionary<AssetInfo, PartAssetData>(AssetInfo.AssetInfoComparer.Instance);
            EffectAssetList  = new List<UnityObject>();


        }
        public void Reset()
        {
            EffectAssetList.Clear();
            PartAssetDict.Clear();
        }
        
    }

    [SceneObject, ]
    public class WeaponObjectComponent : IPlaybackComponent 
    {
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int ConfigId;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int WeaponAvatarId;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int UpperRail;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int LowerRail;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int SideRail;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Stock;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Muzzle;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Magazine;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Bullet;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int ReservedBullet;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Bore;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Feed;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Trigger;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Interlock;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Brake;

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponObjectComponent;
            ConfigId = remote.ConfigId;
            WeaponAvatarId = remote.WeaponAvatarId;
            UpperRail = remote.UpperRail;
            LowerRail = remote.LowerRail;
            SideRail = remote.SideRail;
            Stock = remote.Stock;
            Muzzle = remote.Muzzle;
            Magazine = remote.Magazine;
            Bullet = remote.Bullet;
            ReservedBullet = remote.ReservedBullet;
            Bore = remote.Bore;
            Feed = remote.Feed;
            Trigger = remote.Trigger;
            Interlock = remote.Interlock;
            Brake = remote.Brake;
        }
        public void GameCopyFrom(WeaponScanStruct remote)
        {
            ConfigId = remote.ConfigId;
            WeaponAvatarId = remote.AvatarId;
            UpperRail = remote.UpperRail;
            LowerRail = remote.LowerRail;
            SideRail = remote.SideRail;
            Stock = remote.Stock;
            Muzzle = remote.Muzzle;
            Magazine = remote.Magazine;
            Bullet = remote.Bullet;
            ReservedBullet = remote.ReservedBullet;
            Bore = remote.Bore;
            Feed = remote.Feed;
            Trigger = remote.Trigger;
            Interlock = remote.Interlock;
            Brake = remote.Brake;
        }
     
        public static explicit operator WeaponScanStruct(WeaponObjectComponent remote)
        {
            var newComp = new WeaponScanStruct();
            newComp.ConfigId = remote.ConfigId;
            newComp.AvatarId = remote.WeaponAvatarId;
            newComp.UpperRail = remote.UpperRail;
            newComp.LowerRail = remote.LowerRail;
            newComp.SideRail = remote.SideRail;
            newComp.Stock = remote.Stock;
            newComp.Muzzle = remote.Muzzle;
            newComp.Magazine = remote.Magazine;
            newComp.Bullet = remote.Bullet;
            newComp.ReservedBullet = remote.ReservedBullet;
            newComp.Bore = remote.Bore;
            newComp.Feed = remote.Feed;
            newComp.Trigger = remote.Trigger;
            newComp.Interlock = remote.Interlock;
            newComp.Brake = remote.Brake;
            return newComp;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.SceneObjectWeapon;
        }
     
        public bool IsInterpolateEveryFrame(){ return false; }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    [SceneObject, ]
    public class TeamComponent : IPlaybackComponent
    {
        [NetworkProperty] public long TeamId;

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as TeamComponent;
            TeamId = remote.TeamId;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.SceneObjectTeam;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    [SceneObject, ]
    public class CastFlagComponent : IPlaybackComponent
    {
        [NetworkProperty] public int Flag;

        public void CopyFrom(object rightComponent)
        {
            var flagComponent = rightComponent as CastFlagComponent;
            Flag = flagComponent.Flag;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.SceneObjectCastFlag;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    [SceneObject, ]
    public class SimpleCastTargetComponent : IPlaybackComponent
    {
        [NetworkProperty] public int Key;
        [NetworkProperty] public float Scale;
        [NetworkProperty] public string Tip;

        public void CopyFrom(object rightComponent)
        {
            var simpleCastTargetComponent = rightComponent as SimpleCastTargetComponent;
            Key = simpleCastTargetComponent.Key;
            Scale = simpleCastTargetComponent.Scale;
            Tip = simpleCastTargetComponent.Tip;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.SceneObjectCastTarget;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }

    /// <summary>
    /// 根据配置生成的场景物件
    /// </summary>
    [SceneObject, ]
    public class SimpleItemComponent : IPlaybackComponent
    {
        [NetworkProperty]
        public int Id;
        [NetworkProperty]
        public int Count;
        [NetworkProperty]
        public int Category;

        public virtual void CopyFrom(object rightComponent)
        {
            var comp = rightComponent as SimpleItemComponent;
            if (null == comp)
            {
                return;
            }
            Id = comp.Id;
            Count = comp.Count;
            Category = comp.Category;
        }

        public virtual int GetComponentId()
        {
            return (int)EComponentIds.SceneObjectItem;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
       
    }
    [SceneObject]
    public class AudioTestEmitterComponent : IComponent
    {
        public GameObject P1;
        public GameObject P3;
        [DontInitilize]public GameObject Self;

    }
    [SceneObject,MapObject]
    public class RawGameObjectComponent : IComponent
    {
        public GameObject Value;
    }

    [SceneObject]
    public class ThrowingComponent : IComponent
    {
        [DontInitilize] public int Time;
        [DontInitilize] public Vector3 Velocity;
    }

    [SceneObject]
    public class ArmorDurabilityComponent : IComponent
    {
        public int CurDurability;
        public int MaxDurability;
    }

    [MapObject]
    public class TriggerObjectIdComponent : IPlaybackComponent
    {
        [NetworkProperty]
        public int Id;


        public int GetComponentId()
        {
            return (int)EComponentIds.SceneTriggerObject;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = (TriggerObjectIdComponent) rightComponent;
            Id = r.Id;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }
    }


    [SceneObject,MapObject]
    public class ResetComponent : IComponent
    {
        public Action<Entity> ResetAction;

        public void Reset(Entity entity)
        {
            ResetAction(entity);
        }
    }

    public enum DoorState
    {
        Closed = 0,
        OpenMin,
        OpenMax,
        Rotating,
        Broken,
    }

    [MapObject]
    public class DestructibleObjectFlagComponent : IPlaybackComponent
    {
        public void CopyFrom(object rightComponent)
        {
            
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.SceneDestructibleObjectFlag;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            
        }
    }

    public abstract class BitsMapComponent
    {
        private static readonly int[] MultiplyDeBruijnBitPosition = new int[]
        {0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
            31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9};

        protected static int GetNextBitSet(int state, int startBitIndex)
        {
            state &= ~(int)((uint)(1 << startBitIndex) - 1);
            if (state != 0)
            {
                return MultiplyDeBruijnBitPosition[((uint)((state & -state) * 0x077CB531U)) >> 27];
            }

            return -1;
        }
    }


    public struct DestrutibleDataSync
    {
        public int StateDiff;
        public bool Reset;
    }

    [MapObject]
    public class DestructibleDataComponent : BitsMapComponent, IPlaybackComponent, IResetableComponent
    {

        [NetworkProperty]
        public bool StartAsWhole;

        [DontInitilize]
        public float SyncDelay;

        [DontInitilize]
        public int DestructionState;

        [DontInitilize]
        public int LocalSyncDestructionState;

        [NetworkProperty]
        [DontInitilize]
        public int LastSyncDestructionState;
        
        [DontInitilize]
        public int LocalResetCount;

        [NetworkProperty]
        [DontInitilize]
        public short ResetCount;


        public bool HasAnyChunkDetached()
        {
            return DestructionState != 0;
        }

        public bool IsInDestructionState(int chunkId)
        {
            return (DestructionState & (1 << chunkId)) != 0;
        }

        public void SetDestruction(int id)
        {
            if (StartAsWhole)
            {
                DestructionState = -1;//0xfffffff
            }
            else
            {
                DestructionState |= 1 << id;
            }
        }

        public DestrutibleDataSync SyncDestructionState()
        {
            DestrutibleDataSync sync;
            sync.Reset = LocalResetCount != ResetCount;
            if (sync.Reset)
            {
                ResetState();
            }

            sync.StateDiff = sync.Reset ? LastSyncDestructionState : (LastSyncDestructionState ^ LocalSyncDestructionState) & LastSyncDestructionState;

            LocalResetCount = ResetCount;
            LocalSyncDestructionState = LastSyncDestructionState;
            return sync;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.SceneDestructibleData;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = (DestructibleDataComponent)rightComponent;
            StartAsWhole = r.StartAsWhole;
            LastSyncDestructionState = r.LastSyncDestructionState;
            ResetCount = r.ResetCount;
        }

        public void ResetOnAction()
        {
            ResetState();
            ResetCount++;
        }

        public void Reset()
        {
            ResetState();
            ResetCount = 0;
        }

        private void ResetState()
        {
            DestructionState = LocalSyncDestructionState = LastSyncDestructionState = 0;
        }

        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = (DestructibleDataComponent) left;
            var r = (DestructibleDataComponent) right;

            StartAsWhole = l.StartAsWhole || r.StartAsWhole;
            LastSyncDestructionState = l.LastSyncDestructionState | r.LastSyncDestructionState;
            ResetCount = r.ResetCount;
        }

        public int GetNextDetachedChunkId(int currentBrokenId = -1)
        {
            var startBit = currentBrokenId + 1;
            var id = -1;
            if (startBit < 32)
            {
                id = GetNextBitSet(DestructionState, startBit);
            }

            return id;
        }
    }

    [MapObject ]
    public class DoorDataComponent :  IPlaybackComponent
    {
        [NetworkProperty]
        [DontInitilize]
        public int State;


        public float InitialRotation;

        [NetworkProperty(SyncFieldScale.EularAngle)]
        public float Rotation;//[0, 360)

        public int GetComponentId()
        {
            return (int) EComponentIds.SceneDoorData;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = (DoorDataComponent) left;
            var r = (DoorDataComponent) right;
            State = l.State;
            Rotation = InterpolateAngle(l.Rotation, r.Rotation, interpolationInfo);
        }

        //l,r are [0, 360)
        private static float InterpolateAngle(float l, float r, IInterpolationInfo interpolationInfo)
        {
            var delta = r - l;
            if (delta <= -180)
            {
                delta += 360;
            }
            else if (delta > 180)
            {
                delta -= 360;
            }
            var angle = l + InterpolateUtility.Interpolate(0, delta, interpolationInfo);

            if (angle >= 360)
            {
                angle -= 360;
            }else if (angle < 0)
            {
                angle += 360;
            }

            return angle;

        }

        public void CopyFrom(object rightComponent)
        {
            var r = (DoorDataComponent) rightComponent;
            State = r.State;
            Rotation = r.Rotation;
        }

        public bool IsOpenable()
        {
            return State == (int) DoorState.Closed ||
                   State == (int) DoorState.OpenMax ||
                   State == (int) DoorState.OpenMin;
        }

        public void Reset()
        {
            State = (int) DoorState.Closed;
            Rotation = InitialRotation;
        }
    }

    [MapObject]
    public class DoorRotateComponent : IComponent
    {
        public float Current;
        public float From;
        public float To;
        public int EndState;
    }

    [MapObject, ]
    public class GlassyDataComponent : BitsMapComponent, IPlaybackComponent, IResetableComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(GlassyDataComponent));

        [NetworkProperty]
        [DontInitilize]
        public int BrokenState0;

        [NetworkProperty]
        [DontInitilize]
        public int BrokenState1;

        [NetworkProperty]
        [DontInitilize]
        public int BrokenState2;

        [NetworkProperty]
        [DontInitilize]
        public int BrokenState3;

        [DontInitilize]
        public bool IsStateChanged;

        [DontInitilize]
        public bool HasLocalBrokenChunks;

        [DontInitilize]
        public bool HasFirstLocalSync;

        public int GetComponentId()
        {
            return (int) EComponentIds.SceneGlassyData;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = (GlassyDataComponent) rightComponent;
            BrokenState0 = r.BrokenState0;
            BrokenState1 = r.BrokenState1;
            BrokenState2 = r.BrokenState2;
            BrokenState3 = r.BrokenState3;
        }

        public bool HasChangedState()
        {
            return IsStateChanged || (HasLocalBrokenChunks && !HasAnyChunkBroken()) || !HasFirstLocalSync; 
        }

        public void ClearChangedState()
        {
            IsStateChanged = false;
            HasFirstLocalSync = true;
        }

        private int CombineState(int originState, int newState)
        {
            var state = originState | newState;
            IsStateChanged = IsStateChanged || state != originState;
            return state;
        }

        public void SetBroken(int chunkId)
        {
            if (IsValidChunkId(chunkId))
            {
                int stateIndex = chunkId >> 5;
                int bitIndex = chunkId & 0x1f;

                switch (stateIndex)
                {
                    case 0:
                        BrokenState0 |= 1 << bitIndex;
                        break;
                    case 1:
                        BrokenState1 |= 1 << bitIndex;
                        break;
                    case 2:
                        BrokenState2 |= 1 << bitIndex;
                        break;
                    case 3:
                        BrokenState3 |= 1 << bitIndex;
                        break;
                }
            }
        }

        public bool IsBroken(int chunkId)
        {
            if (IsValidChunkId(chunkId))
            {
                int stateIndex = chunkId >> 5;
                int bitIndex = chunkId & 0x1f;

                switch (stateIndex)
                {
                    case 0:
                        return (BrokenState0 & (1 << bitIndex)) != 0;
                    case 1:
                        return (BrokenState1 & (1 << bitIndex)) != 0;
                    case 2:
                        return (BrokenState2 & (1 << bitIndex)) != 0;
                    case 3:
                        return (BrokenState3 & (1 << bitIndex)) != 0;
                }
            }

            return false;
        }

        public bool HasAnyChunkBroken()
        {
            return BrokenState0 != 0 || BrokenState1 != 0 ||
                   BrokenState2 != 0 || BrokenState3 != 0;
        }

        public int GetNextBrokenChunkId(int currentBrokenId = -1)
        {
            var startId = currentBrokenId + 1;
            int stateIndex = startId >> 5;
            int bitIndex = startId & 0x1f;

            var id = -1;
            while (stateIndex < 4)
            {
                var brokenState = 0;
                switch (stateIndex)
                {
                    case 0:
                        brokenState = BrokenState0;
                        break;
                    case 1:
                        brokenState = BrokenState1;
                        break;
                    case 2:
                        brokenState = BrokenState2;
                        break;
                    case 3:
                        brokenState = BrokenState3;
                        break;
                }

                var nextId = GetNextBitSet(brokenState, bitIndex);

                if (nextId >= 0)
                {
                    id = nextId + (stateIndex << 5); ;
                    break;
                }

                bitIndex = 0;
                stateIndex++;
            }

            return id;
        }

        private bool IsValidChunkId(int chunkId)
        {
            if (chunkId < 0 || chunkId >= 128)
            {
                _logger.ErrorFormat("Invalid ChunkId {0}", chunkId);
                return false;
            }

            return true;
        }

        public void Reset()
        {
            BrokenState0 = 0;
            BrokenState1 = 0;
            BrokenState2 = 0;
            BrokenState3 = 0;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = (GlassyDataComponent) left;
            var r = (GlassyDataComponent) right;

            BrokenState0 = CombineState(l.BrokenState0, r.BrokenState0);
            BrokenState1 = CombineState(l.BrokenState1, r.BrokenState1);
            BrokenState2 = CombineState(l.BrokenState2, r.BrokenState2);
            BrokenState3 = CombineState(l.BrokenState3, r.BrokenState3);
        }
    }

    [MapObject]
    public class TriggerObjectEventComponent : IResetableComponent, IComponent
    {
        public Queue<TriggerObjectSyncEvent> SyncEvents = new Queue<TriggerObjectSyncEvent>();

        public void Reset()
        {
            while (SyncEvents.Count > 0)
            {
                var e = SyncEvents.Dequeue();
                e.ReleaseReference();
            }
        }
    }

    [MapObject]
    public class TriggerObjectEventFlagComponent : IComponent
    {
        
    }
}