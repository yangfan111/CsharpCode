using App.Server.GameModules.GamePlay.Free.item;
using com.wd.free.action;
using com.wd.free.@event;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    class InitialItemAction : AbstractGameAction
    {
        public override void DoAction(IEventArgs args)
        {
            SingletonManager.Get<FreeItemDrop>().Initial(args.GameContext.session.commonSession.RoomInfo.MapId);
        }
    }
}
