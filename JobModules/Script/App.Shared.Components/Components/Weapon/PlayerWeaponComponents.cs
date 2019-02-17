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

namespace App.Shared.Components.Player
{
    /// <summary>
    /// 武器背包集合
    /// </summary>
    [Player]
    public class PlayerWeaponBagSetComponent : IUserPredictionComponent
    {
        [NetworkProperty]
        public WeaponBagContainer[] WeaponBags;
        [NetworkProperty, DontInitilize]
        public int HeldBagPointer;



        public WeaponBagContainer this[int bagIndex]
        {
            get
            {
                AssertUtility.Assert(bagIndex < WeaponBags.Length);
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
        private void InteranlInitialize()
        {
            if (isInitialized) return;
            if (WeaponBags != null)
            {
                isInitialized = true;
                return;
            }
            WeaponBags = new WeaponBagContainer[GameGlobalConst.WeaponBagMaxCount];
            for (int i = 0; i < GameGlobalConst.WeaponBagMaxCount; i++)
            {
                WeaponBags[i] = new WeaponBagContainer();
            }
            isInitialized = true;
        }

        private void CopyFrom(PlayerWeaponBagSetComponent right)
        {
            InteranlInitialize();
            for (int i = 0; i < GameGlobalConst.WeaponBagMaxCount; i++)
            {
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
            for (int i = 0; i < GameGlobalConst.WeaponBagMaxCount; i++)
            {
                if (!WeaponBags[i].IsSimilar(rightComponent.WeaponBags[i]))
                    return false;
            }
            return true;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
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

        [NetworkProperty] public GrenadeCacheData[] GrenadeArr;

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerGrenadeCache;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var comp = right as GrenadeCacheDataComponent;
            for (int i = 0; i < GrenadeArr.Length; i++)
            {
                if (!GrenadeArr[i].IsSimilar(comp.GrenadeArr[i])) return false;
            }
            return true;
        }

        public void CopyFrom(object rightComponent)
        {
            var comp = rightComponent as GrenadeCacheDataComponent;
            for (int i = 0; i < GrenadeArr.Length; i++)
            {
                GrenadeArr[i].RewindTo(comp.GrenadeArr[i]);
            }
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
    public class OverrideBagComponent : Core.SyncLatest.ISelfLatestComponent
    {
        [NetworkProperty] public int TacticWeapon;

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as OverrideBagComponent;
            TacticWeapon = remote.TacticWeapon;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerOverrideBag;
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }


        [Player,]
    public class PlayerWeaponAuxiliaryComponent : IComponent
    {
       // PlayerInterruptStateComponent
        [Flags]
        public enum InterruptReason
        {
            BagUI = 1,
        }
        [DontInitilize] public int ForceInterruptGunSight;

        //PlayerBulletDataComponent
        public List<PlayerBulletData> BulletList;
        //PlayerWeaponEffectComponent
        public List<EClientEffectType> EffectList;
        //WeaponAutoStateComponent
        [Flags]
        public enum EAutoFireState
        {
            Burst,
            ReloadBreak,
        }
        [DontInitilize] public bool AutoThrowing;
        [DontInitilize] public int AutoFire;
        [DontInitilize] public int BagOpenLimitTime;


    }

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
