using App.Shared;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public class WeaponSideCmd 
    {
        public  IUserCmd    UserCmd { get; private set; }
        private EWeaponSide weaponSide;

        public bool FiltedInput(XmlConfig.EPlayerInput playerInput)
        {
            return UserCmd.FilteredInput.IsInput(playerInput);
        }
        public WeaponSideCmd(EWeaponSide weaponSide)
        {
            this.weaponSide = weaponSide;
        }
        public void SetCmd(IUserCmd cmd)
        {
            this.UserCmd = cmd;
        }

        public bool SwitchThrowMode
        {
            get { return UserCmd.IsRightAttack; }
        }
        public bool IsFire
        {
            get { return weaponSide == EWeaponSide.Left?UserCmd.IsLeftAttack:UserCmd.IsRightAttack ; }
        }
    }
}
