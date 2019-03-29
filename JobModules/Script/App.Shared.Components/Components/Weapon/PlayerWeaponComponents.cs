using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Components;
using Core.Prediction.UserPrediction;
using Entitas.CodeGeneration.Attributes;
using Core;
using Core.Utils;
using System.Collections.Generic;
using XmlConfig;
using UnityEngine;

using Core.ObjectPool;
using Entitas;
using System;
using Core.EntityComponent;
using System.Text;
using Core.UpdateLatest;

namespace App.Shared.Components.Player
{
    [Player]
    public class PlayerWeaponCustomizeComponent: IUserPredictionComponent
    {
        [NetworkProperty]
        public EntityKey GrenadeConstWeaponKey;
        [NetworkProperty]
        public EntityKey EmptyConstWeaponkey;
       

        public void CopyFrom(object rightComponent)
        {
            CopyFrom(rightComponent as PlayerWeaponCustomizeComponent);
        }
        void CopyFrom(PlayerWeaponCustomizeComponent comp)
        {
            GrenadeConstWeaponKey = comp.GrenadeConstWeaponKey;
            EmptyConstWeaponkey = comp.EmptyConstWeaponkey;

     
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerWeaponCustomize;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var cmp = right as PlayerWeaponCustomizeComponent;
            return GrenadeConstWeaponKey == cmp.GrenadeConstWeaponKey &&
                EmptyConstWeaponkey == cmp.EmptyConstWeaponkey;

        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    public class PlayerAudioComponent : IGameComponent
    {
     [NetworkProperty,DontInitilize]  public int LastFootPrintPlayStamp;
     [DontInitilize]  public int ReloadedBulletLeft;

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerAudio; 
        }
    }
    /// <summary>
    /// 武器背包集合
    /// </summary>
    [Player]
    public class PlayerWeaponBagSetComponent : IUserPredictionComponent
    {
        [NetworkProperty]
        public List<WeaponBagContainer> WeaponBags;
        [NetworkProperty, DontInitilize]
        public int HeldBagPointer;
        [NetworkProperty, DontInitilize]
        public int HeldBagPointer2;


        public WeaponBagContainer this[int bagIndex]
        {
            get
            {
                AssertUtility.Assert(bagIndex < WeaponBags.Count);
                if (bagIndex < 0)
                    bagIndex = HeldBagPointer;
                return WeaponBags[bagIndex];
            }
        }

        private bool isInitialized;
        public void CopyFrom(object rightComponent)
        {
            CopyFrom(rightComponent as PlayerWeaponBagSetComponent);
        }

        public void Initialize(int usableLength)
        {
            if (isInitialized)
                return;
            if (WeaponBags != null)
            {
                isInitialized = true;
                return;
            }
            WeaponBags = new List<WeaponBagContainer>(usableLength);
            for (int i = 0; i < usableLength; i++)
            {
                WeaponBags.Add(new WeaponBagContainer());
            }
            isInitialized = true;
        }

        private void CopyFrom(PlayerWeaponBagSetComponent right)
        {
            right.Initialize(GlobalConst.WeaponBagMaxCount);
            Initialize(GlobalConst.WeaponBagMaxCount);
            for (int i = 0; i < GlobalConst.WeaponBagMaxCount; i++)
            {
                //  DebugUtil.LogInUnity("left:{0} right:{1}", WeaponBags[i].ToString(), right.WeaponBags[i]);
                WeaponBags[i].RewindTo(right.WeaponBags[i]);
            }


        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponBagSet;
        }

        public bool IsApproximatelyEqual(object right)
        {
            return IsApproximatelyEqual(right as PlayerWeaponBagSetComponent);
        }
     
