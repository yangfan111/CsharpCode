using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public class DoubleWeaponFireUpdate : IWeaponFireUpdate
    {
        private IWeaponFireController _leftWeaponFire;
        private IWeaponFireController _rightWeaponFire;
        private LeftWeaponCmd _leftcmd = new LeftWeaponCmd();
        private RightWeaponCmd _rightCmd = new RightWeaponCmd();
        public DoubleWeaponFireUpdate(IWeaponFireController leftWeaponFire, IWeaponFireController rightWeaponFire)
        {
            _leftWeaponFire = leftWeaponFire;
            _rightWeaponFire = rightWeaponFire;
        }

        public void Update(PlayerWeaponController controller, IUserCmd cmd, Contexts contexts)
        {
            _leftcmd.SetCurrentCmd(cmd);
            _rightCmd.SetCurrentCmd(cmd);
            _leftWeaponFire.OnUpdate(controller, _leftcmd, contexts);
            _rightWeaponFire.OnUpdate(controller, _rightCmd, contexts);
        }
    }
}