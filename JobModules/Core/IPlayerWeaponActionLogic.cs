using Core.Bag;
using System;

namespace Core
{
    
    /// <summary>
    /// 武器相关的人物动作 地面-当前武器-背包
    /// </summary>
    public interface IPlayerWeaponController
    {
        ///// <summary>
        ///// 装备武器到手上（包括动作）
        ///// </summary>
        ///// <param name="slot"></param>
        //void DrawSlotWeapon(EWeaponSlotType slot);
        ///// <summary>
        ///// 装备武器到手上（无动作）
        ///// </summary>
        ///// <param name="player"></param>
        ///// <param name="slot"></param>
        //void TryMountSlotWeapon(EWeaponSlotType slot);
        ///// <summary>
        ///// 卸除当前武器（有动作）
        ///// </summary>
        ////void UnmountWeapon(float holsterParam);
        //void UnmountHeldWeapon();
        ///// <summary>
        ///// 卸除当前武器，动作完成后回调
        ///// </summary>
        ///// <param name="callback"></param>
        //void UnmountHeldWeapon(Action callback);
        ///// <summary>
        ///// 强制卸除当前武器（无动作）
        ///// </summary>
        //void ForceUnmountHeldWeapon();
        ///// <summary>
        ///// 捡起武器，捡起的武器会拿到手上（有动作）
        ///// </summary>
        ///// <param name="itemId"></param>
        //WeaponInfo PickUpWeapon(WeaponInfo weaponInfo);
        ///// <summary>
        ///// 自动捡取武器，捡起的武器不会拿到手上，无动作
        ///// </summary>
        ///// <param name="weaponInfo"></param>
        //bool AutoPickUpWeapon(WeaponInfo weaponInfo);
        ///// <summary>
        ///// 装备武器到指定格子（无动作）
        ///// </summary>
        ///// <param name="slot"></param>
        ///// <param name="weaponInfo"></param>
        ///// <returns>旧的武器信息</returns>
        //bool ReplaceWeaponToSlot(EWeaponSlotType slot, WeaponInfo weaponInfo,bool vertify = true);
        ///// <summary>
        ///// 丢弃指定位置的武器，不同的武器行为不同，手雷为抛出
        ///// </summary>
        //void DropSlotWeapon(EWeaponSlotType slot);
        ///// <summary>
        ///// 切换武器 
        ///// </summary>
        ///// <param name="slot"></param>
        //void SwitchIn(EWeaponSlotType slot);
        ///// <summary>
        ///// 移除武器, 打断当前动作，不锁定背包
        ///// </summary>
        ///// <param name="slot"></param>
        //void RemoveSlotWeapon(EWeaponSlotType slot);
        ///// <summary>
        ///// 移除武器，不打断当前动作
        ///// </summary>
        ///// <param name="slot"></param>
        //void RemoveSlotWeaponNoInterrupt(EWeaponSlotType slot);
        ///// <summary>
        ///// 消耗武器
        ///// </summary>
        //void OnCost(EWeaponSlotType slot);
   
        ///// <summary>
        ///// 被打断
        ///// </summary>
        //void Interrupt();
    }
}