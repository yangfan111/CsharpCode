using App.Shared.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;
using WeaponConfigNs;

namespace Core.WeaponLogic.Accuracy
{
    public class PistolAccuracyLogic : IAccuracyLogic 
    {
        private Contexts _contexts;
        public PistolAccuracyLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void BeforeFireBullet(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var weaponState = weaponEntity.weaponData;
            if (weaponState.LastFireTime == 0)
            {
                //
            }
            else
            {
                var config = GetConfig(playerEntity);
                if(null != config)
                {
                    var accuracy = weaponState.Accuracy;
                    accuracy -= config.AccuracyFactor* (0.3f - (cmd.RenderTime - weaponState.LastFireTime) / 1000.0f);
                    if (accuracy > config.MaxAccuracy)
                        accuracy = config.MaxAccuracy;
                    else if (accuracy < config.MinAccuracy)
                        accuracy = config.MinAccuracy;
                    weaponState.Accuracy = accuracy;
                }
            }
        }

        public void OnIdle(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var weaponState = weaponEntity.weaponData;
            if (weaponState.ContinuesShootCount == 0)
            {
                var config = GetConfig(playerEntity);
                if(null != config)
                {
                    weaponState.Accuracy = config.InitAccuracy;
                }
            }
        }

        private PistolAccuracyLogicConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.PistolAccuracyLogicCfg;
            }
            return null;
        }
    }
}