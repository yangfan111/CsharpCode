using Core.Bag;
using Core.Utils;
using App.Shared.Components.Bag;
using Core.GameModeLogic;
using Core.Sound;
using Core;
using App.Shared.Util;

namespace App.Shared.WeaponLogic
{
    /// <summary>
    /// 4 location : ground body hand pacakge
    /// </summary>
    [WeaponSpecies(EWeaponSlotType.PrimeWeapon1)]
    [WeaponSpecies(EWeaponSlotType.PrimeWeapon2)]
    [WeaponSpecies(EWeaponSlotType.SubWeapon)]

    public class WeaponSlotBaseHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponSlotBaseHandler));
        protected int lastSlotWeaponId;
        protected EWeaponSlotType handledSlot;
        public void SetSlotTarget(EWeaponSlotType slot)
        {
            handledSlot = slot;
        }
        
        public virtual void OnSpend(PlayerWeaponComponentAgent bagLogic, WeaponBag_WeaponSpendCallback spendCallback)
        {
            var bullet = bagLogic.HeldWeaponBullet;
            bagLogic.SetHeldWeaponBullet(bullet - 1);
            if (spendCallback != null)
            {
                var paramsData = new WeaponSpendCallbackData(handledSlot,false, false);
                spendCallback(paramsData);
            }

        }
        public virtual void StuffEnd()
        {

        }
        /// <summary>
        /// 装备槽位填充完成
        /// </summary>
        /// <returns></returns>
        public virtual void RecordLastWeaponId(int lastId)
        {
            lastSlotWeaponId = lastId;
        }
        //选择下一个可装备的武器id
        public virtual int PickNextId(bool differentSpecies)
        {
            return -1;
        }
        /// <summary>
        /// weapon from body, hand to ground
        /// </summary>
        public virtual void OnDrop()
        {
        }

        /// <summary>
        /// wewapon from body to hand
        /// </summary>
        public virtual void SwitchIn(EWeaponSlotType from,  WeaponBag_SlotSwitchCallback onSwitchCallback)
        {
            // 如果要切换到的位置和当前位置一致不作处理

            if (handledSlot != from)
            {
                // 如果切换到的位置有枪，设置当前武器
                onSwitchCallback(handledSlot, -1);
            }
        
     
        }
    }
}
