using App.Shared.Util;
using Core;
using Core.Utils;
using System;
using System.Collections.Generic;

namespace App.Shared.GameModules.Weapon
{
    public partial class WeaponSlotsAux
    {
     
       private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponSlotsAux));
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
        
        public WeaponSlotsAux()
        {
            CommonUtil.ProcessDerivedTypes(typeof(WeaponSlotHandlerBase), true, OnDerivedTypeInstanceProcess);
        }

        public WeaponSlotHandlerBase FindHandler(EWeaponSlotType slot)
        {
#if UNITY_EDITOR
            WeaponSlotHandlerBase handler = null;
            var result = handlers.TryGetValue(slot, out handler);
            UnityEngine.Debug.Assert(result, "required slot dont exist in handler instance");
            return handler;
#endif
            return handlers[slot];
        }
         

    }
}