using Assets.XmlConfig;
using Core.EntityComponent;
using Core.Enums;
using System;
using System.Collections.Generic;
using WeaponConfigNs;
using XmlConfig;

namespace Core
{
    public enum EWeaponSlotType
    {
        None = 0,
        PrimeWeapon,
        SecondaryWeapon,
        PistolWeapon,
        MeleeWeapon,
        ThrowingWeapon,
        TacticWeapon,
        Length,
    }

    public enum EWeaponBagIndex
    {
        None = 0,
        First,
        Second,
        Third,
        Forth,
        Fifth,
        Length,
    } 

    public enum EWeaponSlotsGroupType
    {
        Default,
        Group,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class WeaponSpeciesAttribute : Attribute
    {
        public readonly EWeaponSlotType slotType;

        public WeaponSpeciesAttribute(EWeaponSlotType stype)
        {
            this.slotType = stype;
        }
    }
    public struct WeaponInfo
    {
        public int Id;
        public int AvatarId;
        public int Muzzle;
        public int Magazine;
        public int Stock;
        public int UpperRail;
        public int LowerRail;
        public int Bullet;
        public int ReservedBullet;
        public int weaponKey;
        public readonly static WeaponInfo Empty = new WeaponInfo();
        public override string ToString()
        {
            return string.Format("id : {0}, avatarId {1}, muzzle {2}, magazine {3}, stock {4}, upper {5}, lower {6}, bullet {7}, reserved {8}",
                Id, AvatarId, Muzzle, Magazine, Stock, UpperRail, LowerRail, Bullet, ReservedBullet);
        }
    }
    public interface IPlayerWeaponComponentGetter
    {
    }
    public interface IPlayerModuleComponentAgent
    {
       
    }
    public interface IPlayerModuleController { }

    /// <summary>
    /// 武器相关的人物动作 地面-当前武器-背包
    /// </summary>
   
    
    public struct ItemInfo
    {
        public ECategory Category;
        public int ItemId;
        public int Count;
    }


}