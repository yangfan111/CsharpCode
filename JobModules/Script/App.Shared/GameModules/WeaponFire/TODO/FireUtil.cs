using System;

namespace App.Shared.GameModules.Weapon.Behavior
{
    [Flags]
    public enum EAutoFireState
    {
        Burst = 1,
        ReloadBreak,
    }

    /// <summary>
    /// Defines the <see cref="FireUtil" />
    /// </summary>
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
