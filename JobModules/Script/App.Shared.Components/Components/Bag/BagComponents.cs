using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;
using Entitas;
using Core.SyncLatest;

namespace App.Shared.Components.Bag
{
    public abstract class EquipmentComponent : IUserPredictionComponent
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(EquipmentComponent));
        [DontInitilize, NetworkProperty] public int Id;

        public abstract int GetComponentId();
        public abstract EWeaponSlotType GetSlotType();

        public virtual bool IsApproximatelyEqual(object right)
        {
            var equip = right as EquipmentComponent;
            if (null == equip)
            {
                Logger.Error("right is not EquipmentComponent");
                return false;
            }
            return equip.Id == Id;
        }

        public virtual void CopyFrom(object rightComponent)
        {
            var equip = rightComponent as EquipmentComponent;
            if (null == equip)
            {
                Logger.Error("right is not EquipmentComponent");
                return;
            }
            Id = equip.Id;
        }

        public override string ToString()
        {
            return string.Format("Id: {0}", Id);
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player, ]
    public class WeaponStateComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public int BagIndex;
        [DontInitilize, NetworkProperty] public bool BagLocked;
        [DontInitilize, NetworkProperty] public int BagOpenLimitTime;
        [NetworkProperty] public int CurrentWeaponSlot;
        [NetworkProperty] public int LastWeapon;
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
            var equip = rightComponent as WeaponStateComponent;
            if (null == equip)
            {
                return;
            }

            BagIndex = equip.BagIndex;
            BagLocked = equip.BagLocked;
            BagOpenLimitTime = equip.BagOpenLimitTime;
            CurrentWeaponSlot = equip.CurrentWeaponSlot;
            LastWeapon = equip.LastWeapon;
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
            return (int)EComponentIds.BagCurrentWeapon;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var comp = right as WeaponStateComponent;
            return BagIndex == comp.BagIndex
                && BagLocked == comp.BagLocked
                && BagOpenLimitTime == comp.BagOpenLimitTime
                && CurrentWeaponSlot == comp.CurrentWeaponSlot
                && LastWeapon == comp.LastWeapon
                && ReservedBullet12 == comp.ReservedBullet12
                && ReservedBullet300 == comp.ReservedBullet300
                && ReservedBullet45 == comp.ReservedBullet45
                && ReservedBullet50 == comp.ReservedBullet50
                && ReservedBullet556 == comp.ReservedBullet556
                && ReservedBullet762 == comp.ReservedBullet762
                && ReservedBullet9 == comp.ReservedBullet9
                && ReservedBullet57 == comp.ReservedBullet57;
        }

        public override string ToString()
        {
            return string.Format("CurrentWeaponSlot: {0}, LastWeapon: {1}, ReservedBullet300: {2}, ReservedBullet45: {3}, ReservedBullet50: {4}, ReservedBullet556: {5}, ReservedBullet762: {6}, ReservedBullet9: {7}, ReservedBullet12: {8}", CurrentWeaponSlot, LastWeapon, ReservedBullet300, ReservedBullet45, ReservedBullet50, ReservedBullet556, ReservedBullet762, ReservedBullet9, ReservedBullet12);
        }
    }


    [Player, ]
    public class GrenadeComponent : WeaponComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.BagGrenade;
        }

        public override EWeaponSlotType GetSlotType()
        {
            return EWeaponSlotType.GrenadeWeapon;
        }
    }

    [Player, ]
    public class TacticWeaponComponent : WeaponComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.BagTactic;
        }

        public override EWeaponSlotType GetSlotType()
        {
            return EWeaponSlotType.TacticWeapon;
        }
    }

    [Player, ]
    public class MeleeComponent : WeaponComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.BagMelee;
        }

        public override EWeaponSlotType GetSlotType()
        {
            return EWeaponSlotType.MeleeWeapon;
        }
    }

    [Player, ]
    public class PistolComponent : WeaponComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.BagPistol;
        }

        public override EWeaponSlotType GetSlotType()
        {
            return EWeaponSlotType.SubWeapon;
        }
    }

    [Player, ]
    public class PrimeWeaponComponent : WeaponComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.BagPrimeWeapon;
        }

        public override EWeaponSlotType GetSlotType()
        {
            return EWeaponSlotType.PrimeWeapon1;
        }

        public override string ToString()
        {
            return string.Format("{0}", base.ToString());
        }
    }

    [Player, ]
    public class SubWeaponComponent : WeaponComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.BagSubWeapon;
        }

        public override EWeaponSlotType GetSlotType()
        {
            return EWeaponSlotType.PrimeWeapon2;
        }
    }

  
    
    [Player, ]
    public class GrenadeCacheDataComponent : IUserPredictionComponent
    {
        [NetworkProperty] public int GrenadeCount1;
        [NetworkProperty] public int GrenadeCount2;
        [NetworkProperty] public int GrenadeCount3;
        [NetworkProperty] public int GrenadeCount4;

        public int GetComponentId()
        {
            return (int)EComponentIds.BagGrenadeInventory;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var comp = right as GrenadeCacheDataComponent;
            return GrenadeCount1 == comp.GrenadeCount1
                && GrenadeCount2 == comp.GrenadeCount2
                && GrenadeCount3 == comp.GrenadeCount3
                && GrenadeCount4 == comp.GrenadeCount4;
        }

        public void CopyFrom(object rightComponent)
        {
            var comp = rightComponent as GrenadeCacheDataComponent;
            GrenadeCount1 = comp.GrenadeCount1;
            GrenadeCount2 = comp.GrenadeCount2;
            GrenadeCount3 = comp.GrenadeCount3;
            GrenadeCount4 = comp.GrenadeCount4;
        }
        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

    }

    public abstract class WeaponComponent : EquipmentComponent
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponComponent));
        [DontInitilize, NetworkProperty] public int Muzzle;//枪口配件
        [DontInitilize, NetworkProperty] public int LowerRail;//枪把
        [DontInitilize, NetworkProperty] public int UpperRail;//瞄具
        [DontInitilize, NetworkProperty] public int Magazine;//弹夹
        [DontInitilize, NetworkProperty] public int Stock;//枪托
        [DontInitilize, NetworkProperty] public int Bullet;//子弹
        [DontInitilize, NetworkProperty] public int ReservedBullet;//子弹
        [DontInitilize, NetworkProperty] public bool IsBoltPulled;//是否上膛
        [DontInitilize, NetworkProperty] public int FireMode;//开火模式：单发，连发
        [DontInitilize, NetworkProperty] public int AvatarId;//皮肤ID

        public override bool IsApproximatelyEqual(object right)
        {
            var weapon = right as WeaponComponent;
            if (null == weapon)
            {
                Logger.Error("rightcomponent is not WeaponComponent");
                return false;
            }
            var r = base.IsApproximatelyEqual(right);
            r &= weapon.Muzzle == Muzzle;
            r &= weapon.LowerRail == LowerRail;
            r &= weapon.UpperRail == UpperRail;
            r &= weapon.Magazine == Magazine;
            r &= weapon.Stock == Stock;
            r &= weapon.Bullet == Bullet;
            r &= weapon.ReservedBullet == ReservedBullet;
            r &= weapon.IsBoltPulled == IsBoltPulled;
            r &= weapon.FireMode == FireMode;
            r &= weapon.AvatarId == AvatarId;
            return r;
        }

        public override void CopyFrom(object rightComponent)
        {
            base.CopyFrom(rightComponent);
            var weaponComponent = rightComponent as WeaponComponent;
            if (null == weaponComponent)
            {
                Logger.Error("rightcomponent is not WeaponComponent");
                return;
            }

            Muzzle = weaponComponent.Muzzle;
            LowerRail = weaponComponent.LowerRail;
            UpperRail = weaponComponent.UpperRail;
            Magazine = weaponComponent.Magazine;
            Stock = weaponComponent.Stock;
            Bullet = weaponComponent.Bullet;
            ReservedBullet = weaponComponent.ReservedBullet;
            IsBoltPulled = weaponComponent.IsBoltPulled;
            FireMode = weaponComponent.FireMode;
            AvatarId = weaponComponent.AvatarId;
        }

        public void ResetAttachments()
        {
            Muzzle = 0;
            LowerRail = 0;
            UpperRail = 0;
            Magazine = 0;
            Stock = 0;
        }

        public void Clear()
        {
            Id = 0;
            Bullet = 0;
            ResetAttachments();
            IsBoltPulled = false;
            FireMode = 0;
            ReservedBullet = 0;
            AvatarId = 0;
        }
    }

    [Player, ]
    public class OverrideBagComponent : ISelfLatestComponent
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
}
