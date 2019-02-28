using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.GamePlay.Free.map.position;
using com.wd.free.map.position;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class FreeNotMove : AbstractFreeMove
    {
        public override bool Frame(FreeRuleEventArgs args, FreeMoveEntity entity, int interval)
        {
            return false;
        }

        public override void StartMove(FreeRuleEventArgs args, FreeMoveEntity entity)
        {
        }
    }
}
