using App.Shared.Components.Bag;
using App.Shared.Util;
using Core;
using Core.Bag;
using Core.GameModeLogic;
using Core.Sound;
using Core.Utils;
using System.Collections.Generic;

namespace App.Shared.WeaponLogic
{
    [WeaponSpecies(EWeaponSlotType.GrenadeWeapon)]
    public class GrenadeSlotHandler : WeaponSlotBaseHandler
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(GrenadeSlotHandler));
        private IGrenadeBagCacheAgent _cacheAgent;
        private PlayerEntity _playerEntity;

        public GrenadeSlotHandler()
         
        {
            //_cacheAgent = playerEntity.grenadeInventoryHolder.Inventory;
            //_playerEntity = playerEntity;
        }

        public override void RecordLastWeaponId(int lastId)
        {
            base.RecordLastWeaponId(lastId);
            _cacheAgent.CacheLastGrenade(lastId);
        }
      
        //尝试更换手雷或拿出手雷操作
        public override void SwitchIn(EWeaponSlotType from, WeaponBag_SlotSwitchCallback onSwitchCallback)
        {
            if (from == handledSlot)
            {
                Err_WeaponLogicErrCode ret = IsGreandePull();
                if (ret != Err_WeaponLogicErrCode.Sucess)
                {
                    _playerEntity.tip.TipType = Core.Common.ETipType.CantSwithGrenade;
                    return;
                    //return ret;
                }
                int nextGrenadeId = _cacheAgent.PickNextGrenadeSpecies();
                if (nextGrenadeId < 1) return;//Err_WeaponLogicErrCode.Err_GrenadeNotFindNextUsableId;
                onSwitchCallback(handledSlot, nextGrenadeId);
            }
            else
            {
                onSwitchCallback(handledSlot, -1);
            }
        }
        public override int PickNextId(bool differentSpecies)
        {
            if (differentSpecies)
                return -1;
            return _cacheAgent.PickNextGrenadeInstance();

        }
        private Err_WeaponLogicErrCode IsGreandePull()
        {
            if (!_playerEntity.hasThrowingAction)
                return Err_WeaponLogicErrCode.Err_PlayerDontHaveThrowingComponent;
            var pull = _playerEntity.throwingAction.ActionInfo.IsPull;
            var destroy = _playerEntity.throwingAction.ActionInfo.IsInterrupt;
            var fly = _playerEntity.throwingUpdate.IsStartFly;
            bool ret = (pull && !destroy && !fly);
            return ret ? Err_WeaponLogicErrCode.Sucess : Err_WeaponLogicErrCode.Err_PlayerGrenadePullVertifyFailed;
        }

        private Err_WeaponLogicErrCode TryVarySlotGrenadeObject(System.Action<EWeaponSlotType, int> onSameSpeciesSwitch)
        {
            Err_WeaponLogicErrCode ret = IsGreandePull();
            if (ret != Err_WeaponLogicErrCode.Sucess)
            {
                _playerEntity.tip.TipType = Core.Common.ETipType.CantSwithGrenade;
                return ret;
            }
            int nextGrenadeId = _cacheAgent.PickNextGrenadeSpecies();
            if (nextGrenadeId < 1) return Err_WeaponLogicErrCode.Err_GrenadeNotFindNextUsableId;
            if (onSameSpeciesSwitch != null)
                onSameSpeciesSwitch(EWeaponSlotType.GrenadeWeapon, nextGrenadeId);
            return Err_WeaponLogicErrCode.Sucess;
        }
        //for (var i = 0; i < _grenadelist.Count; i++)
        //{
        //    if(_grenadelist[i] == _weapon.Id)
        //    {
        //        var next = _grenadelist[(i + 1) % _grenadelist.Count];
        //        if(next != _weapon.Id)
        //        {
        //            if (!_grenadeInventory.Remove(next))
        //            {
        //                return false;
        //            }
        //            MountGrenade(next);
        //            return true;
        //        }
        //    }
        //}

        //return false;

        private void SwitchToGrenade()
        {

        }

        //private void MountGrenade(int id)
        //{
        //    _playerWeaponActionLogic.ReplaceWeaponToSlot(EWeaponSlotType.GrenadeWeapon, new WeaponInfo
        //    {
        //        Id = id,
        //    });
        //    _playerWeaponActionLogic.TryMountWeapon(EWeaponSlotType.GrenadeWeapon);
        //}
        public override void OnSpend(PlayerWeaponComponentAgent weaponAgent, WeaponBag_WeaponSpendCallback spendCallback)
        {

            if (_cacheAgent.RemoveCache(weaponAgent.HeldSlotWeaponId))
            {

                if (spendCallback != null)
                {
                    var paramsData = new WeaponSpendCallbackData(handledSlot, true, true);
                    spendCallback(paramsData);
                }

            }



            //Err_WeaponLogicErrCode errCode = TryVarySlotGrenadeObject();
            //if(errCode != Err_WeaponLogicErrCode.Sucess)
            //        _playerWeaponActionLogic.RemoveWeaponNoInterrupt(EWeaponSlotType.GrenadeWeapon);

        }
    }
}
