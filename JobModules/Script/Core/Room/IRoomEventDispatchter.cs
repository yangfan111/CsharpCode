using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Room
{
    public interface IRoomEventDispatchter
    {
        void AddEvent(RoomEvent e);
    }
}
