using Core.WeaponLogic.Attachment;
using WeaponConfigNs;

namespace Core.WeaponLogic.Accuracy
{
    public class BaseAccuracyLogic : AbstractAccuracyLogic<BaseAccuracyLogicConfig,  object>
    {
        public BaseAccuracyLogic(BaseAccuracyLogicConfig config):base(config)
        {
        }

        public override void BeforeFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {
            int accuracyDivisor = _config.AccuracyDivisor;
            if (accuracyDivisor != -1)
            {
                int shotsFired = playerWeapon.ContinuesShootCount;
                float maxInaccuracy = _config.MaxInaccuracy;
                float accuracyOffset = _config.AccuracyOffset;
                float accuracy = shotsFired * shotsFired * shotsFired / accuracyDivisor + accuracyOffset;
                if (accuracy > maxInaccuracy)
                    accuracy = maxInaccuracy;
                playerWeapon.LastAccuracy = accuracy;
            }
            else
            {
                playerWeapon.LastAccuracy = 0;
            }
        }

        public override void OnIdle(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            
        }
        public override void Apply(BaseAccuracyLogicConfig baseConfig, BaseAccuracyLogicConfig output, object arg)
        {
            
        }
        
    }
}