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
   

    public enum EInterruptType
    {
        GunSight,   //开镜
        HoldWeapon, //收枪
        Count,

    }

    public enum EInterruptState
    {
        WaitInterrupt=0,
        WaitRecover =2,
        Closed=5,

        
        
    }

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
        Normal = 1,
        Survival = 2,
    }

}
