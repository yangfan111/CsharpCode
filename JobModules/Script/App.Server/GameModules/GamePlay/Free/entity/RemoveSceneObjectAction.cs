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
    public class RemoveSceneObjectAction : AbstractPlayerAction
    {
        private string id;

        public override void DoAction(IEventArgs args)
        {
            Debug.LogFormat("remove scene object {0}", id);
            SceneObjectEntity entity = args.GameContext.sceneObject.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(FreeUtil.ReplaceInt(id, args), (short)EEntityType.SceneObject));

            if (entity != null)
            {
                entity.isFlagDestroy = true;
            }
            else
            {
                FreeMoveEntity moveEntity = args.GameContext.freeMove.GetEntityWithEntityKey(new EntityKey(FreeUtil.ReplaceInt(id, args), (short)EEntityType.FreeMove));
                if (moveEntity != null)
                {
                    moveEntity.isFlagDestroy = true;
                }
            }
        }
    }
}
