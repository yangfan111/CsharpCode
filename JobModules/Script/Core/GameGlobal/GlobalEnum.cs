using Assets.XmlConfig;
using System;

namespace Core
{
    public enum EPlayerStateCollectType
    {
        UseCache,
        UseMoment,
        UseCacheAddation,
    }

    public enum EInterruptCmdType
    {
        InterruptSimple,
        InterruptAndRollback,
        None,
    }
    public enum EWeaponUpdateCmdType
    {
        None,
        UpdateHoldAppearance,
        ExchangePrimaryAppearance,
    }
    public enum EInterruptType
    {
        GunSight,   //开镜
        HoldWeapon, //收枪
        Count,
    }

    public enum EMeleeAttackType
    {
        Hard = 0,
        Soft = 1,
    }

    public enum EInterruptState
    {
        WaitInterrupt = 0,
        WaitRecover   = 2,
        Closed        = 5,
    }

    public enum EAudioUniqueId
    {
        None          = 0,
        Footstep      = 1,
        JumpStep      = 2,
        FireMode      = 13,
        SightOpen     = 17,
        SightClose    = 16,
        WeaponIn      = 14,
        WeaponOff     = 15,
        Prone         = 3,
        ProneToStand  = 4,
        Crouch        = 5,
        CrouchToStand = 6,

        OpenDoor = 5021,
        CloseDoor =5020,
                                                                        

        EmptyFire = 3036,

        BulletFly          = 3038,
        BulletHit          = 3039,
        GrenadeThrow       = 4011,
        GrenadeTrigger     = 4012,
        FlashBombExplosion = 4007,
        GrenadeExplosion   = 4008,
        OceanEnvironment   = 5031,

        PikcupWeapon         = 5011,
        PickupWeaponPart     = 5006,
        PicupMagazinePart    = 5009,
        PickupSightPart      = 5007,
        PickupBullet         = 5002,
        PickupEngeryDrink    = 5004,
        PickupPill           = 5005, //止疼药
        PickupDoping         = 5008, //肾上腺素
        PickupBandage        = 5000,
        PickupAidPackage     = 5055, //急救包
        PickupMedicalPackage = 5010, //医疗包
        PickupGasoline       = 5001, //汽油桶
        PickupCloth          = 5003, //捡衣服
    }

    public enum EItemAudioType
    {
        Bullet         = 1,
        EngeryDrink    = 2,
        Pill           = 3, //止疼药
        Doping         = 4, //肾上腺素
        Bandage        = 5,
        AidPackage     = 6, //急救包
        MedicalPackage = 7, //医疗包
        Gasoline       = 8, //汽油桶
    }

    public enum EWeaponSlotType
    {
        None            = 0,
        PrimeWeapon     = 1,
        SecondaryWeapon = 2,
        PistolWeapon    = 3,
        MeleeWeapon     = 4,
        ThrowingWeapon  = 5,
        TacticWeapon    = 6,
        Length,
        Pointer     = 99,
        LastPointer = 100,
    }

//    public enum EPlayerActionType
//    {
//       // Run, //快跑
//        UIOpen,
//        UIClose,
//        Pickup,
//        Drop,
//        Climp,
//        SwitchToWalk,
//        SwitchToSquat , //蹲
//        Prone , //爬
//        PullBolt,
//        Reload,
//        Drive,
//        SwitchWeapon,
//        CornerWeaponAction, //靠墙收枪
//        Swim,
//        BuryOrOpenC4, //埋，拆包
////        SwitchPersonalViewSight,
////        SwitchWeaponFireMode,
//        
//    }


    public enum EWeaponSlotsGroupType
    {
        Default,
        Group,
    }

    public enum EGameMode
    {
        Normal   = 1,
        Survival = 2,
    }
}