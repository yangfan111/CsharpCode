using App.Shared;
using Core.Free;
using Core.Utils;
using Free.framework;

namespace Assets.App.Server.GameModules.GamePlay.Free
{
    public partial class FreeMessageSender
    {
        public static void SendOpenSpecifyUIMessageC(PlayerEntity playerEntity)
        {
            SimpleProto openUIProto = FreePool.Allocate();
            openUIProto.Key = FreeMessageConstant.OpenSpecifyUI;
            SendMessageC(playerEntity, openUIProto);
        }

     
        public static void SendOpenSpecifyUIMessageS(PlayerEntity playerEntity)
        {
            SimpleProto sp = FreePool.Allocate();
            sp.Key = FreeMessageConstant.OpenSpecifyUI;
            SendMessage(playerEntity, sp);
        }
    }
}