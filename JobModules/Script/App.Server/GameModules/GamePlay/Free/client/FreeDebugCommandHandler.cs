using App.Server.GameModules.GamePlay.free.player;
using App.Shared.DebugHandle;
using com.wd.free.action;
using com.wd.free.para;
using Sharpen;

namespace App.Server.GameModules.GamePlay.Free.client
{
    public class FreeDebugCommandHandler
    {

        private static MyDictionary<string, IGameAction> commandDic = new MyDictionary<string, IGameAction>();

        public static void RegisterCommand(string command, IGameAction action)
        {
            commandDic[command.ToLower()] = action;
        }

        private static StringPara commandPara;

        public static void Handle(ServerRoom room, DebugCommand message, PlayerEntity player)
        {
            if (commandDic.ContainsKey(message.Command.ToLower()))
            {
                FreeLog.Reset();

                IGameAction action = commandDic[message.Command.ToLower()];

                if (FreeLog.IsEnable())
                {
                    FreeLog.SetTrigger(string.Format("命令行 {0}: {1}", message.Command, string.Join(" ", message.Args)));
                }

                if (commandPara == null)
                {
                    commandPara = new StringPara("command", "");
                }

                if (message.Args != null)
                {
                    for (int i = 1; i <= message.Args.Length; i++)
                    {
                        room.FreeArgs.TempUsePara(new StringPara("arg" + i, message.Args[i - 1]));
                    }
                }


                room.FreeArgs.TempUsePara(commandPara);
                room.FreeArgs.TempUse("current", (FreeData)player.freeData.FreeData);

                action.Act(room.FreeArgs);

                if (message.Command == "relive")
                {
                    player.isFlagCompensation = true;
                }

                room.FreeArgs.Resume("current");
                room.FreeArgs.ResumePara("command");

                if (message.Args != null)
                {
                    for (int i = 1; i <= message.Args.Length; i++)
                    {
                        room.FreeArgs.ResumePara("arg" + i);
                    }
                }

                FreeLog.Print();
            }
        }
    }
}
