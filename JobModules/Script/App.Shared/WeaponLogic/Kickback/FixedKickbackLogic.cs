using App.Shared.WeaponLogic;
using Core.Utils;
using WeaponConfigNs;

namespace Core.WeaponLogic.Kickback
{
    public class FixedKickbackLogic : AbstractKickbackLogic<FixedKickbackLogicConfig>
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(FixedKickbackLogic));

        public FixedKickbackLogic(Contexts contexts) :base(contexts)
        {
        }

        public override void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var config = GetConfig(playerEntity);
            if(null == config)
            {
                Logger.Error("config is null");
                return;
            }

            playerEntity.orientation.NegPunchPitch += config.PunchPitch;
            playerEntity.orientation.WeaponPunchPitch += config.PunchPitch * config.PunchOffsetFactor;
        }

        protected override float GetWeaponPunchYawFactor(PlayerEntity playerEntity)
        {
            var config = GetConfig(playerEntity); 
            if(null == config)
            {
                return 0;
            }
            return config.FallbackOffsetFactor;
        }

        private FixedKickbackLogicConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.FixedKickbackLogicCfg;
            }
            return null;
        }
    }
}
