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
    public partial class WeaponBagSlotsAux
    {
     
       private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponBagSlotsAux));
        private Dictionary<EWeaponSlotType, WeaponSlotBaseHandler> handlers= 
            new Dictionary<EWeaponSlotType, WeaponSlotBaseHandler>();


        void OnDerivedTypeInstanceProcess(System.Type t)
        {
            var attributes = Attribute.GetCustomAttributes(t, false);
            WeaponSpeciesAttribute speciesAttr;
            foreach(Attribute attr  in attributes)
            {
                speciesAttr = attr as WeaponSpeciesAttribute;
                var handler = (WeaponSlotBaseHandler)Activator.CreateInstance(t);
                handler.SetSlotTarget(speciesAttr.slotType);
                handlers.Add(speciesAttr.slotType, handler);
            }
        }
        
        public WeaponBagSlotsAux()
        {
            CommonUtil.ProcessDerivedTypes(typeof(WeaponSlotBaseHandler), true, OnDerivedTypeInstanceProcess);
        }

        public WeaponSlotBaseHandler FindHandler(EWeaponSlotType slot)
        {
#if UNITY_EDITOR
            WeaponSlotBaseHandler handler = null;
            UnityEngine.Debug.Assert(handlers.TryGetValue(slot, out handler), "required slot dont exist in handler instance");
            return handler;
#endif
            return handlers[slot];
        }
         

    }
}