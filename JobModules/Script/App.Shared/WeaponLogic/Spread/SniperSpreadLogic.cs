using App.Shared.GameModules.Camera.Utils;
using App.Shared.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;
using WeaponConfigNs;

namespace Core.WeaponLogic
{
    public class SniperSpreadLogic : ISpreadLogic
    {
        private Contexts _contexts;

        protected int Length2D1 = 350;
        protected int Length2D2 = 25;

        public SniperSpreadLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void BeforeFireBullet(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var config = GetConfig(playerEntity);
            var weaponState = weaponEntity.weaponRuntimeInfo;
            float spread = UpdateSpread(playerEntity, weaponState.Accuracy);
            weaponState.LastSpreadX = spread * config.SpreadScale.ScaleX;
            weaponState.LastSpreadY = spread * config.SpreadScale.ScaleY;
        }

        protected float UpdateSpread(PlayerEntity playerEntity, float accuracy)
        {
            var config = GetConfig(playerEntity);
            float param;
            var posture = playerEntity.stateInterface.State.GetCurrentPostureState();
            if (!playerEntity.playerMove.IsGround)
            {
                param = config.AirParam;
            }
            else if (playerEntity.playerMove.HorizontalVelocity > Length2D1)
            {
                param = config.LengthParam1;
            }
            else if (playerEntity.playerMove.HorizontalVelocity > Length2D2)
            {
                param = config.LengthParam2;
            }
            else if (posture == XmlConfig.PostureInConfig.Crouch || posture == XmlConfig.PostureInConfig.Prone)
            {
                param = config.DuckParam;
            }
            else
            {
                param = config.DefaultParam;
            }
            if(!playerEntity.IsAiming())
            {
                param += config.FovAddParam;
            }
            return param;
        }

        private SniperSpreadLogicConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.SniperSpreadLogicCfg;
            }
            return null;
        }
    }
}