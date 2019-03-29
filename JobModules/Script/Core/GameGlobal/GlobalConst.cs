using Assets.XmlConfig;
using System;

namespace Core
{
    /// <summary>
    /// Defines the <see cref="GlobalConst" />
    /// </summary>
    public partial class GlobalConst
    {
        public const int WeaponBagMaxCount = 1;

        public static readonly int WeaponSlotMaxLength = (int)EWeaponSlotType.Length;

        public const int WeaponEntityType = 11;

        public const int AudioEvt_Footstep = 1;
        
        public const int AudioEvt_BulletHit = 3039;


        public const int AimDefaultFov = 80;
        
        public static  readonly  int[]SilencerWeapons = new int[]{6,8};
        
        public const float DefaultAudioFootstepInterval = 100f;





    }

}
