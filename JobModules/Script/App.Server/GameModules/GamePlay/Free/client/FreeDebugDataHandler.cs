using Core.Free;
using Core.Utils;
using Free.framework;

namespace App.Server.GameModules.GamePlay.free.client
{
    public class FreeDebugDataHandler : IFreeMessageHandler
    {
        static LoggerAdapter logger = new LoggerAdapter("FrameTest");

        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.DebugData;
        }

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            logger.InfoFormat("{0},{1},{2},{3},{4},{5},{6}", message.Ss[0], message.Ks[0], message.Ins[0], message.Ins[1], message.Ss[1], message.Ss[2], message.Ss[3]);
        }
    }
}
