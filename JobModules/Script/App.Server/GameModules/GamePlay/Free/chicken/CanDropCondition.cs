using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.Free.entity;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    public class CanDropCondition : IParaCondition
    {
        private FreeMoveEntity plane;

        public bool Meet(IEventArgs args)
        {
            FreeEntityData entity = ChickenRuleVars.GetDropPlane(args);
            if (entity != null)
            {
                int x = ChickenRuleVars.GetFogStopX(args);
                int y = ChickenRuleVars.GetFogStopY(args);
                int r = ChickenRuleVars.GetFogStopRadius(args);

                float flyX = entity.FreeMoveEntity.position.Value.x;
                float flyY = entity.FreeMoveEntity.position.Value.z;
                if ((x - flyX) * (x - flyX) + (y - flyY) * (y - flyY) <= r * r)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
