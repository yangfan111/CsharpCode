using System;

namespace Utils.Appearance
{
    public interface IPredictedBreathState
    {
        float SightHorizontalShift { get; set; }
        float SightVerticalShift { get; set; }
        float SightVerticalShiftRange { get; set; }
        int SightHorizontalShiftDirection { get; set; }
        int SightVerticalShiftDirection { get; set; }
        int SightRemainVerticalPeriodTime { get; set; }
        int RandomSeed { get; set; }
    }

    public interface IPredictedAppearanceState : IPredictedBreathState
    {
        float FirstPersonHeight { get; set; }
        float FirstPersonForwardOffset { get; set; }
    }

    public enum MagazineReloadState
    {
        None,
        StartReload,
        DropMagazine,
        AddMagazine,
        EndReload,
    }

    public enum ClientWeaponStateIndex
    {
        AlternativeWeaponLocator,
        AlternativeP3WeaponLocator,
        EndOfTheWorld
    }

    public enum PredictedWeaponStateIndex
    {
        WeaponInHand,
        ReloadState,
        EndOfTheWorld
    }

    public enum LatestWeaponStateIndex
    {
        EmptyHand,

        PrimaryWeaponOne,

        PrimaryWeaponOneMuzzle,
        PrimaryWeaponOneLowRail,
        PrimaryWeaponOneMagazine,
        PrimaryWeaponOneButtstock,
        PrimaryWeaponOneScope,

        PrimaryWeaponTwo,

        PrimaryWeaponTwoMuzzle,
        PrimaryWeaponTwoLowRail,
        PrimaryWeaponTwoMagazine,
        PrimaryWeaponTwoButtstock,
        PrimaryWeaponTwoScope,

        SideArm,

        SideArmMuzzle,
        SideArmLowRail,
        SideArmMagazine,
        SideArmButtstock,
        SideArmScope,

        MeleeWeapon,
        ThrownWeapon,
        TacticWeapon,

        EndOfTheWorld
    }

    public enum AppearanceFloatType
    {
        PeekDegree,
        FirstPersonHeight,

        EndOfTheWorld
    }
}
