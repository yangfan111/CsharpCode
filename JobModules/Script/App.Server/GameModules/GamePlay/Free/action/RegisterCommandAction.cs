using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using gameplay.gamerule.free.ui;
using Core.Free;
using App.Server.GameModules.GamePlay.Free.client;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class RegisterCommandAction : SendMessageAction
    {
        private string command;
        private string usage;

        private IGameAction action;

        protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = FreeMessageConstant.RegisterCommand;

            builder.Ss.Add(command);
            if(desc == null)
            {
                desc = "";
            }
            builder.Ss.Add(desc);

            if(usage == null)
            {
                usage = "";
            }
            builder.Ss.Add(usage);

            if(action != null)
            {
                FreeDebugCommandHandler.RegisterCommand(command, action);
            }
            
        }
    }
}
