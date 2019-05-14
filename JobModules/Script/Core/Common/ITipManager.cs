using System;

namespace Core.Common
{
    
//    class TipStringAttribute : Attribute
//    {
//        public string TipString;
//        public TipStringAttribute(string tipString) { TipString = tipString; }
//    }
    
    public enum ETipType
    {
        None,
        BulletRunout,
        NoBulletInPackage,
        CanNotRescure,
        CanNotStand,
        CanNotCrouch,
        CanNotToCrouch,
        CanNotProne,
        OutOfOxygen,
        FireModeToAuto,
        FireModeToManual,
        FireModeToBurst,
        FireModeLocked,
        FireWithNoBullet,
        NoWeaponInSlot,
        CantSwithGrenade,
        EnterNumError,
    }
}
