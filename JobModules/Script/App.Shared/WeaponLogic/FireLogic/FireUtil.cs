using System;

namespace App.Shared.WeaponLogic.FireLogic
{
    [Flags]
    public enum EAutoFireState
    {
        Burst = 1,
        ReloadBreak,
    }

    public static class FireUtil
    {
        public static void SetFlag(ref int flags, int flag)
        {
            flags |= flag;
        } 

        public static void ClearFlag(ref int flags, int flag)
        {
            flags &= ~flag;
        }
    }
}
