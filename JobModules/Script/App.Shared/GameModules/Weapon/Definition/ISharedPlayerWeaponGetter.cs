using App.Shared.Components.Player;
using App.Shared.GameModules.Weapon;
using Core;
using Core.EntityComponent;
using System.Collections.Generic;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="ISharedPlayerWeaponGetter" />
    /// </summary>
    public interface ISharedPlayerWeaponGetter : IPlayerWeaponGetter
    {
        int ForceInterruptGunSight { get; }

        bool? AutoThrowing { get; }

        int? AutoFire { get; }

        bool CanSwitchWeaponBag { get; }

        int HeldConfigId { get; }

        int BagOpenLimitTIme { get; }

        int OverrideBagTactic { get; }

        bool BagLockState { get; }

        List<PlayerBulletData> BulletList { get; }

        List<EClientEffectType> EffectList { get; }


        EntityKey EmptyWeaponKey { get; }

        //        void AddAuxEffect(EClientEffectType effectType);
        //        void AddAuxBullet();
        //        void AddAuxBullet(PlayerBulletData bulletData);
        //        void AddAuxEffect();
        /// <summary>
        /// 当前武器槽位
        /// </summary>
        EWeaponSlotType HeldSlotType { get; }

        /// <summary>
        /// 当前槽位是否为空
        /// </summary>
        bool IsHeldSlotEmpty { get; }

        /// <summary>
        /// 指定槽位是否为空
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        bool IsWeaponSlotEmpty(EWeaponSlotType slot);

        /// <summary>
        /// 上一次切换的槽位
        /// </summary>
        EWeaponSlotType LastSlotType { get; }

        /// <summary>
        /// 当前背包索引
        /// </summary>
        int HeldBagPointer { get; }

        /// <summary>
        /// 判断是否为当前槽位
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        bool IsHeldSlotType(EWeaponSlotType slot, int bagIndex = -1);
        /// <summary>
        /// 轮询获取上一把武器配置id
        /// </summary>
        /// <returns></returns>
        EWeaponSlotType PollGetLastSlotType();

        int GetReservedBullet(EWeaponSlotType slot);

        int GetReservedBullet(EBulletCaliber caliber);

        int GetReservedBullet();

        /// <summary>
        /// 获取当前武器Agent，默认不为空，通过IsVailed判断是否空手
        /// </summary>
        WeaponComponentsAgent HeldWeaponAgent { get; }

        /// <summary>
        /// 获取 槽位武器Agent，默认不为空，通过IsVailed判断是否空手 
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagIndex"></param>
        /// <returns></returns>
        WeaponComponentsAgent GetWeaponAgent(EWeaponSlotType slotType = EWeaponSlotType.Pointer, int bagIndex = -1);

        IGrenadeCacheHelper GetBagCacheHelper(EWeaponSlotType slotType);

        bool IsWeaponInSlot(int configId);

        void PlayFireAudio();
    }
}
