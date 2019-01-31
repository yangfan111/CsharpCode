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
            var runtimeInfo = weaponEntity.weaponRuntimeInfo;
            var basicInfo = weaponEntity.weaponBasicInfo;
            if(basicInfo.FireMode != (int)EFireMode.Burst)
            {
                runtimeInfo.BurstShootCount = 0;
                EnableAutoFire(playerEntity, false);
                return;
            }
            runtimeInfo.BurstShootCount += 1;
            if(runtimeInfo.BurstShootCount < config.BurstCount)
            {
                runtimeInfo.NextAttackTimer = (cmd.RenderTime + config.BurstAttackInnerInterval);
                EnableAutoFire(playerEntity, true);
            }
            else
            {
                runtimeInfo.NextAttackTimer = (cmd.RenderTime + config.BurstAttackInterval);
                runtimeInfo.BurstShootCount = 0;
                EnableAutoFire(playerEntity, false);
            }
            if(IsTheLastBullet(playerEntity))
            {
                runtimeInfo.BurstShootCount = 0;
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
