using Core;
using Core.GameModeLogic;
using Entitas;

namespace App.Shared.GameModeLogic.WeaponActionListener
{
    public class DummyWeaponActionListener : IWeaponProcessListener
    {
        public void OnExpend(Entity playerEntity, EWeaponSlotType slot)
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
