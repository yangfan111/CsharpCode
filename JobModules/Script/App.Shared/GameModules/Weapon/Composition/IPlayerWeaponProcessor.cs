using App.Shared.Audio;
using Core;
using Core.EntityComponent;
using System;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="IPlayerWeaponSharedGetter" />
    /// </summary>
    public interface IPlayerWeaponProcessor : IPlayerWeaponSharedGetter
    {
        /// <summary>
        /// 带动作的切武器
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="includeAction"></param>
        void ArmWeapon(EWeaponSlotType slot, bool includeAction );

        void TryArmWeaponImmediately(EWeaponSlotType slot);

        EWeaponSlotType UnArmWeapon(bool includeAction, Action onfinish);
        EWeaponSlotType UnArmWeapon(bool includeAction);

        /// <summary>
        /// 武器掉落
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagIndex"></param>
        /// <returns></returns>
        bool DropWeapon(EWeaponSlotType slotType, int bagIndex);
        bool DropWeapon(EWeaponSlotType slotType = EWeaponSlotType.Pointer);
        /// <summary>
        /// 直接销毁当前槽位武器
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagIndex"></param>
        /// <param name="interrupt"></param>
        /// <returns></returns>
        bool DestroyWeapon(EWeaponSlotType slotType, int bagIndex, bool interrupt = true);
        /// <summary>
        /// 增加自动拾取判定
        /// </summary>
        /// <param name="orient"></param>
        /// <returns></returns>
        bool AutoPickUpWeapon(WeaponScanStruct orient);
        /// <summary>
        /// 捡武器接口，最终会调用ReplaceWeaponToSlot 
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="arm"></param>
        /// <returns></returns>
        bool PickUpWeapon(WeaponScanStruct orient, bool arm = true);

        /// <summary>
        /// 切换槽位,drawWeapon+逻辑判断
        /// </summary>
        /// <param name="in_slot"></param>
        void SwitchIn(EWeaponSlotType in_slot);
        /// <summary>
        /// 没有换动作版本
        /// </summary>
        /// <param name="in_slot"></param>
        void PureSwitchIn(EWeaponSlotType in_slot);
        /// <summary>
        /// 武器打击后消耗调用
        /// </summary>
        void AfterAttack();
        /// <summary>
        /// 后台直接替换槽位武器，老的武器entity会被销毁（手雷等特殊武器除外）
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagIndex"></param>
        /// <param name="orient"></param>
        /// <returns></returns>
        bool ReplaceWeaponToSlot(EWeaponSlotType slotType, int bagIndex, WeaponScanStruct orient);

        bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponScanStruct orient);
        /// <summary>
        /// 添加一只手雷并且在空手状态下尝试持有
        /// </summary>
        /// <param name="greandeId"></param>
        /// <returns></returns>
        bool TryHoldGrenade(int greandeId);
        /// <summary>
        /// 尝试从手雷库存中拿出手雷并且在空手状态下尝试持有
        /// </summary>
        void TryHoldGrenade(bool autoStuff= true,bool tryArm = true);
        /// <summary>
        /// 打断
        /// </summary>
        void Interrupt();

        void SwitchFireMode();
     //   void InterruptSight();

        void SetReservedBullet(int count);

        void SetReservedBullet(EWeaponSlotType slot, int count);

        int SetReservedBullet(EBulletCaliber caliber, int count);

        bool SetWeaponPart(EWeaponSlotType slot, int survivalCfgId);


        void DeleteWeaponPart(EWeaponSlotType slot, EWeaponPartType part);

        void InitBag(int pointer);
        /// <summary>
        /// TODO:
        /// </summary>
        /// <param name="player"></param>
        void SwitchBag();

        PlayerAudioController AudioController { get; }
        
    }
}
