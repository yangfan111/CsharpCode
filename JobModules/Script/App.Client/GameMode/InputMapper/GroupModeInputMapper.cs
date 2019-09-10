using System;
using Core;
using Core.Prediction.UserPrediction.Cmd;
using UserInputManager.Lib;

namespace App.Client.GameMode
{
    class GroupModeInputMapper : IGlobalKeyInputMapper
    {
        private Contexts _contexts;
        public GroupModeInputMapper(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void RegisterEnvKeyInput(KeyHandler keyHandler, UserCmd userCmd)
        {
        }

        public void RegisterSpecialCmdKeyInput(KeyHandler keyHandler, UserCmd userCmd)
        {
            keyHandler.BindKeyAction(UserInputKey.Switch1, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.PrimeWeapon);
            keyHandler.BindKeyAction(UserInputKey.Switch3, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.PistolWeapon);
            keyHandler.BindKeyAction(UserInputKey.Switch4, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.MeleeWeapon);
            keyHandler.BindKeyAction(UserInputKey.Switch5, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.ThrowingWeapon);
            keyHandler.BindKeyAction(UserInputKey.Switch6, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.TacticWeapon);
        }
    }
}
