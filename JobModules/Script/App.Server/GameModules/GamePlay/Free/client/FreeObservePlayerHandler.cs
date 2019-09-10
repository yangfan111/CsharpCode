using App.Shared.FreeFramework.Free.player;
using Core.Free;
using Free.framework;

namespace App.Server.GameModules.GamePlay.free.client
{
    /// <summary>
    /// 废弃. 使用PlayerSyncStageSystem作为登录完成动作
    /// </summary>
    public class FreeObservePlayerHandler : IFreeMessageHandler
    {
        public bool CanHandle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            return message.Key == FreeMessageConstant.ObservePlayer;
        }

        public void Handle(ServerRoom room, PlayerEntity player, SimpleProto message)
        {
            PlayerObserveAction.ObservePlayer(room.ContextsWrapper.FreeArgs, player, true, true);
        }
    }
}
