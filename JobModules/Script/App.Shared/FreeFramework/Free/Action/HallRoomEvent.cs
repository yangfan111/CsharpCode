using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Room;

namespace App.Shared
{
    public class SetRoomStatusEvent : RoomEvent
    {
        public ERoomGameStatus GameStatus;
        public ERoomEnterStatus EnterStatus;

        public SetRoomStatusEvent()
        {
            EventType = ERoomEventType.SetRoomStatus;
        }
    }
}
