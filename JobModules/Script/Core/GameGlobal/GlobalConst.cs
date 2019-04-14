using Assets.XmlConfig;
using System;
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
        public const int Length2D1 = 350;

        public const int Length2D2 = 25;

        public static float WeaponSpreadPerOffset = 1000f;
        
        public const int AudioEvt_BulletHit = 3039;
        public const int AudioEvt_BulletFly = 3038;

        public  static float FireSpreadDuration = 0.5f;

        public static bool EnableAudioLog = false;
        public static bool EnableWeaponLog = false;


        public static float MotionSpreadDuration = 0.5f;

        public const int AimDefaultFov = 80;

        public static readonly int[] SilencerWeapons = new int[] {6, 8};

        public const float DefaultAudioFootstepInterval = 10f;

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

    }
}