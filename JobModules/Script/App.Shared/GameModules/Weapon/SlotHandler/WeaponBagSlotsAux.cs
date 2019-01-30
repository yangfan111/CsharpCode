using App.Shared.Util;
using Core;
using Core.Utils;
using System;
using System.Collections.Generic;

namespace App.Shared.GameModules.Weapon
{
    public partial class WeaponBagSlotsAux
    {
     
       private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponBagSlotsAux));
        private Dictionary<EWeaponSlotType, WeaponSlotHandlerBase> handlers= 
            new Dictionary<EWeaponSlotType, WeaponSlotHandlerBase>();


        void OnDerivedTypeInstanceProcess(System.Type t)
        {
            var attributes = Attribute.GetCustomAttributes(t, false);
            WeaponSpeciesAttribute speciesAttr;
            foreach(Attribute attr  in attributes)
            {
                speciesAttr = attr as WeaponSpeciesAttribute;
                var handler = (WeaponSlotHandlerBase)Activator.CreateInstance(t);
                handler.SetSlotTarget(speciesAttr.slotType);
                handlers.Add(speciesAttr.slotType, handler);
            }
        }
        
        public WeaponBagSlotsAux()
        {
            CommonUtil.ProcessDerivedTypes(typeof(WeaponSlotHandlerBase), true, OnDerivedTypeInstanceProcess);
        }

        public WeaponSlotHandlerBase FindHandler(EWeaponSlotType slot)
        {
#if UNITY_EDITOR
            WeaponSlotHandlerBase handler = null;
            UnityEngine.Debug.Assert(handlers.TryGetValue(slot, out handler), "required slot dont exist in handler instance");
            return handler;
#endif
            return handlers[slot];
        }
         

    }
}