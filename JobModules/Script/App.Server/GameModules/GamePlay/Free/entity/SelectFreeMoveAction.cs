using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.util;
using System;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class SelectFreeMoveAction : AbstractGameAction, IRule
    {
        private string entity;

        private IGameAction action;
        private IParaCondition condition;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            if (!StringUtil.IsNullOrEmpty(entity))
            {
                object obj = fr.GetEntity(FreeUtil.ReplaceVar(entity, args));
                if (obj is FreeMoveEntity)
                {
                    fr.TempUse("entity", (FreeEntityData) ((FreeMoveEntity)obj).freeData.FreeData);
                    action.Act(args);
                    fr.Resume("entity");
                }
            }
            else
            {
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

        public int GetRuleID()
        {
            return (int)ERuleIds.SelectFreeMoveAction;
        }
    }
}
