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
        /// <summary>
        /// 做武器component统一获取适配
        /// </summary>
        WeaponState = 99,
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

    public interface IPlayerWeaponComponentArchive
    {
        /// <summary>
        /// 获取武器控制器实例
        /// </summary>
        /// <returns></returns>
        IPlayerWeaponController GetController();

        int GetReservedBullet();
        /// <summary>
        /// 获取当前槽位信息
        /// </summary>
        /// <returns></returns>
        EWeaponSlotType HeldSlotType { get; }


        /// 获取当前槽位信息
        /// </summary>
        /// <returns></returns>
        int HeldSlotWeaponId { get; }
        /// <summary>
        /// 获取当前槽位信息
        /// </summary>
        /// <returns></returns>
        /// <summary>
        WeaponInfo HeldSlotWeaponInfo { get; }
        /// <summary>
        /// 获取武器信息(槽位)
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        WeaponInfo GetSlot__WeaponInfo(EWeaponSlotType slot);
        /// <summary>
        ///  获取武器信息(槽位)
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="wpInfo"></param>
        /// <returns></returns>
        int GetSLot__WeaponId(EWeaponSlotType slot);


        bool TryGetSlot__WeaponInfo(EWeaponSlotType slot, out WeaponInfo wpInfo);

        /// <summary>
        /// 获取上一次使用的武器槽位
        /// </summary>
        /// <returns></returns>
        int GetLastWeaponSlot();

        /// <summary>
        /// 当前武器槽位是否有武器
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        bool IsWeaponSlotStuffed(EWeaponSlotType slot);

        /// <summary>
        /// 判断当前武器是否在槽位内
        /// </summary>
        /// <param name="weaponId"></param>
        /// <returns></returns>
        bool IsWeaponStuffedInSlot(int weaponId);


        /// <summary>
        /// 获取最近一次使用的武器槽位
        /// </summary>
        /// <returns></returns>
        EWeaponSlotType PopGetLastWeaponId();
        /// <summary>
        /// 当前开火模式
        /// </summary>
        int HeldFireMode { get; set; }

        bool HeldBolted { get; set; }

        int GetSlotFireModeCount(EWeaponSlotType slot);
        /// <summary>
        /// 当前武器子弹数
        /// </summary>
        /// <returns></returns>
        int HeldWeaponBullet { get; }
        /// <summary>
        /// 武器子弹数(槽位)
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        int GetSlotWeaponBullet(EWeaponSlotType slot);

        bool GetSlotWeaponBolted(EWeaponSlotType slot);

        /// <summary>
        /// 开火模式(武器)
        /// </summary>
        int GetSlotFireMode(EWeaponSlotType slot);
        bool IsHeldSlotType(EWeaponSlotType slot);
    }
    //WeaponInfo GetWeaponInfo(EWeaponSlotType slot);
    //WeaponInfo GetHeldSlot__WeaponInfo();
    //EWeaponSlotType PopLastWeaponSlot();

    //bool HasWeapon();
    //bool IsWeaponSlotStuffed(EWeaponSlotType slot);
    //bool HasWeapon(int weaponId);

    //bool CurBolted { get; set; }

    //void SetWeaponBullet(EWeaponSlotType slot, int count);
    //int GetWeaponBullet(EWeaponSlotType slot);
    //int GetWeaponBullet();

    //void SetWeaponBullet(int count);
    //int GetWeaponAvatarId(int weaponId);

    //EWeaponSlotType GetHeldSlot__Type();
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
