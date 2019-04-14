using System;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared
{
    public static class WeaponSystemCmdUtil
    {
        public static bool FilterWeaponReload(PlayerEntity player, PlayerWeaponController controller,IUserCmd cmd)
        {
            if (!cmd.FilteredInput.IsInput(EPlayerInput.IsReload) && !player.playerMove.IsAutoRun)
                return false;
            if (!controller.HeldWeaponAgent.IsValid())
                return false;
            return true;
        }
           
     
    }
}