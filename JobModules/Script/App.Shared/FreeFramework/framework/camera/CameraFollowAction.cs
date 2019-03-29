using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;

namespace App.Shared.FreeFramework.framework.camera
{
    [Serializable]
    public class CameraFollowAction : AbstractPlayerAction
    {
        private string target;

        public override void DoAction(IEventArgs args)
        {
            int id = 0;
            PlayerEntity p = GetPlayerEntity(args);
            if (p != null)
            {
                if (id == 0 && !string.IsNullOrEmpty(target))
                {
                    FreeData fd = (FreeData)args.GetUnit(target);
                    if (fd != null)
                    {
                        id = fd.Player.entityKey.Value.EntityId;
                    }

                    if (id == 0)
                    {
                        foreach (FreeMoveEntity freeMoveEntity in args.GameContext.freeMove.GetEntities())
                        {
                            if (freeMoveEntity.freeData.Key == target)
                            {
                                id = freeMoveEntity.entityKey.Value.EntityId;
                            }
                        }
                    }
                }

                p.gamePlay.CameraEntityId = id;
            }
        }
    }
}
