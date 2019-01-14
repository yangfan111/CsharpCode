using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Shared.FreeFramework.framework.ai
{
    [Serializable]
    public class PlayerInterceptKeyAction : AbstractPlayerAction
    {
        private enum KeyType { Press = 1, Intercept = 2, Cancel = 3 }
        private int type;
        private string key;
        private string time;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);

            switch (type)
            {
                case (int)KeyType.Press:
                    PlayerInterceptCommands.PressKeys(player, args.GetInt(key), args.GetInt(time));
                    break;
                case (int)KeyType.Intercept:
                    PlayerInterceptCommands.InterceptKeys(player, args.GetInt(key), args.GetInt(time));
                    break;
                case (int)KeyType.Cancel:
                    PlayerInterceptCommands.ClearKeys(player);
                    break;
                default:
                    break;
            }
        }
    }
}
