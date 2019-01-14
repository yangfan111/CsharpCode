namespace Core.WeaponLogic.Common
{
    public class AutoFireLogic : IAutoFireLogic
    {
        private readonly int MaxBurstCount = 0;
        private readonly int BurstInterval = 0;
        private readonly int AttackInterval = 0;
        public AutoFireLogic(int maxCount, int burstInterval, int attackInterval)
        {
            MaxBurstCount = maxCount;
            BurstInterval = burstInterval;
            AttackInterval = attackInterval;
        }

        public bool Running
        {
            get;
            private set;
        }

        public void AfterFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {
            if(playerWeapon.FireMode != WeaponConfigNs.EFireMode.Burst)
            {
                playerWeapon.BurstShootCount = 0;
                Running = false;
                return;
            }
            playerWeapon.BurstShootCount += 1;
            if(playerWeapon.BurstShootCount < MaxBurstCount)
            {
                playerWeapon.NextAttackTimer = (playerWeapon.ClientTime + BurstInterval);
                Running = true;
            }
            else
            {
                playerWeapon.NextAttackTimer = (playerWeapon.ClientTime + AttackInterval);
                playerWeapon.BurstShootCount = 0;
                Running = false;
            }
            if(IsTheLastBullet(playerWeapon))
            {
                playerWeapon.BurstShootCount = 0;
                Running = false;
            }
        }

        private bool IsTheLastBullet(IPlayerWeaponState playerWeapon)
        {
            return playerWeapon.LoadedBulletCount == 1;
        }


        public void Reset()
        {
            Running = false;
        }
    }
}
