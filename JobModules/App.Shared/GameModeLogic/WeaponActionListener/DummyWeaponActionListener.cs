using Core.Bag;
using Core.GameModeLogic;
using Entitas;

namespace App.Shared.GameModeLogic.WeaponActionListener
{
    public class DummyWeaponActionListener : IWeaponActionListener
    {
        public void OnCost(Entity playerEntity, EWeaponSlotType slot)
        {
            //DO NOTHING
        }

        public void OnDrop(Entity playerEntity, EWeaponSlotType slot)
        {
            //DO NOTHING
        }

        public void OnPickup(Entity playerEntity, EWeaponSlotType slot)
        {
            //DO NOTHING
        }
    }
}
