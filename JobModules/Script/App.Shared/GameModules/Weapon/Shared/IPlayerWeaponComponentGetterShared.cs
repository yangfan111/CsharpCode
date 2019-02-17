using Core;
using WeaponConfigNs;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="ISharedPlayerWeaponComponentGetter" />
    /// </summary>
    public interface ISharedPlayerWeaponComponentGetter : IPlayerModuleComponentAgent
    {
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
        bool IsHeldSlotType(EWeaponSlotType slot);

        /// <summary>
        /// 轮询获取上一把武器配置id
        /// </summary>
        /// <returns></returns>
        EWeaponSlotType PollGetLastSlotType();

        int GetReservedBullet(EWeaponSlotType slot);

        int GetReservedBullet(EBulletCaliber caliber);

        int GetReservedBullet();
    }
}
