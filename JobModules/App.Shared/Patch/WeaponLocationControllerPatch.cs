using App.Shared.Util;
using App.Shared.Player;
using App.Shared.Util;
using Core;
using Core.Bag;
using Core.Common;
using Core.GameModeLogic;
using Core.Utils;
using System.Collections.Generic;
using Utils.Utils;

namespace App.Shared.WeaponLogic
{
    public partial class WeaponSlotAuxiliary
    {
        public Err_WeaponLogicErrCode OnWpSwitchIn(EWeaponSlotType in_slot, EWeaponSlotType from_slot,System.Action<EWeaponSlotType,int> onSameSpeciesSwitch)
        {
            return OnWpProcess(in_slot, (WeaponSlotBaseHandler controller) => { return controller.OnSwitchIn(from_slot, onSameSpeciesSwitch); });
        }
        public Err_WeaponLogicErrCode OnWpDrop(EWeaponSlotType in_slot)
        {
            return OnWpProcess(in_slot, (WeaponSlotBaseHandler controller) =>
            {
                return controller.OnDrop();
            });
        }
        public void OnWpCost(EWeaponSlotType in_slot,System.Action<EWeaponSlotType> onRestuff)
        {
             OnWpProcess(in_slot, (WeaponSlotBaseHandler controller) => {  controller.OnCost(onRestuff);return Err_WeaponLogicErrCode.Sucess; });
        }
        public Err_WeaponLogicErrCode OnWpStuffEnd(EWeaponSlotType in_slot,int lastWpId)
        {
            return OnWpProcess(in_slot, (WeaponSlotBaseHandler controller) => { return controller.OnStuffEnd(lastWpId); });
        }
        public Err_WeaponLogicErrCode OnWpProcess(EWeaponSlotType in_slot, BagWeaponControllerCallback process)
        {

            WeaponSlotBaseHandler controller;
            if (handlers.TryGetValue(in_slot, out controller))
            {
                return process(controller);
            }
            Logger.ErrorFormat("location controller in slot {0} is null", in_slot);
            return Err_WeaponLogicErrCode.Err_Default;


        }
        public int PickNextInstance(EWeaponSlotType in_slot)
        {
            WeaponSlotBaseHandler controller;
            if (handlers.TryGetValue(in_slot, out controller))
            {
                return controller.PickNextInstance();
            }
            return -1;
        }
    }
}