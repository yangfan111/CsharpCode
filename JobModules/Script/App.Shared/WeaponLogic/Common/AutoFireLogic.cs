using App.Shared.WeaponLogic;
using App.Shared.WeaponLogic.FireLogic;
using Core.WeaponLogic.WeaponLogicInterface;
using WeaponConfigNs;

namespace Core.WeaponLogic.Common
{
    public class AutoFireLogic : IAfterFire
    {
        private Contexts _contexts;

        public AutoFireLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var config = GetConfig(playerEntity);
            var weaponState = weaponEntity.weaponData;
            if(weaponState.FireMode != (int)EFireMode.Burst)
            {
                weaponState.BurstShootCount = 0;
                EnableAutoFire(playerEntity, false);
                return;
            }
            weaponState.BurstShootCount += 1;
            if(weaponState.BurstShootCount < config.BurstCount)
            {
                weaponState.NextAttackTimer = (cmd.RenderTime + config.BurstAttackInnerInterval);
                EnableAutoFire(playerEntity, true);
            }
            else
            {
                weaponState.NextAttackTimer = (cmd.RenderTime + config.BurstAttackInterval);
                weaponState.BurstShootCount = 0;
                EnableAutoFire(playerEntity, false);
            }
            if(IsTheLastBullet(playerEntity))
            {
                weaponState.BurstShootCount = 0;
                EnableAutoFire(playerEntity, false);
            }
        }

        private bool IsTheLastBullet(PlayerEntity playerEntity)
        {
            return playerEntity.GetCurrentWeaponInfo(_contexts).Bullet == 0;
        }

        private void EnableAutoFire(PlayerEntity playerEntity, bool autoFire)
        {
            if(!playerEntity.hasWeaponAutoState)
            {
                return;
            }
            if(autoFire)
            {
                FireUtil.SetFlag(ref playerEntity.weaponAutoState.AutoFire, (int)EAutoFireState.Burst);
            }
            else
            {
                FireUtil.ClearFlag(ref playerEntity.weaponAutoState.AutoFire, (int)EAutoFireState.Burst);
            }
        }

        private DefaultFireModeLogicConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.DefaultFireModeLogicCfg;
            }
            return null;
        }
    }
}
