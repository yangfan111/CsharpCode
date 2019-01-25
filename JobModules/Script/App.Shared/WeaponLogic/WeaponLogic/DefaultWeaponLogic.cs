using Core.Prediction.UserPrediction.Cmd;

namespace Core.WeaponLogic
{
    public class DefaultWeaponLogic : IWeaponLogic
    {
        private IFireLogic _fireLogic;
        private LeftWeaponCmd _cmd = new LeftWeaponCmd();

        public DefaultWeaponLogic(IFireLogic fireLogic)
        {
            _fireLogic = fireLogic;
        }

        public void Update(PlayerEntity playerEntity, WeaponEntity weaponEntity, IUserCmd cmd)
        {
            _cmd.SetCurrentCmd(cmd);
            _fireLogic.OnFrame(playerEntity, weaponEntity, _cmd);
        }
    }
}