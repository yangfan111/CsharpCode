using com.wd.free.action;
using com.wd.free.@event;
using System;
using Core.Room;
using Core.Free;

namespace App.Shared.FreeFramework.Free.Action
{
    [Serializable]
    class SetRoomStatusAction : AbstractGameAction, IRule
    {
        private string gameStatus;
        private string enterStatus;

        public override void DoAction(IEventArgs args)
        {
            var re = RoomEvent.AllocEvent<SetRoomStatusEvent>();
            re.GameStatus = (ERoomGameStatus) args.GetInt(gameStatus);
            re.EnterStatus = (ERoomEnterStatus) args.GetInt(enterStatus);
            args.GameContext.session.serverSessionObjects.RoomEventDispatchter.AddEvent(re);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.SetRoomStatusAction;
        }
    }
}