        private bool IsApproximatelyEqual(PlayerWeaponBagSetComponent rightComponent)
        {
                if (WeaponBags[0] == null || (!WeaponBags[0].IsSimilar(rightComponent.WeaponBags[0])))
                {

                    //builder.Append("Approxiamate diff :");
                    //builder.Append("left:"+ WeaponBags[i]+"\n");
                    //builder.Append("right:" + rightComponent.WeaponBags[i]+ "\n");
                    //Logger.InfoFormat(builder.ToString());
                    return false;
                }
;
            return true;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        //public override string ToString()
        //{
        //    //string str = "Prepare Convert Value....";
        //    //for (int i = 0; i < GameGlobalConst.WeaponBagMaxCount; i++)
        //    //{
        //    //    if (WeaponBags != null && WeaponBags[i] != null)
        //    //        str += WeaponBags[i].ToString();

        //    //}
        //    return str;
        //}
        public void ClearPointer()
        {
            HeldBagPointer2 = 0;
            WeaponBags[0].ClearPointer();
        }


        #region//shotcut

        public int HeldSlotIndex { get { return WeaponBags[HeldBagPointer].HeldSlotPointer; } }

        public int LastSlotIndex { get { return WeaponBags[HeldBagPointer].LastSlotPointer; } }

        public WeaponBagContainer HeldBagContainer { get { return WeaponBags[HeldBagPointer]; } }

        #endregion
    }
    
    /// <summary>
    /// 手雷背包缓存
    /// </summary>
    [Player,]
    public class GrenadeCacheDataComponent : IUserPredictionComponent
    {
        [NetworkProperty] public List<GrenadeCacheData> GrenadeArr;
        [NetworkProperty,DontInitilize] public int LastId;

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerGrenadeCache;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var comp = right as GrenadeCacheDataComponent;
            for (int i = 0; i < GrenadeArr.Count; i++)
            {
                if (!GrenadeArr[i].IsSimilar(comp.GrenadeArr[i]))
                    return false;
            }
            return LastId == comp.LastId;
        }
        public void InternalInitialize(int length)
        {
            if (GrenadeArr == null)
            {
                GrenadeArr = new List<GrenadeCacheData>(length);
                for (int i = 0; i < length; i++)
                {
                    GrenadeArr.Add(new GrenadeCacheData());
                }
            }
        }
      
        public void CopyFrom(object rightComponent)
        {
            var comp = rightComponent as GrenadeCacheDataComponent;
            var length = comp.GrenadeArr.Count;
            InternalInitialize(length);
            for (int i = 0; i < length; i++)
            {
                GrenadeArr[i].RewindTo(comp.GrenadeArr[i]);
            }
            LastId = comp.LastId;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player,]
    public class PlayerWeaponAmmunitionComponent : IUserPredictionComponent
    {

        [DontInitilize, NetworkProperty] public int ReservedBullet300;
        [DontInitilize, NetworkProperty] public int ReservedBullet45;
        [DontInitilize, NetworkProperty] public int ReservedBullet50;
        [DontInitilize, NetworkProperty] public int ReservedBullet556;
        [DontInitilize, NetworkProperty] public int ReservedBullet762;
        [DontInitilize, NetworkProperty] public int ReservedBullet9;
        [DontInitilize, NetworkProperty] public int ReservedBullet12;
        [DontInitilize, NetworkProperty] public int ReservedBullet57;

