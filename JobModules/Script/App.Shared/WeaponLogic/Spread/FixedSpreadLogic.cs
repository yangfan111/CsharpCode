using WeaponConfigNs;
using Core.WeaponLogic.WeaponLogicInterface;
using App.Shared.WeaponLogic;

namespace Core.WeaponLogic
{
    public class FixedSpreadLogic : ISpreadLogic
    {
        private Contexts _contexts;
        public FixedSpreadLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void BeforeFireBullet(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var config = GetConfig(playerEntity);
            var weaponState = weaponEntity.weaponData;
            float spread = UpdateSpread(playerEntity, weaponState.Accuracy);
            weaponState.LastSpreadX = spread * config.SpreadScale.ScaleX;
            weaponState.LastSpreadY = spread * config.SpreadScale.ScaleY;
        }

        protected float UpdateSpread(PlayerEntity playerEntity, float accuracy)
        {
            var config = GetConfig(playerEntity);
            return config.Value;
        }

        FixedSpreadLogicConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.FixedSpreadLogicCfg;
            }
            return null;
        }
    }
}