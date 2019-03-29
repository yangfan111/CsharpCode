using Core;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="WeaponSlotsLibrary" />
    /// </summary>
    public class WeaponSlotsLibrary : IWeaponSlotsLibrary
    {
        private static readonly EWeaponSlotType[] DefaultSlotIndices = new EWeaponSlotType[]
{
            EWeaponSlotType.PrimeWeapon,
            EWeaponSlotType.SecondaryWeapon,
            EWeaponSlotType.PistolWeapon,
            EWeaponSlotType.MeleeWeapon,
            EWeaponSlotType.ThrowingWeapon,
};

        private static readonly EWeaponSlotType[] GroupSlotIndices = new EWeaponSlotType[]
{
            EWeaponSlotType.PrimeWeapon,
            EWeaponSlotType.PistolWeapon,
            EWeaponSlotType.ThrowingWeapon,
            EWeaponSlotType.MeleeWeapon,
            EWeaponSlotType.TacticWeapon
};

        public static WeaponSlotsLibrary Allocate(EWeaponSlotsGroupType groupType)
        {
            //EWeaponSlotType[] indices = null;
            switch (groupType)
            {
                case EWeaponSlotsGroupType.Default:
                    return new WeaponSlotsLibrary(DefaultSlotIndices);
                default:
                    return new WeaponSlotsLibrary(GroupSlotIndices);
            }
        }

        private EWeaponSlotType[] availableIndices;

        public EWeaponSlotType[] AvaliableSlots
        {
            get { return availableIndices; }
        }

        public WeaponSlotsLibrary(EWeaponSlotType[] in_indices)
        {
            availableIndices = in_indices;
        }

        public bool IsSlotValid(EWeaponSlotType slot)
        {
            for (int i = 0; i < availableIndices.Length; i++)
            {
                if (slot == availableIndices[i])
                {
                    return true;
                }
            }
            return false;
        }

        public EWeaponSlotType GetWeaponSlotByIndex(int index)
        {
            if (index > availableIndices.Length - 1 || index < 0)
            {
                return EWeaponSlotType.None;
            }
            return availableIndices[index];
        }
    }
}