        public void CopyFrom(object rightComponent)
        {
            var equip = rightComponent as PlayerWeaponAmmunitionComponent;
            if (null == equip)
            {
                return;
            }
            ReservedBullet300 = equip.ReservedBullet300;
            ReservedBullet45 = equip.ReservedBullet45;
            ReservedBullet50 = equip.ReservedBullet50;
            ReservedBullet556 = equip.ReservedBullet556;
            ReservedBullet762 = equip.ReservedBullet762;
            ReservedBullet9 = equip.ReservedBullet9;
            ReservedBullet12 = equip.ReservedBullet12;
            ReservedBullet57 = equip.ReservedBullet57;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponAmmunition;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var comp = right as PlayerWeaponAmmunitionComponent;
            return
                 ReservedBullet12 == comp.ReservedBullet12
            && ReservedBullet300 == comp.ReservedBullet300
            && ReservedBullet45 == comp.ReservedBullet45
            && ReservedBullet50 == comp.ReservedBullet50
            && ReservedBullet556 == comp.ReservedBullet556
            && ReservedBullet762 == comp.ReservedBullet762
            && ReservedBullet9 == comp.ReservedBullet9
            && ReservedBullet57 == comp.ReservedBullet57;
        }
    }




[Player,]
    public class PlayerWeaponUpdateComponent : IUserPredictionComponent
    {
        //TODO:移到customize中去
        [NetworkProperty] public int TacticWeapon;
        [DontInitilize,NetworkProperty] public bool UpdateHeldAppearance;

        public void CopyFrom(object rightComponent)
        {
            var remote           = rightComponent as PlayerWeaponUpdateComponent;
            TacticWeapon         = remote.TacticWeapon;
            UpdateHeldAppearance = remote.UpdateHeldAppearance;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponUpdateComponent;
        }
        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public bool IsApproximatelyEqual(object right)
        {
            var rightC = right as PlayerWeaponUpdateComponent;
            return rightC.UpdateHeldAppearance == UpdateHeldAppearance && rightC.TacticWeapon == TacticWeapon;
        }
    }


    [Player,]
    public class PlayerWeaponAuxiliaryComponent : IUserPredictionComponent
    {
        // PlayerInterruptStateComponent
        //PlayerBulletDataComponent
        [DontInitilize]
        public List<PlayerBulletData> BulletList;
        //PlayerWeaponEffectComponent
        //客户端相关变量
        [Flags]
        public enum InterruptReason
        {
            BagUI = 1,
        }

        [DontInitilize] public int ForceInterruptGunSight;
        [DontInitilize] public bool ClientInitialize;
        [DontInitilize]
        public List<EClientEffectType> EffectList;
        //WeaponAutoStateComponent
        [Flags]
        public enum EAutoFireState
        {
            Burst,
            ReloadBreak,
        }
        //Auto Fire
        [DontInitilize] public bool HasAutoAction;
        [DontInitilize] public int AutoFire;
        [DontInitilize] public bool AutoThrowing;
        //TODO:移到别处
        [DontInitilize, NetworkProperty] public int BagOpenLimitTime;
        [DontInitilize, NetworkProperty] public bool BagLockState;

        public int GetComponentId()
        {
           return (int)EComponentIds.WeaponAux;
        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as PlayerWeaponAuxiliaryComponent;
            BagOpenLimitTime = remote.BagOpenLimitTime;
            BagLockState = remote.BagLockState;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as PlayerWeaponAuxiliaryComponent;
            return BagOpenLimitTime == remote.BagOpenLimitTime&&
            BagLockState == remote.BagLockState;
        }
    }
//#if UNITY_EDITOR
//    [Player,]
//    public class PlayerDebugComponent : IComponent
//    {
//        public WeaponBagDebugInfo DebugInfo1;
//        public WeaponBagDebugInfo DebugInfo2;
//        public WeaponBagDebugInfo DebugInfo3;
//        public WeaponBagDebugInfo DebugInfo4;


//    }
//#endif

    public class PlayerBulletData : BaseRefCounter
    {
        public Vector3 Dir;
        public Vector3 ViewPosition;
        public Vector3 EmitPosition;

        public static PlayerBulletData Allocate()
        {
            return ObjectAllocatorHolder<PlayerBulletData>.Allocate();
        }

        protected override void OnCleanUp()
        {
            ObjectAllocatorHolder<PlayerBulletData>.Free(this);
        }
    }




    [System.Obsolete]
    [Player]
    public class WeaponSound : IComponent
    {
        public List<EWeaponSoundType> PlayList;
    }






}
