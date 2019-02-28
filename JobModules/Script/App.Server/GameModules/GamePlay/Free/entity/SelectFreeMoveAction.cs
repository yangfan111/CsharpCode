using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.para.exp;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class SelectFreeMoveAction : AbstractGameAction
    {
        private IGameAction action;
        private IParaCondition condition;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            foreach (FreeMoveEntity free in ((Contexts)(fr.GameContext)).freeMove.GetEntities())
            {
                if (free.hasFreeData && free.freeData.FreeData != null)
                {
                    fr.TempUse("entity", (FreeEntityData)free.freeData.FreeData);

                    if (condition == null || condition.Meet(args))
                    {
                        if (action != null)
                        {
                            action.Act(args);
                        }
                    }

                    fr.Resume("entity");
                }
            }
        }
    }
}
