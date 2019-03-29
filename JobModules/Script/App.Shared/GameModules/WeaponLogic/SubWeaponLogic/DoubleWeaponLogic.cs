using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public class DoubleWeaponLogic : IWeaponLogic
    {
        private IFireLogic _leftFireLogic;
        private IFireLogic _rightFireLogic;
        private LeftWeaponCmd _leftcmd = new LeftWeaponCmd();
        private RightWeaponCmd _rightCmd = new RightWeaponCmd();
        public DoubleWeaponLogic(IFireLogic leftFireLogic, IFireLogic rightFireLogic)
        {
            _leftFireLogic = leftFireLogic;
            _rightFireLogic = rightFireLogic;
        }

        public void Update(PlayerWeaponController controller, IUserCmd cmd)
        {
            _leftcmd.SetCurrentCmd(cmd);
            _rightCmd.SetCurrentCmd(cmd);
            _leftFireLogic.OnFrame(controller, _leftcmd);
            _rightFireLogic.OnFrame(controller, _rightCmd);
        }
    }
}