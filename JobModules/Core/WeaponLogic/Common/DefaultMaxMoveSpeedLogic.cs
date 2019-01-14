using WeaponConfigNs;

namespace Core.WeaponLogic
{
    public class DefaultMaxMoveSpeedLogic : IMoveSpeedLogic
    {
        private DefaultMoveSpeedLogicConfig _config;
        public DefaultMaxMoveSpeedLogic(DefaultMoveSpeedLogicConfig config)
        {
            _config  = config;
        }
        public float GetMoveSpeedMs()
        {
            return _config.MaxSpeed;
        }
    }
}