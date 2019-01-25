using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    public interface IFreeMove
    {
        void Start(FreeRuleEventArgs args, FreeMoveEntity entity);

        bool Frame(FreeRuleEventArgs args, FreeMoveEntity entity, int interval);
    }
}
