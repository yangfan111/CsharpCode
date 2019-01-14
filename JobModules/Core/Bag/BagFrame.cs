using Assets.XmlConfig;
using Core.Enums;
using System;
using System.Collections.Generic;
using WeaponConfigNs;
using XmlConfig;

namespace Core.Bag
{
    public enum EWeaponSlotType
    {
        None = 0,
        PrimeWeapon1,
        PrimeWeapon2,
        SubWeapon,
        MeleeWeapon,
        GrenadeWeapon,
        TacticWeapon,
        Length,
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true,Inherited =false)]
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
        public readonly static WeaponInfo Empty = new WeaponInfo();
        public override string ToString()
        {
            return string.Format("id : {0}, avatarId {1}, muzzle {2}, magazine {3}, stock {4}, upper {5}, lower {6}, bullet {7}, reserved {8}",
                Id, AvatarId, Muzzle, Magazine, Stock, UpperRail, LowerRail, Bullet, ReservedBullet);
        }
    }
    public interface IGrenadeBagCacheAgent
    {
        int ShowCount(int id);
        void CacheLastGrenade(int lastId);
        int GetCount(int id);
        List<int> GetOwnedIds();

        int PickNextGrenadeInstance();
        int PickNextGrenadeSpecies();

        bool AddCache(int id);
        bool RemoveCache(int id);
        void SetCache(int id, int count);
        void Rewind();
        void Recycle();
    }
    public struct ItemInfo
    {
        public ECategory Category;
        public int ItemId;
        public int Count;
    }

    public interface IWeaponBagLogic
    {
    }
        //WeaponInfo GetWeaponInfo(EWeaponSlotType slot);
        //WeaponInfo GetCurrentWeaponInfo();
        //EWeaponSlotType PopLastWeaponSlot();

        //bool HasWeapon();
        //bool HasWeaponInSlot(EWeaponSlotType slot);
        //bool HasWeapon(int weaponId);

        //bool CurBolted { get; set; }

        //void SetWeaponBullet(EWeaponSlotType slot, int count);
        //int GetWeaponBullet(EWeaponSlotType slot);
        //int GetWeaponBullet();

        //void SetWeaponBullet(int count);
        //int GetWeaponAvatarId(int weaponId);

        //EWeaponSlotType GetCurrentWeaponSlot();
        //int CurrentWeaponId { get; }

        //int GetReservedBullet(EWeaponSlotType slot);
        //int GetReservedBullet(EBulletCaliber caliber);
        //void SetReservedBullet(EWeaponSlotType slot, int count);
        //int SetReservedBullet(EBulletCaliber caliber, int count);

        //int GetReservedBullet();
        //void SetReservedBullet(int count);

        //int CurFireMode { get; set; }
        //int GetFireMode(EWeaponSlotType slot);
        //int GetFireModeCount(EWeaponSlotType slot);
        /// <summary>
        /// 添加武器配件到当前武器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //EFuncResult SetCurrentWeaponPart(int id);
        ///// <summary>
        ///// 添加武器配件到指定位置
        ///// </summary>
        ///// <param name="slot"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //EFuncResult SetWeaponPart(EWeaponSlotType slot, int id);
        ///// <summary>
        ///// 删除指定位置武器的指定部位的配件
        ///// </summary>
        ///// <param name="slot"></param>
        ///// <param name="part"></param>
        //void DeleteWeaponPart(EWeaponSlotType slot, EWeaponPartType part);
        ///// <summary>
        ///// 将武器Id转为AvatarId
        ///// </summary>
        ///// <param name="weaponId"></param>
        //int GetAvatarById(int weaponId);

    //public interface IBagWeaponController
    //{
    //    /// <summary>
    //    /// 添加武器到指定位置
    //    /// </summary>
    //    /// <param name="slot"></param>
    //    /// <param name="weapon"></param>
    //    /// <returns></returns>
    //    WeaponInfo AddWeaponToSlot(EWeaponSlotType slot, WeaponInfo weapon);
    //    /// <summary>
    //    /// 设置当前武器
    //    /// </summary>
    //    /// <param name="slot"></param>
    //    void SetCurrentWeapon(EWeaponSlotType slot);
    //    /// <summary>
    //    /// 移除武器
    //    /// </summary>
    //    /// <param name="slot"></param>
    //    bool RemoveWeapon(EWeaponSlotType slot);
    //}
}
