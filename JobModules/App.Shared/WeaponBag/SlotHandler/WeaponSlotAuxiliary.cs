using App.Shared.Util;
using Core;
using Core.Bag;
using Core.Common;
using Core.GameModeLogic;
using Core.Utils;
using System;
using System.Collections.Generic;
using Utils.Utils;

namespace App.Shared.WeaponLogic
{
    public partial class WeaponSlotAuxiliary
    {
       public  delegate Err_WeaponLogicErrCode BagWeaponControllerCallback(WeaponSlotBaseHandler contrller);
       private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponSlotAuxiliary));
        private Dictionary<EWeaponSlotType, WeaponSlotBaseHandler> handlers= 
            new Dictionary<EWeaponSlotType, WeaponSlotBaseHandler>();


        void OnDerivedTypeInstanceProcess(System.Type t,System.Object instance)
        {
            var attributes = Attribute.GetCustomAttributes(t, false);
            WeaponSpeciesAttribute speciesAttr;
            foreach(Attribute attr  in attributes)
            {
                speciesAttr = attr as WeaponSpeciesAttribute;
                handlers.Add(speciesAttr.slotType, instance as WeaponSlotBaseHandler);
            }
        }
        public WeaponSlotAuxiliary(
            PlayerEntity player, 
            IWeaponSlotController weaponSlotController,
            IPlayerWeaponActionLogic playerWeaponActionLogic,
            IReservedBulletLogic reservedBulletController,
            IGrenadeBagCacheAgent grenadeInventory)
        {
            Util.CommonUtil.ProcessDerivedTypeInstances(typeof(WeaponSlotBaseHandler), true, OnDerivedTypeInstanceProcess);
            //var slots = weaponSlotController.AvaliableSlots;
            //for(int i = 0; i < slots.Length; i++)
            //{
            //    var slot = slots[i];
            //    var weaponComponent = player.GetWeaponComponentBySlot(slot);
            //    var soundManager = player.soundManager.Value;
            //    switch(slot)
            //    {
            //        case EWeaponSlotType.GrenadeWeapon:
            //            _controllerDic[slot] = new GrenadeSlotHandler(
            //                player,
            //                playerWeaponActionLogic,
            //                weaponComponent,
            //                reservedBulletController,
            //                soundManager);
            //            break;
            //        case EWeaponSlotType.MeleeWeapon:
            //            _controllerDic[slot] = new MeleeSlotHandler(
            //                player,
            //                playerWeaponActionLogic,
            //                weaponComponent,
            //                reservedBulletController,
            //                soundManager);
            //            break;
            //        case EWeaponSlotType.TacticWeapon:
            //            _controllerDic[slot] = new TacticSlotHandler(
            //                player,
            //                playerWeaponActionLogic,
            //                weaponComponent,
            //                reservedBulletController,
            //                soundManager);
            //            break;
            //        default:
            //            _controllerDic[slot] = new WeaponSlotBaseHandler(
            //                player,
            //                playerWeaponActionLogic,
            //                weaponComponent,
            //                reservedBulletController,
            //                soundManager);
            //            break;
            //    }
            //}
        }

        public WeaponSlotBaseHandler GetLocationController(EWeaponSlotType slot)
        {
            if(!handlers.ContainsKey(slot))
            {
                Logger.ErrorFormat("location controller in slot {0} is null", slot);
                return null;
            }
            return handlers[slot];
        }
         

    }
}