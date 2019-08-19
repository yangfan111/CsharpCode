
using Assets.App.Server.GameModules.GamePlay.Free;
using Core.Free;
using Free.framework;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreeOpenSpecifyUIHandler : IFreeMessageHandler
    {

        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.OpenSpecifyUI;
        }

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {

            player.playerClientUpdate.OpenUIFrameSync = true;
            FreeMessageSender.SendOpenSpecifyUIMessageS(player);

        }

    }
}