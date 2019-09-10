using App.Shared.GameMode;
using Core;
using Core.Prediction.UserPrediction.Cmd;
using UserInputManager.Lib;

namespace App.Client.GameMode
{
    public class DefaultModeInputMapper  : IGlobalKeyInputMapper 
    {
        public void RegisterEnvKeyInput(KeyHandler keyHandler, UserCmd userCmd)
        {
            keyHandler.BindKeyAction(UserInputKey.Switch1, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.PrimeWeapon);
            keyHandler.BindKeyAction(UserInputKey.Switch2, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.SecondaryWeapon);
            keyHandler.BindKeyAction(UserInputKey.Switch3, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.PistolWeapon);
            keyHandler.BindKeyAction(UserInputKey.Switch4, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.MeleeWeapon);
            keyHandler.BindKeyAction(UserInputKey.Switch5, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.ThrowingWeapon);
            keyHandler.BindKeyAction(UserInputKey.Switch6, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.TacticWeapon);
        }

        public void RegisterSpecialCmdKeyInput(KeyHandler keyHandler, UserCmd userCmd)
        {
        }
    }
}
