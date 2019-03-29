using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.SessionState;
using Entitas;

namespace App.Server
{
    public class ServerSessionStateMachineMonitor : SessionStateMachineMonitor
    {
        public ServerSessionStateMachineMonitor(IContexts contexts, ServerRoom room)
        {
            AddMonitor(new ServerSessionStateProgress(room));
        }
    }
}
