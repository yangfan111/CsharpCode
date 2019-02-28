using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.skill;
using com.wd.free.util;
using Core.EntityComponent;
using gameplay.gamerule.free.ui;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class RemoveEntityAction : AbstractPlayerAction
    {
        public String entity;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            object obj = fr.GetEntity(FreeUtil.ReplaceVar(entity, args));

            if(obj is FreeMoveEntity)
            {
                FreeEntityData data = (FreeEntityData)((FreeMoveEntity)obj).freeData.FreeData;
                if (data.effect != null)
                {
                    FreeEffectDeleteAction del = new FreeEffectDeleteAction();
                    del.SetKey(data.effect.GetKey());
                    del.SetScope(4);
                    del.Act(args);
                }

                if(data.gameObject != null)
                {
                    GameObject.Destroy(data.gameObject);
                }

                ((FreeMoveEntity) obj).isFlagDestroy = true;
            }
        }
    }
}
