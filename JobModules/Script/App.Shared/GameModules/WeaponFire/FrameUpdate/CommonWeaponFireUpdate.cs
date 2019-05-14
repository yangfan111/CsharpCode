using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public class CommonWeaponFireUpdate : IWeaponFireUpdate
    {
        private IWeaponFireController fireController;
        private WeaponSideCmd _cmd = new WeaponSideCmd(EWeaponSide.Left);

        public CommonWeaponFireUpdate(IWeaponFireController controller)
        {
            fireController = controller;
        }


        public void Update(EntityKey owner, IUserCmd cmd, Contexts contexts)
        {
            _cmd.SetCmd(cmd);
            if(null != fireController)
            {
                fireController.OnUpdate(owner, _cmd, contexts);
            }
        }

    }
}