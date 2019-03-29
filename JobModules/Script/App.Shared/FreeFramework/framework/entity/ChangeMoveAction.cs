using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.util;
using App.Server.GameModules.GamePlay.Free.entity;
using App.Server.GameModules.GamePlay;

namespace com.wd.free.entity
{
    [Serializable]
    public class ChangeMoveAction : AbstractGameAction
    {
        private string entity;
        private IFreeMove move;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            object obj = fr.GetEntity(FreeUtil.ReplaceVar(entity, args));

            if (obj is FreeMoveEntity)
            {
                FreeEntityData data = (FreeEntityData)((FreeMoveEntity)obj).freeData.FreeData;
                data.SetMove(args, move);
            }
        }
    }
}
