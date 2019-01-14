using Core.Bag;

namespace App.Shared.WeaponLogic.SlotController
{
    public class GroupWeaponSlotController : DefaultWeaponSlotController
    {
        private readonly EWeaponSlotType[] _slots = new EWeaponSlotType[]
        {
            EWeaponSlotType.PrimeWeapon1,
            EWeaponSlotType.SubWeapon,
            EWeaponSlotType.GrenadeWeapon,
            EWeaponSlotType.MeleeWeapon,
            EWeaponSlotType.TacticWeapon
        };
        public override EWeaponSlotType[] AvaliableSlots
        {
            get
            {
                return _slots;
            }
        }
    }
}
