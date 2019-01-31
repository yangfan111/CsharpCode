using WeaponConfigNs;
using Core.WeaponLogic.WeaponLogicInterface;
using App.Shared.WeaponLogic;
using App.Shared.GameModules.Camera.Utils;

namespace Core.WeaponLogic
{
    public class RifleSpreadLogic : ISpreadLogic 
    {
        private Contexts _contexts;

        public RifleSpreadLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void BeforeFireBullet(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var config = GetConfig(playerEntity);
            var weaponState = weaponEntity.weaponRuntimeInfo;
            float spread = UpdateSpread(playerEntity, weaponEntity, weaponState.Accuracy);
            weaponState.LastSpreadX = spread * config.SpreadScale.ScaleX;
            weaponState.LastSpreadY = spread * config.SpreadScale.ScaleY;
        }

        protected float UpdateSpread(PlayerEntity playerEntity, WeaponEntity weaponEntity, float accuracy)
        {
            var config = GetConfig(playerEntity);
            float spread = 0;
            var spreadCfg = playerEntity.IsAiming() ? config.Aiming : config.Default;
            var posture = playerEntity.stateInterface.State.GetCurrentPostureState();
            if (!playerEntity.playerMove.IsGround)
            {
                spread = spreadCfg.Base * spreadCfg.Air;
            }
            else if (playerEntity.playerMove.HorizontalVelocity > config.FastMoveSpeed)
            {
                spread = spreadCfg.Base * spreadCfg.FastMove;
            }
            else if (posture == XmlConfig.PostureInConfig.Prone)
            {
                spread = spreadCfg.Base * spreadCfg.Prone;
            }
            else
            {
                spread = spreadCfg.Base;
            }
            return spread * accuracy;
        }

        private RifleSpreadLogicConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.RifleSpreadLogicCfg;
            }
            return null;
        }
    }
}