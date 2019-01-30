using Core.WeaponLogic.Attachment;
using WeaponConfigNs;

namespace Core.WeaponLogic.Accuracy
{
    public class PistolAccuracyLogic : AbstractAccuracyLogic<PistolAccuracyLogicConfig, object>
    {
        public PistolAccuracyLogic(PistolAccuracyLogicConfig config):base(config)
        {
        }

        public override void BeforeFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {
            if (playerWeapon.LastFireTime == 0)
            {
                //
            }
            else
            {
                var accuracy = playerWeapon.LastAccuracy;
                accuracy -= _config.AccuracyFactor* (0.3f - (playerWeapon.ClientTime - playerWeapon.LastFireTime) / 1000.0f);
                if (accuracy > _config.MaxAccuracy)
                    accuracy = _config.MaxAccuracy;
                else if (accuracy < _config.MinAccuracy)
                    accuracy = _config.MinAccuracy;
                playerWeapon.LastAccuracy = accuracy;
            }
        }

        public override void OnIdle(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if (playerWeapon.ContinuesShootCount == 0)
            {
                playerWeapon.LastAccuracy = _config.InitAccuracy;
            }
        }
        public override void Apply(PistolAccuracyLogicConfig baseConfig, PistolAccuracyLogicConfig output, object arg)
        {
            
        }
    }
}