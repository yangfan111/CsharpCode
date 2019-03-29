using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Room;
using Core.SessionState;
using Core.Utils;

namespace App.Server
{
    public class ServerSessionStateProgress : ISessionStateMachineMonitor
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerSessionStateProgress));
        private ServerRoom _room;
        private int _remainStates;
        public ServerSessionStateProgress(ServerRoom room)
        {
            _room = room;
        }

        public void AddState(ISessionState state)
        {
            _remainStates++;
        }

        public void ChangeState(ISessionState state)
        {
            _remainStates--;
            if (_remainStates == 0)
            {
                _room.OnInitializeCompleted();
            }
        }

        public void LateUpdate()
        {
           
        }
    }
}
