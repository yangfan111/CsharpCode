using App.Shared.WeaponLogic.FireLogic;
using Core.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.WeaponLogic.Common
{
    public class SpecialReloadCheckLogic : IFireCheck
    {
        private Contexts _contexts;
        public SpecialReloadCheckLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public bool IsCanFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            return CheckSpecialReload(playerEntity, weaponEntity);
        }

        /// <summary>
        /// 判断特殊换弹逻辑 
        /// </summary>
        /// <param name="playerWeapon"></param>
        /// <param name="cmd"></param>
        /// <returns>是否可以射击</returns>
        private bool CheckSpecialReload(PlayerEntity playerEntity, WeaponEntity weaponEntity)
        {
            if(playerEntity.stateInterface.State.GetActionState() != ActionInConfig.Reload && 
                playerEntity.stateInterface.State.GetActionState() != ActionInConfig.SpecialReload)
            {
                return true;
            }
            var config = GetConfig(playerEntity);
            if (config.SpecialReloadCount > 0 && weaponEntity.weaponBasicInfo.Bullet > 0)
            {
                //TODO 特殊换弹打断逻辑
                var weaponState = playerEntity.GetCurrentWeaponInfo(_contexts);
                if (weaponEntity.weaponBasicInfo.PullBolt)
                {
                    //如果已经上膛，直接打断并开枪
                    playerEntity.stateInterface.State.ForceBreakSpecialReload(null);
                    return true;
                }
                else
                {
                    //如果没有上膛，执行上膛，结束后开枪
                    playerEntity.stateInterface.State.BreakSpecialReload();
                    weaponEntity.weaponBasicInfo.PullBolt = true;
                    if (playerEntity.hasWeaponAutoState)
                    {
                        FireUtil.SetFlag(ref playerEntity.weaponAutoState.AutoFire, (int)EAutoFireState.ReloadBreak);
                    }
                }
            }
            return false;
        }

        private CommonFireConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.CommonFireCfg;
            }
            return null;
        }
    }
}
