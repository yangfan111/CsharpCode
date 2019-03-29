using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.Free.entity;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class FollowEntityAction : AbstractPlayerAction
    {
        private string entity;
        private bool remove;

        public override void DoAction(IEventArgs args)
        {
            foreach (FreeMoveEntity free in args.GameContext.freeMove.GetEntities())
            {
                if (free.hasFreeData && free.freeData.FreeData != null)
                {
                    FreeEntityData data = (FreeEntityData)free.freeData.FreeData;
                    if(data.GetKey() == entity)
                    {
                        PlayerEntity p = GetPlayerEntity(args);
                        if(p != null)
                        {
                            if (remove)
                            {
                                data.follows.Remove(p);
                            }
                            else
                            {
                                data.follows.Add(p);
                            }
                        }
                    }
                }
            }
        }
    }
}
