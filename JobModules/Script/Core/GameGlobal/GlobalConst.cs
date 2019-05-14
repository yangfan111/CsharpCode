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

        public static float WeaponSpreadPerOffset = 1f;
        
        public const int AudioEvt_BulletHit = 3039;
        public const int AudioEvt_BulletFly = 3038;

        public  static float FireSpreadDuration = 0.4f;
        public static bool EnableInputBlockLog = false;
        public static EPlayerInput serachedInput;
        public static bool EnableAudioLog = false;
        public static bool EnableStateLog = false;

        public static bool EnableWeaponLog = false;
        //RTPC
        public static float ModelCarEngine = 0f;
        public static float NormalAttenuation = 1f;
        public static float DifferentPlayerEffect = 0f;
        public static float PlayerCamera = 0f;
        public static float GunMagazineSpeed = 0f;


        public static uint testBusId = 0;
        public static float testFValue = 0f;
        public static bool testValueChanged = false;
        

        public static float MotionSpreadDuration = 0.3f;
        public static sbyte MaxAudioExistTimeSec = 10;


        public const int AimDefaultFov = 80;

        public static readonly int[] SilencerWeapons = new int[] {6, 8};

        public const float DefaultAudioFootstepInterval = 10f;


        public static Vector3 ThrdEmitterDistanceDelta = new Vector3(0,0,5);
        public static Vector3 FstEmitterDistanceDelta = new Vector3(0,0,3);

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

        public static int ObjectUsageMaxCount = 50;
    }
}