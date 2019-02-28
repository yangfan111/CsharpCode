using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Components;
using Core.Prediction.UserPrediction;
using Entitas.CodeGeneration.Attributes;
using Core.SyncLatest;
using Core.EntityComponent;

namespace App.Shared.Components.Bag
{
    [Player]
    public class FirstWeaponBagComponent : WeaponBagComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.WeaponBag1;
        }
    }

    [Player]
    public class SecondaryWeaponBagComponent : WeaponBagComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.WeaponBag2;
        }
    }

    [Player]
    public class ThirdWeaponBagComponent : WeaponBagComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.WeaponBag3;
        }
    }

    [Player]
    public class ForthWeaponBagComponent : WeaponBagComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.WeaponBag4;
        }
    }

    [Player]
    public class FifthWeaponBagComponent : WeaponBagComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.WeaponBag5;
        }
    }

    public abstract class WeaponBagComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public int PrimeWeapon;
        [DontInitilize, NetworkProperty] public int SecondaryWeapon;
        [DontInitilize, NetworkProperty] public int PistolWeapon;
        [DontInitilize, NetworkProperty] public int MeleeWeapon;
        [DontInitilize, NetworkProperty] public int ThrowingWeapon;
        [DontInitilize, NetworkProperty] public int TacticWeapon;

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponBagComponent;
            PrimeWeapon = remote.PrimeWeapon;
            SecondaryWeapon = remote.SecondaryWeapon;
            PistolWeapon = remote.PistolWeapon;
            MeleeWeapon = remote.MeleeWeapon;
            ThrowingWeapon = remote.ThrowingWeapon;
            TacticWeapon = remote.TacticWeapon;
        }

        public abstract int GetComponentId();

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as WeaponBagComponent;
            return PrimeWeapon == remote.PrimeWeapon &&
            SecondaryWeapon == remote.SecondaryWeapon &&
            PistolWeapon == remote.PistolWeapon &&
            MeleeWeapon == remote.MeleeWeapon &&
            ThrowingWeapon == remote.ThrowingWeapon &&
            TacticWeapon == remote.TacticWeapon;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    public class EmptyHandComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public int EntityId;

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as EmptyHandComponent;
            EntityId = remote.EntityId;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.BagEmptyHand;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as EmptyHandComponent;
            return EntityId == remote.EntityId;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    public class BagStateComponent : IUserPredictionComponent
    {
        [NetworkProperty] public int CurSlot;  
        [DontInitilize, NetworkProperty] public int LastSlot;  
        [NetworkProperty] public int CurBag;  
        [DontInitilize, NetworkProperty] public int LastBag;

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as BagStateComponent;
            CurSlot = remote.CurSlot;
            LastSlot = remote.LastSlot;
            CurBag = remote.CurBag;
            LastBag = remote.LastBag;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.BagState;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as BagStateComponent;
            return CurSlot == remote.CurSlot
                && LastSlot == remote.LastSlot
                && CurBag == remote.CurBag
                && LastBag == remote.LastBag;
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
                && ReservedBullet12 == comp.ReservedBullet12
                && ReservedBullet300 == comp.ReservedBullet300
                && ReservedBullet45 == comp.ReservedBullet45
                && ReservedBullet50 == comp.ReservedBullet50
                && ReservedBullet556 == comp.ReservedBullet556
                && ReservedBullet762 == comp.ReservedBullet762
                && ReservedBullet9 == comp.ReservedBullet9
                && ReservedBullet57 == comp.ReservedBullet57;
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
