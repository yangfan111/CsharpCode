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
        CanNotProne,
        OutOfOxygen,
        FireModeToAuto,
        FireModeToManual,
        FireModeToBurst,
        FireModeLocked,
        FireWithNoBullet,
        NoWeaponInSlot,
        CantSwithGrenade,
    }
}
