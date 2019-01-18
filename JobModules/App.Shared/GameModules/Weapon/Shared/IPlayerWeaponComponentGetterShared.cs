using Core;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    public interface ISharedPlayerWeaponComponentGetter : IPlayerModuleComponentAgent
    {
        ///// <summary>
        ///// 获取武器控制器实例
        ///// </summary>
        ///// <returns></returns>
        //IPlayerModuleController GetController();


        /// <summary>
        /// 获取当前槽位信息
        /// </summary>
        /// <returns></returns>
        EWeaponSlotType CurrSlotType { get; }


        /// 获取当前槽位信息
        /// </summary>
        /// <returns></returns>
        int CurrSlotWeaponId { get; }
        /// <summary>
        /// 获取当前槽位信息
        /// </summary>
        /// <returns></returns>
        /// <summary>
        WeaponInfo CurrSlotWeaponInfo { get; }
        /// <summary>
        /// 获取武器信息(槽位)
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        WeaponInfo GetSlotWeaponInfo(EWeaponSlotType slot);
        /// <summary>
        ///  获取武器信息(槽位)
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="wpInfo"></param>
        /// <returns></returns>
        int GetSlotWeaponId(EWeaponSlotType slot);


        bool TryGetSlotWeaponInfo(EWeaponSlotType slot, out WeaponInfo wpInfo);

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

        bool IsWeaponCurrSlotStuffed();

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
        int CurrFireMode { get; set; }

        bool CurrBolted { get; set; }

        int GetSlotFireModeCount(EWeaponSlotType slot);
        /// <summary>
        /// 当前武器子弹数
        /// </summary>
        /// <returns></returns>
        int CurrWeaponBullet { get; }
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
        bool IsCurrSlotType(EWeaponSlotType slot);
        int GetReservedBullet();
        int GetReservedBullet(EWeaponSlotType slot);
        int GetReservedBullet(EBulletCaliber caliber);
    }
    //WeaponInfo GetWeaponInfo(EWeaponSlotType slot);
    //WeaponInfo GetCurrSlot__WeaponInfo();
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

    //EWeaponSlotType GetCurrSlot__Type();
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
