using WeaponConfigNs;

namespace Core.WeaponLogic
{
    public class RifleFireBulletCounter : IFireBulletCounter
    {
        private RifleFireCounterConfig _config;
        public RifleFireBulletCounter(RifleFireCounterConfig config)
        {
            _config = config;
        }

        public void OnIdle(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if (playerWeapon.ContinuesShootDecreaseNeeded)
            {
                playerWeapon.ContinuesShootDecreaseNeeded = false;
                
                if (playerWeapon.ContinuesShootCount > _config.MaxCount)
                    playerWeapon.ContinuesShootCount = _config.MaxCount;
                playerWeapon.ContinuesShootDecreaseTimer = playerWeapon.ClientTime + _config.DecreaseInitInterval;
            }

            if (playerWeapon.ContinuesShootCount > 0 && playerWeapon.ContinuesShootDecreaseTimer <= playerWeapon.ClientTime)
            {
                playerWeapon.ContinuesShootDecreaseTimer = playerWeapon.ClientTime + _config.DecreaseStepInterval;
                playerWeapon.ContinuesShootCount--;
            }
            else if (playerWeapon.ContinuesShootCount == 0)
            {
            }

        }

        public void BeforeFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {
            playerWeapon.ContinuesShootDecreaseNeeded = true;
            playerWeapon.ContinuesShootCount++;
        }
    }
}