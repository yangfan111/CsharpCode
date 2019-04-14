using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public class CommonWeaponFireUpdate : IWeaponFireUpdate
    {
        private IWeaponFireController _weaponFireController;
        private LeftWeaponCmd _cmd = new LeftWeaponCmd();

        public CommonWeaponFireUpdate(IWeaponFireController controller)
        {
            _weaponFireController = controller;
        }


        public void Update(PlayerWeaponController controller, IUserCmd cmd, Contexts contexts)
        {
            _cmd.SetCurrentCmd(cmd);
            if(null != _weaponFireController)
            {
                _weaponFireController.OnUpdate(controller, _cmd, contexts);
            }
        }

    }
}