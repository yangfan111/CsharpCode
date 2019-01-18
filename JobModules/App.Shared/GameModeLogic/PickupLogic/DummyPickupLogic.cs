using Core;
using Core.GameModeLogic;

namespace App.Shared.GameModeLogic.PickupLogic
{
    public class DummyPickupLogic : IPickupLogic
    {
        public DummyPickupLogic()
        {
            
        }

        public virtual void SendAutoPickupWeapon(int entityId)
        {
            //DO NOTHING
        }

        public virtual void DoPickup(int playerEntityId, int itemEntityId)
        {
            //DO NOTHING
        }

        public virtual void Dorp(int playerEntityId, EWeaponSlotType slot)
        {
            //DO NOTHING
        }

        public virtual void SendPickup(int entityId, int itemId, int category, int count)
        {
            //DO NOTHING
        }

        public virtual void AutoPickupWeapon(int playerEntityId, int entityId)
        {
            //DO NOTHING
        }
    }
}
