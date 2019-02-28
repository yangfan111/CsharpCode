using App.Shared.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;
using WeaponConfigNs;

namespace Core.WeaponLogic
{
    public class PistolSpreadLogic : ISpreadLogic
    {
        private Contexts _contexts;

        public PistolSpreadLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void BeforeFireBullet(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var weaponState = weaponEntity.weaponRuntimeInfo;
            float spread = UpdateSpread(playerEntity, weaponEntity, weaponState.Accuracy);
            var config = GetConfig(playerEntity);
            weaponState.LastSpreadX = spread * config.SpreadScale.ScaleX;
            weaponState.LastSpreadY = spread * config.SpreadScale.ScaleY;
        }

        protected float UpdateSpread(PlayerEntity playerEntity, WeaponEntity weaponEntity, float accuracy)
        {
            var weaponState = weaponEntity.weaponRuntimeInfo;
            var config = GetConfig(playerEntity);
            float param;
            var posture = playerEntity.stateInterface.State.GetCurrentPostureState();
            if (!playerEntity.playerMove.IsGround)
            {
                param = config.AirParam;
            }
            else if (playerEntity.playerMove.HorizontalVelocity > 13)
            {
                param = config.LengthGreater13Param;
            }
            else if (posture == XmlConfig.PostureInConfig.Prone || posture == XmlConfig.PostureInConfig.Crouch)
            {
                param = config.DuckParam;
            }
            else
            {
                param = config.DefaultParam;
            }
            return param * (1 - accuracy);
        }

        private PistolSpreadLogicConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.PistolSpreadLogicCfg;
            }
            return null;
        }
    }
}