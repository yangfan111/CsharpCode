using Assets.XmlConfig;
using System;

namespace Core
{
    public enum EWeaponSlotType
    {
        None = 0,
        PrimeWeapon=1,
        SecondaryWeapon=2,
        PistolWeapon=3,
        MeleeWeapon=4,
        ThrowingWeapon=5,
        TacticWeapon=6,
        Length,
        Pointer = 99,
        LastPointer = 100,
    }

  

 
    public enum EWeaponSlotsGroupType
    {
        Default,
        Group,
    }

    public enum EGameMode
    {
        Normal = 1,
        Survival = 2,
    }

}
