using Assets.XmlConfig;
using System;
using UnityEngine;
using XmlConfig;

namespace Core
{
    /// <summary>
    /// Defines the <see cref="GlobalConst" />
    /// </summary>
    public partial class GlobalConst
    {
        public const int WeaponBagMaxCount = 1;

        public static readonly int WeaponSlotMaxLength = (int) EWeaponSlotType.Length;

        public const int WeaponEntityType = 11;

        public const int AudioEvt_Footstep = 1;

        public const int AudioEvt_JumpStep = 2;

        //public const float CrouchSpreadFactor = 0.8f;
        public const int Length2D1 = 350;

        public const int Length2D2 = 25;
   
        public static float WeaponSpreadPerOffset = 1000f;

        public const int AudioEvt_BulletHit = 3039;
        public const int AudioEvt_BulletFly = 3038;
     

        public static float        FireSpreadDuration  = 0.4f;
        public static bool         EnableInputBlockLog = false;
        public static EPlayerInput serachedInput;
        public static bool         EnableStateLog = false;
        public static bool         EnableAssetLog = false;


        public static bool EnableWeaponLog = false;

        //RTPC
        public static float ModelCarEngine        = 0f;
        public static float NormalAttenuation     = 1f;
        public static float DifferentPlayerEffectVal = 0f;
        public static float PlayerCamera          = 0f;
        public static float GunMagazineSpeed      = 0f;


        public static uint  testBusId        = 0;
        public static float testFValue       = 0f;
        public static bool  testValueChanged = false;

        public static int autoMoveSignal= 0;
        public const float ServerShootStatisticsMaxDeltaTime= 1000;

        public static float MotionSpreadDuration = 0.1f;
        public static sbyte MaxAudioExistTimeSec = 10;
        public static float MaxLoadAsyncWaitTime = 10;

        public static sbyte DefaultEffectExistTimeSec = 10;
        public const int AimDefaultFov = 80;

        public static readonly int[] SilencerWeapons = new int[] {6, 8};

        public const float DefaultAudioFootstepInterval = 10f;


        public static Vector3 ThrdEmitterDistanceDelta = new Vector3(0, 0, 5);
        public static Vector3 FstEmitterDistanceDelta  = new Vector3(0, 0, 3);

//        //开镜打断
//        public static readonly EPlayerState[] SightInterruptSimpleTypes =
//        {
//      //      EPlayerState.OpenUI,
////            EPlayerState.Droping,
////            EPlayerState.Climb,
////            EPlayerState.Reload,
////            EPlayerState.SpecialReload,
////            EPlayerState.Swim,
////            EPlayerState.SwitchWeapon,
//        };

        public static Quaternion ThrdEmitterRotationDelta;
        public static Quaternion FstEmitterRotationDelta;

        public static int   AudioObjectUsageMaxCount    = 50;
        public static float AudioObjectCuttoffThreshold = 0.3f;
        public static int CommonEffectLifeMScd = 30000;
        public static float EffectObjectCuttoffThreshold = 0.3f;

        public const  float InitDistanceForDisplay = 10f;
        public const  float MaxDistanceForCorrent  = 20;
        public const  float InitZDown              = 0.3f;
        public static Vector3 InvalidVec = new Vector3(-9999,-9999,-9999);
        public static float offset =0.01f;

        public static int AttackTimeBegin{ get; set; }
        public const float AttributeInvalidValue = -999;

        public static Vector3 LocalScale = new Vector3(0.01f, 0.01f, 0.01f);
        public static bool isServer;

        public const float RaycastStepOffset = 0.01f;

        public const int SwitchCdTime = 300;
        
        
        public const float WeaponDropHSpeed=5;
        public const float WeaponDropVSpeed=-10f;
        public const float WeaponDropOffset=2;
        public const int SceneWeaponLifetime=20000;
        public const int BagLimitTime=30000;
        public static Vector3 ThrowingEmittorFirstOffset = new Vector3(0.3f, -0.4f, 1.2f);
        public static Vector3 ThrowingEmittorThirdOffset = new Vector3(0, -0.2f, 3.0f);
        
    }
}