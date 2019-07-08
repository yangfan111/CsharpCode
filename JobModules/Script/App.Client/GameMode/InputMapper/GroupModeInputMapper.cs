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

        public void RegisterEnvKeyInput(KeyReceiver keyReceiver, UserCmd userCmd)
        {
        }

        public void RegisterSpecialCmdKeyInput(KeyReceiver keyReceiver, UserCmd userCmd)
        {
            keyReceiver.AddAction(UserInputKey.Switch1, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.PrimeWeapon);
            keyReceiver.AddAction(UserInputKey.Switch3, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.PistolWeapon);
            keyReceiver.AddAction(UserInputKey.Switch4, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.MeleeWeapon);
            keyReceiver.AddAction(UserInputKey.Switch5, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.ThrowingWeapon);
            keyReceiver.AddAction(UserInputKey.Switch6, (data) => userCmd.CurWeapon = (int)EWeaponSlotType.TacticWeapon);
        }
    }
}
