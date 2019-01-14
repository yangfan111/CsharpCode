using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace Free.framework.ui
{
    public interface IFreeMessageSender
    {
        void SendMessage(IEventArgs args, SimpleProto message, int scope, string player);
    }
}
