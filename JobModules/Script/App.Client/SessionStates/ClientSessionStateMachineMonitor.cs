using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.SessionState;
using Entitas;

namespace App.Client.SessionStates
{
    public class ClientSessionStateMachineMonitor : SessionStateMachineMonitor
    {
        public ClientSessionStateMachineMonitor(IContexts contexts)
        {
            AddMonitor(new ClientSessionStateProgress(contexts));
        }
    }
}
