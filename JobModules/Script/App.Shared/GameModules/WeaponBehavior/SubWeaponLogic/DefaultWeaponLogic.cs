using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public class DefaultWeaponLogic : IWeaponLogic
    {
        private IFireLogic _fireLogic;
        private LeftWeaponCmd _cmd = new LeftWeaponCmd();

        public void SetFireLogic(IFireLogic fireLogic)
        {
            _fireLogic = fireLogic;
        }

        public void Update(PlayerWeaponController controller, IUserCmd cmd)
        {
            _cmd.SetCurrentCmd(cmd);
            if(null != _fireLogic)
            {
                _fireLogic.OnFrame(controller, _cmd);
            }
        }
    }
}