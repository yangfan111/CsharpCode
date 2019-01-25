using System;

namespace Core.BulletSimulation
{
    [Flags]
    public enum EPlayerMask
    {
        TeamA = 1,
        TeamB = 2,
        Invincible = 4,
    }
}
