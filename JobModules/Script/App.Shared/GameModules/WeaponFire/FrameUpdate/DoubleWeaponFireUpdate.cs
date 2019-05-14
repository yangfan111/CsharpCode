using App.Shared;
using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;


namespace App.Shared.GameModules.Weapon.Behavior
{
    public class DoubleWeaponFireUpdate : IWeaponFireUpdate
    {
        private IWeaponFireController _leftWeaponFire;
        private IWeaponFireController _rightWeaponFire;
        private WeaponSideCmd leftCmd = new WeaponSideCmd(EWeaponSide.Left);
        private WeaponSideCmd rightCmd = new WeaponSideCmd(EWeaponSide.Right);
        public DoubleWeaponFireUpdate(IWeaponFireController leftWeaponFire, IWeaponFireController rightWeaponFire)
        {
            _leftWeaponFire = leftWeaponFire;
            _rightWeaponFire = rightWeaponFire;
        }

        public void Update(EntityKey owner, IUserCmd cmd, Contexts contexts)
        {
            leftCmd.SetCmd(cmd);
            rightCmd.SetCmd(cmd);
            _leftWeaponFire.OnUpdate(owner, leftCmd, contexts);
            _rightWeaponFire.OnUpdate(owner, rightCmd, contexts);
        }
    }
}