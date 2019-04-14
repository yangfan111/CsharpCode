using App.Server.GameModules.GamePlay.Free.item;
using com.wd.free.action;
using com.wd.free.@event;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    class InitialItemAction : AbstractGameAction
    {
        public override void DoAction(IEventArgs args)
        {
            FreeItemDrop.Initial(args.GameContext.session.commonSession.RoomInfo.MapId);
        }
    }
}
