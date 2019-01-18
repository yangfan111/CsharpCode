using Core;
using Core.Utils;

namespace App.Shared.GameModules.Weapon
{
    [WeaponSpecies(EWeaponSlotType.GrenadeWeapon)]
    internal class GrenadeSlotHandler : WeaponSlotHandlerBase
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(GrenadeSlotHandler));
        private GrenadeBagCacheHelper bagCacheHelper;
        //private PlayerEntity _playerEntity;
        public override bool HasBagData { get { return true; } }
        internal override void RecordLastWeaponId(int lastId)
        {
            base.RecordLastWeaponId(lastId);
            bagCacheHelper.CacheLastGrenade(lastId);
        }

        //尝试更换手雷或拿出手雷操作

        internal override int PickNextId(bool differentSpecies)
        {
            if (differentSpecies)
                return bagCacheHelper.PickupNextManually(lastSlotWeaponId);
            return bagCacheHelper.PickupNextAutomatic(lastSlotWeaponId);

        }
        public override void SetHelper(IBagDataCacheHelper in_helper)
        {
            base.SetHelper(in_helper);
            bagCacheHelper = (GrenadeBagCacheHelper)in_helper;
        }
        internal override void OnExpend(PlayerWeaponComponentAgent agent, WeaponSlotExpendCallback slotExpendCb)
        {

            bagCacheHelper.RemoveCache(agent.CurrSlotWeaponId);
            if (slotExpendCb != null)
            {
                var paramsData = new WeaponSlotExpendData(handledSlot, true, true);
                slotExpendCb(paramsData);
            }

        }
    }
}
