using Core;
using System.Collections.Generic;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class BlastWeaponStateUiAdapter : WeaponStateUiAdapter
    {
        private Contexts _contexts;

        public BlastWeaponStateUiAdapter(Contexts contexts) : base(contexts)
        {
            _contexts = contexts;
        }

        protected override EWeaponSlotType Index2Slot(int index)
        {
            if (index == 0)
            {
                return EWeaponSlotType.None;
            }
            var mappingIndex = SlotIndexList[index - 1];
            return base.Index2Slot(mappingIndex);
        }

        protected override int Slot2Index(EWeaponSlotType slot)
        {
            var mappingIndex = base.Slot2Index(slot);
            if (mappingIndex == 0) return 0;
            var list = SlotIndexList;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == mappingIndex)
                {
                    return i + 1;
                }
            }

            return 0;
        }

        public override List<int> SlotIndexList
        {
            get { return new List<int> { 6, 1, 3, 4, 5 }; }
        }

    }
}
