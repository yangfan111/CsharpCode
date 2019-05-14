using App.Shared.Audio;
using Core.Utils;
using System.Collections.Generic;
using Core;
using UnityEngine;
using XmlConfig;

namespace App.Shared
{
    public enum AudioBank_LoadAction
    {
        DecodeOnLoad,
        DecodeOnLoadAndSave,
        Normal,
    }

    public enum AudioBnk_LoadTactics
    {
        LoadEntirely,
    }

    public enum AudioBank_LoadMode
    {
        Sync,
        Async,
        Prepare,
    }

    public enum AudioBank_LoadStage
    {
        Unload,
        Loaded,
        Loading,
    }

    //AudioGrp_FootstepState
    public enum AudioGrp_Footstep
    {
        None  = -1,
        Walk  = 0,
        Squat = 1,
        Crawl = 2,

        //unused
        Land = 3,
        Id   = 122,
    }

    public enum AudioGrp_HitMatType
    {
        None     = -1,
        Armor    = 0,
        Body     = 1,
        Helmet   = 4,
        Head     = 3,
        Concrete = 2,

        //unused
        Metal = 5,
        Water = 6,
        Wood  = 7,
        Id    = 148,
    }

    public enum AudioClientEffectType
    {
        BulletHit      = 1,
        BulletDrop     = 2,
        ThrowExplosion = 3,
    }

    public enum AudioGrp_Magazine
    {
        None            = -1,
        FillBulletOnly  = 0, //只填弹
        PullboltOnly    = 1, //只拉栓
        MagizineAndPull = 2, //换弹夹+拉栓
        MagizineOnly    = 3, //只换弹夹
        Id              = 144,
    }

    public enum TerrainMatOriginType
    {
        Default = 0,
        Dirt    = 1,
        Grass   = 2,
        Rock    = 3,
        Sand    = 4,
    }

    public enum AudioGrp_ShotMode
    {
        Single   = 0,
        Trriple  = 1,
        Continue = 2,
        Silencer = 3,
        Id       = 141,
    }

    public enum AuidoGrp_RefShotMode
    {
        Default =0,
        Silencer =1,
        Id = 145,
    }

    public enum AudioEnvironmentSourceType
    {
        UseBandage,
        UseAidKt,
        UseDrink,
        UseTablet,
        UseEpinephrine,
        UseGasoline,
        OpenParachute,
        OnGliding,
        OnParachute,
        GetDown,
        GetUp,
        ChangeWeapon,
        ChangeMode,
        OpenDoor,
        CloseDoor,
        Walk,  //footStep
        Squat, //footStep
        Crawl, //footStep
        WalkSwamp,
        SquatSwamp,
        DropWater,
        Swim,
        Dive,
        CrawlSwamp,
        Land, //footStep
        Length,
    }


    public enum AudioGrp_FootMatType
    {
        Default  = 0,
        Grass    = 0,
        Concrete = 1,
        Wood     = 2,
        Sand     = 3,
        Rock     = 4,
        Metal    = 5,
        Rug      = 6,
        Wetland  = 7,
        Id       = 121,
    }

    public enum AudioGrp_BulletType
    {
        Default = 0,
        Lv1     = 0,
        Lv2     = 1,
        Lv3     = 2,
        Id      = 142,
    }

    public enum AudioGrp_MeleeAttack
    {
        Default = 1,
        Hard    = 0,
        Soft    = 1,
        Id      = 143,
    }

    [System.Obsolete]
    public enum AudioAmbientEmitterType
    {
        ActionOnCustomEventType,
        UseCallback
    }

    [System.Obsolete]
    public enum AudioTriggerEventType
    {
        SceneLoad      = 1,
        ColliderEnter  = 2,
        CollisionExist = 3,
        MouseDown      = 4,
        MouseEnter     = 5,
        MouseExist     = 6,
        MouseUp        = 7,
        GunSimple      = 33,
        GunContinue    = 34,
        CarStar        = 35,
        CarStop        = 36,
        Default        = 99,
    }
}