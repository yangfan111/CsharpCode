using App.Shared.Util;
using Core;
using Core.Utils;
using System;
using System.Collections.Generic;
using Utils.Utils;

namespace App.Shared.GameModules.Weapon
{
    public partial class WeaponSlotsAux
    {
     
        private Dictionary<EWeaponSlotType, CommonSlotHandler> handlers= 
            new Dictionary<EWeaponSlotType, CommonSlotHandler>(CommonIntEnumEqualityComparer<EWeaponSlotType>.Instance);

        void OnDerivedTypeInstanceProcess(System.Type t)
        {
            var attributes = Attribute.GetCustomAttributes(t, false);
            WeaponSpeciesAttribute speciesAttr;
            foreach(Attribute attr  in attributes)
            {
                speciesAttr = attr as WeaponSpeciesAttribute;
                var handler = (CommonSlotHandler)Activator.CreateInstance(t);
                handler.SetSlotTarget(speciesAttr.slotType);
                handlers.Add(speciesAttr.slotType, handler);
            }
        }
        
        public WeaponSlotsAux()
        {
            CommonUtil.ProcessDerivedTypes(typeof(CommonSlotHandler), true, (System.Type t)=> OnDerivedTypeInstanceProcess(t));
       
        }

        public CommonSlotHandler FindHandler(EWeaponSlotType slot)
        {
#if UNITY_EDITOR
            CommonSlotHandler handler = null;
            var result = handlers.TryGetValue(slot, out handler);
            UnityEngine.Debug.Assert(result, "required slot dont exist in handler instance");
            return handler;
#endif
            return handlers[slot];
        }
         

    }
}