using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.SessionState
{
    public abstract class SessionStateMachineMonitor :  ISessionStateMachineMonitor
    {

        private IList<ISessionStateMachineMonitor> _monitors = new List<ISessionStateMachineMonitor>();

        protected void AddMonitor(ISessionStateMachineMonitor monitor)
        {
            _monitors.Add(monitor);
        }

        public void AddState(ISessionState state)
        {
            int count = _monitors.Count;
            for (int i = 0; i < count; ++i)
            {
                _monitors[i].AddState(state);
            }
        }

        public void ChangeState(ISessionState state)
        {
            int count = _monitors.Count;
            for (int i = 0; i < count; ++i)
            {
                _monitors[i].ChangeState(state);
            }
        }

        public void LateUpdate()
        {
            int count = _monitors.Count;
            for (int i = 0; i < count; ++i)
            {
                _monitors[i].LateUpdate();
            }
        }
    }
}
