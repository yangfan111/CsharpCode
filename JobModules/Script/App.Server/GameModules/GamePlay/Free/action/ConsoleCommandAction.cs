using App.Server.GameModules.GamePlay.Free.client;
using App.Shared.DebugHandle;
using com.wd.free.action;
using com.wd.free.@event;
using Sharpen;
using System;
using System.Collections.Generic;

namespace App.Shared.FreeFramework.Free.Action
{
    [Serializable]
    class ConsoleCommandAction : AbstractPlayerAction
    {
        private string cmd;

        public override void DoAction(IEventArgs args)
        {
            string[] cmdSplit = cmd.Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries);
            if (cmdSplit.Length <= 0)
            {
                return;
            }

            if (cmdSplit.Length == 1)
            {
                DebugCommand debug = new DebugCommand(cmdSplit[0]);
                FreeDebugCommandHandler.Handle(args, debug, GetPlayerEntity(args));
            }
            else
            {
                string[] cmdArgs = new string[cmdSplit.Length - 1];
                for (int i = 1; i < cmdSplit.Length; i++)
                {
                    cmdArgs[i - 1] = cmdSplit[i];
                }
                DebugCommand debug = new DebugCommand(cmdSplit[0], cmdArgs);
                FreeDebugCommandHandler.Handle(args, debug, GetPlayerEntity(args));
            }
        }
    }
}
