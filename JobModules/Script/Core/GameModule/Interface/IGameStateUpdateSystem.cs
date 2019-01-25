using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.GameModule.Interface
{
    public interface IGameStateUpdateSystem
    {
        void Update();
        void SendGameEvents();
        void ProcessGameEvents();
    }
}
