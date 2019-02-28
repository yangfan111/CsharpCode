using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.UserInput;
using App.Shared.Util;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Client.GameModules.Player
{
    public class InputSchemeUpdateSystem: AbstractUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(InputSchemeUpdateSystem));

        protected override bool filter(PlayerEntity player)
        {
            return player.hasAppearanceInterface && player.appearanceInterface.Appearance!=null;
        }

        protected override void ExecuteUserCmd(PlayerEntity player, IUserCmd cmd)
        {
            var updateList = player.appearanceInterface.Appearance.GetInputSchemeActionFieldToUpdate();
            
            foreach (KeyValuePair<int, string> keyValuePair in updateList)
            {
                PlayerInputManager.UpdateAction(keyValuePair.Key, keyValuePair.Value);
                Logger.InfoFormat("updateListCount key:{0}, value:{1}", keyValuePair.Key, keyValuePair.Value);
            }

            updateList.Clear();
        }
    }
}
