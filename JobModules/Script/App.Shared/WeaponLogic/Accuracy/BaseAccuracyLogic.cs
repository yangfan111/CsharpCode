using App.Shared.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;
using WeaponConfigNs;

namespace Core.WeaponLogic.Accuracy
{
    public class BaseAccuracyLogic : IAccuracyLogic
    {
        private Contexts _contexts;

        public BaseAccuracyLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public  void BeforeFireBullet(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var config = GetConfig(playerEntity);
            if(null == config)
            {
                return;
            }
            var weaponState = weaponEntity.weaponRuntimeInfo;
            int accuracyDivisor = config.AccuracyDivisor;
            if (accuracyDivisor != -1)
            {
                int shotsFired = weaponState.ContinuesShootCount;
                float maxInaccuracy = config.MaxInaccuracy;
                float accuracyOffset = config.AccuracyOffset;
                float accuracy = shotsFired * shotsFired * shotsFired / accuracyDivisor + accuracyOffset;
                if (accuracy > maxInaccuracy)
                    accuracy = maxInaccuracy;
                weaponState.Accuracy = accuracy;
            }
            else
            {
                weaponState.Accuracy = 0;
            }
        }

        public  void OnIdle(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            
        }
        
        private BaseAccuracyLogicConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.BaseAccuracyLogicCfg;
            }
            return null;
        }
    }
}