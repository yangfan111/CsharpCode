using App.Shared.GameMode;
using Core;
using Core.Prediction.UserPrediction.Cmd;
using UserInputManager.Lib;

namespace App.Client.GameMode
{
    public class DefaultModeInputMapper  : IGlobalKeyInputMapper 
    {
        public void RegisterEnvKeyInput(KeyReceiver keyReceiver, UserCmd userCmd)
        {
            keyReceiver.BindKeyAction(UserInputKey.Switch1, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.PrimeWeapon);
            keyReceiver.BindKeyAction(UserInputKey.Switch2, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.SecondaryWeapon);
            keyReceiver.BindKeyAction(UserInputKey.Switch3, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.PistolWeapon);
            keyReceiver.BindKeyAction(UserInputKey.Switch4, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.MeleeWeapon);
            keyReceiver.BindKeyAction(UserInputKey.Switch5, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.ThrowingWeapon);
            keyReceiver.BindKeyAction(UserInputKey.Switch6, (data) => userCmd.CurWeapon = (int) EWeaponSlotType.TacticWeapon);
        }

        public void RegisterSpecialCmdKeyInput(KeyReceiver keyReceiver, UserCmd userCmd)
        {
        }
    }
}
