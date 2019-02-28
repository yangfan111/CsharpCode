using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using Free.framework;

namespace App.Server.GameModules.GamePlay.free.client
{
    public interface IFreeMessageHandler
    {
        bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message);
        void Handle(ServerRoom room, PlayerEntity player, SimpleProto message);
    }
}
