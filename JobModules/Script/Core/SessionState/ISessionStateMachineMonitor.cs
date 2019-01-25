using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.SessionState
{
    public interface ISessionStateMachineMonitor
    {
        void AddState(ISessionState state);
        void ChangeState(ISessionState state);
        void LateUpdate();
    }
}
