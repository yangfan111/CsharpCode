using System.Collections.Generic;
using App.Protobuf;
using Free.framework;
using Core.Free;
using App.Client.Console;
using App.Shared.Client;

namespace Assets.Sources.Free.UI
{
    public class RegisterCommandHandler
    {
        public static bool canHandle(object messsage)
        {
            if(messsage is SimpleProto)
            {
                SimpleProto sp = (SimpleProto)messsage;
                return sp.Key == FreeMessageConstant.RegisterCommand;
            }

            return false;
        }

        public static void Handle(IClientRoom room, object message)
        {
            if (message is SimpleProto)
            {
                SimpleProto sp = (SimpleProto)message;

                room.RegisterCommand(sp.Ss[0], sp.Ss[1], sp.Ss[2]);
            }
        }

    }
}
