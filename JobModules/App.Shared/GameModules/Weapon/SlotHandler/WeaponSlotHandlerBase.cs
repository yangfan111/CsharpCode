using Core;
using Core.Utils;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// 4 location : ground body hand pacakge
    /// </summary>
    [WeaponSpecies(EWeaponSlotType.PrimeWeapon1)]
    [WeaponSpecies(EWeaponSlotType.PrimeWeapon2)]
    [WeaponSpecies(EWeaponSlotType.SubWeapon)]

    public class WeaponSlotHandlerBase
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponSlotHandlerBase));
        protected int lastSlotWeaponId;
        protected EWeaponSlotType handledSlot;
        public IBagDataCacheHelper Helper { get; private set; }
        internal void SetSlotTarget(EWeaponSlotType slot)
        {
            handledSlot = slot;
        }
        public virtual void SetHelper(IBagDataCacheHelper in_helper)
        {
            Helper = in_helper;
        }
        public virtual bool HasBagData { get { return false; } }
        internal virtual void OnExpend(PlayerWeaponComponentAgent agent, WeaponSlotExpendCallback expendCb)
        {
            var bullet = agent.CurrWeaponBullet;
            agent.SetSlotWeaponBullet(bullet - 1);
            if (expendCb != null)
            {
                var paramsData = new WeaponSlotExpendData(handledSlot,false, false);
                expendCb(paramsData);
            }

        }
  
  
        /// <summary>
        /// 装备槽位填充完成
        /// </summary>
        /// <returns></returns>
        internal virtual void RecordLastWeaponId(int lastId)
        {
            lastSlotWeaponId = lastId;
        }
        internal virtual void Recycle() { }
        //选择下一个可装备的武器id
        internal virtual int PickNextId(bool differentSpecies)
        {
            return -1;
        }
        /// <summary>
        /// weapon from body, hand to ground
        /// </summary>
        internal virtual void OnDrop()
        {
        }

   
    }
}
