using Assets.Utils.Configuration;
using Assets.XmlConfig;
using System;
using Utils.Singleton;

namespace Core
{
    /// <summary>
    /// Defines the <see cref="WeaponSpeciesAttribute" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class WeaponSpeciesAttribute : Attribute
    {
        public readonly EWeaponSlotType slotType;

        public WeaponSpeciesAttribute(EWeaponSlotType stype)
        {
            this.slotType = stype;
        }
    }

    /// <summary>
    /// 武器相关的人物动作 地面-当前武器-背包
    /// </summary>
    public struct ItemInfo
    {
        public ECategory Category;
        public int ItemId;
        public int Count;
    }

    public struct MeleeAttackInfo
    {
        public EMeleeAttackType AttackType;
    }

    public struct WeaponPartsStruct
    {
        public int UpperRail;
        public int LowerRail;
        public int Magazine;
        public int Stock;
        public int Muzzle;
        public int SideRail;
        public int Bore;
        public int Feed;
        public int Trigger;
        public int Interlock;
        public int Brake;

        public readonly static WeaponPartsStruct Default = new WeaponPartsStruct();

        public WeaponPartsStruct Clone()
        {
            var result = new WeaponPartsStruct();
            result.UpperRail = UpperRail;
            result.LowerRail = LowerRail;
            result.Magazine = Magazine;
            result.Stock = Stock;
            result.Muzzle = Muzzle;
            result.SideRail = SideRail;
            result.Bore = Bore;
            result.Feed = Feed;
            result.Trigger = Trigger;
            result.Interlock = Interlock;
            result.Brake = Brake;
            return result;
        }

        public WeaponPartsStruct Sync(WeaponScanStruct scanData)
        {
            LowerRail = scanData.LowerRail;
            UpperRail = scanData.UpperRail;
            Magazine = scanData.Magazine;
            Stock = scanData.Stock;
            Muzzle = scanData.Muzzle;
            SideRail = scanData.SideRail;
            Bore = scanData.Bore;
            Feed = scanData.Feed;
            Trigger = scanData.Trigger;
            Interlock = scanData.Interlock;
            Brake = scanData.Brake;
            return this;
        }

        public override string ToString()
        {
            return string.Format("Upper {0}, Lower {1}, Magazine {2}, Stock {3}, Muzzle {4}",
                UpperRail, LowerRail, Magazine, Stock, Muzzle);
        }

        public static bool operator ==(WeaponPartsStruct x, WeaponPartsStruct y)
        {
            return x.LowerRail == y.LowerRail && x.UpperRail == y.UpperRail && x.Stock == y.Stock
                && x.Muzzle == y.Muzzle && x.Magazine == y.Magazine && x.SideRail == y.SideRail
                && x.Bore == y.Bore && x.Feed == y.Feed && x.Trigger == y.Trigger
                && x.Interlock == y.Interlock && x.Brake == y.Brake;
        }

        public static bool operator !=(WeaponPartsStruct x, WeaponPartsStruct y)
        {
            return x.LowerRail != y.LowerRail || x.UpperRail != y.UpperRail || x.Stock != y.Stock
                || x.Muzzle != y.Muzzle || x.Magazine != y.Magazine || x.SideRail != y.SideRail
                || x.Bore != y.Bore || x.Feed != y.Feed || x.Trigger != y.Trigger
                || x.Interlock != y.Interlock || x.Brake != y.Brake;
        }
    }

    /// <summary>
    /// 武器展示数据
    /// </summary>
    public struct WeaponScanStruct
    {
        public int ConfigId;
        public int AvatarId;
        public int Muzzle;
        public int Magazine;
        public int Stock;
        public int UpperRail;
        public int LowerRail;
        public int SideRail;
        public int Bore;
        public int Feed;
        public int Trigger;
        public int Interlock;
        public int Brake;
        public int Bullet;
        public int ReservedBullet;

        public readonly static WeaponScanStruct Empty = new WeaponScanStruct();

        public static bool operator ==(WeaponScanStruct x, WeaponScanStruct y)
        {
            return x.ConfigId == y.ConfigId;
        }

        public static bool operator !=(WeaponScanStruct x, WeaponScanStruct y)
        {
            return x.ConfigId != y.ConfigId;
        }

        public override string ToString()
        {
            return string.Format(
                "id : {0}, avatarId {1}, muzzle {2}, magazine {3}, stock {4}, upper {5}, lower {6}, bullet {7}, reserved {8}",
                ConfigId, AvatarId, Muzzle, Magazine, Stock, UpperRail, LowerRail, Bullet, ReservedBullet);
        }

        public bool IsValid
        {
            get { return ConfigId > 0; }
        }

        public bool IsSafeVailed
        {
            get
            {
                if (ConfigId < 1) return false;
                return SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(ConfigId) != null;
            }
        }
    }
}





