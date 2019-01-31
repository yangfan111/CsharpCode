using App.Shared.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;
using WeaponConfigNs;

namespace Core.WeaponLogic
{
    public class RifleFireBulletCounter : IFireBulletCounter
    {
        private Contexts _contexts;
        public RifleFireBulletCounter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void OnIdle(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var config = GetConfig(playerEntity);
            var weaponState = weaponEntity.weaponRuntimeInfo;
            if (weaponState.ContinuesShootDecreaseNeeded)
            {
                weaponState.ContinuesShootDecreaseNeeded = false;
                if (weaponState.ContinuesShootCount > config.MaxCount)
                {
                    weaponState.ContinuesShootCount = config.MaxCount;
                }
                weaponState.ContinuesShootDecreaseTimer = playerEntity.time.ClientTime + config.DecreaseInitInterval;
            }

            if (weaponState.ContinuesShootCount > 0 && weaponState.ContinuesShootDecreaseTimer <= playerEntity.time.ClientTime)
            {
                weaponState.ContinuesShootDecreaseTimer = playerEntity.time.ClientTime + config.DecreaseStepInterval;
                weaponState.ContinuesShootCount--;
            }
            else if (weaponState.ContinuesShootCount == 0)
            {
            }
        }

        public void BeforeFireBullet(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var weaponState = weaponEntity.weaponRuntimeInfo;
            weaponState.ContinuesShootDecreaseNeeded = true;
            weaponState.ContinuesShootCount++;
        }

        private RifleFireCounterConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.RifleFireCounterCfg;
            }
            return null;
        }
    }
}