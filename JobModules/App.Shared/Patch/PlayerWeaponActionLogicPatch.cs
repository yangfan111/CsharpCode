using Core.Bag;
using Core;
using Assets.Utils.Configuration;

using Utils.Singleton;
using Assets.XmlConfig;

namespace App.Shared.Player
{


    /// <summary>
    /// 装备到格子
    /// 添加武器逻辑分手雷和普通流程处理 
    /// </summary>
    public partial class PlayerWeaponActionLogic : IPlayerWeaponActionLogic
    {

        public WeaponInfo PickUpWeapon(WeaponInfo weaponInfo)
        {
            var weapon = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponInfo.Id);
            if (weapon == null) return WeaponInfo.Empty;
            var slotType = GetMatchSlot((EWeaponType)weapon.Type);
            if (slotType != EWeaponSlotType.None)
            {
                _weaponActionListener.OnPickup(_playerEntity, slotType);
                //除去手雷已经填充到当前槽位的情况
                if (FilterGrenadeStuffedCond(slotType))
                {
                    WeaponInfo last;
                    if(ReplaceWeaponToSlot(slotType, weaponInfo,out last))
                    {
                        TryMountWeapon(slotType);
                        return last;
                    }
                    
                }
            }
            Logger.Error("Illegal Slot type for weapon ");
            return WeaponInfo.Empty;
        }
     
        public void OnCost(EWeaponSlotType slot)
        {
            if (slot == EWeaponSlotType.None)
            {
                return;
            }
            _weaponActionListener.OnCost(_playerEntity, slot);
            _slotAuxiliary.OnWpCost(slot, OnCurrSlotRestuff);
            //var locationController = _locationController.GetLocationController(slot); 
            //if(null != locationController)
            //{
            //    locationController.OnCost();
            //}
        }
        /// <summary>
        /// 武器发射完自动填充逻辑
        /// </summary>
        /// <param name="slot"></param>
        private void OnCurrSlotRestuff(EWeaponSlotType slot)
        {
            if (!_weaponBagLogic.IsWeaponSlotStuffed(slot))
            {
                int nextId = _slotAuxiliary.PickNextInstance(slot);
                if(nextId>0)
                {
                    WeaponInfo last;
                    if (ReplaceWeaponToSlot(slot,new WeaponInfo() { Id = nextId}, out last))
                    {
                        TryMountWeapon(slot);
                    }
                }
            }
        }
        /// <summary>
        /// 当前槽位同种武器切换逻辑
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="nextWeaponId"></param>
        private void OnSameSpeciesSwitch(EWeaponSlotType slotType,int nextWeaponId)
        {
            if(ReplaceWeaponToSlot(EWeaponSlotType.GrenadeWeapon, new WeaponInfo
            {
                Id = nextWeaponId,
            }))
            {
                TryMountWeapon(EWeaponSlotType.GrenadeWeapon);

            }
        }
        /// <summary>
        /// 不处理手雷已装备情况
        /// </summary>
        /// <param name="slotType"></param>
        /// <returns></returns>
        private bool FilterGrenadeStuffedCond(EWeaponSlotType slotType)
        {
            return slotType != EWeaponSlotType.GrenadeWeapon ||
               !_weaponBagLogic.IsWeaponSlotStuffed(slotType);

        }

        public void SwitchIn(EWeaponSlotType in_slot)
        {
            if (in_slot == EWeaponSlotType.None)
            {
                Logger.Error("swithc in weapon in slot none");
                return;
            }
            //var locationController = _locationController.GetLocationController(slot);
            var from_slot = _weaponBagLogic.GetCurrentWeaponSlot();
            var errCode = _slotAuxiliary.OnWpSwitchIn(in_slot, from_slot, OnSameSpeciesSwitch);
            if (errCode == Util.Err_WeaponLogicErrCode.Sucess)
            {
                _playerEntity.weaponLogic.State.OnSwitchWeapon();
            }

            //   _playerEntity.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.ChangeWeapon);
        }

        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo, bool vertify = true)
        {
             WeaponInfo lastWp;
            return ReplaceWeaponToSlot(slotType, weaponInfo, vertify, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo)
        {
            WeaponInfo lastWp;
            return ReplaceWeaponToSlot(slotType, weaponInfo, true, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo,out WeaponInfo lastWp)
        {
            return ReplaceWeaponToSlot(slotType, weaponInfo, true, out lastWp);
            //  return lastWp;
        }
        public bool ReplaceWeaponToSlot(EWeaponSlotType slotType, WeaponInfo weaponInfo, bool vertify, out WeaponInfo lastWp)
        {
            lastWp = WeaponInfo.Empty;
            if (vertify)
            {
                if (slotType == EWeaponSlotType.None) return false;
                var weaonCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponInfo.Id);
                if (weaonCfg == null)
                    return false;
            }
            var errCode = _weaponBagLogic.AddWeaponToSlot(slotType, weaponInfo, out lastWp);
            if (errCode == Util.Err_WeaponLogicErrCode.Sucess)
            {
                _slotAuxiliary.OnWpStuffEnd(slotType, lastWp.Id);
                return true;

            }
            else
            {
                throw new System.Exception("Add weapon to slot failed");


            }
            return false;
        }
    }

}


