using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using Core.Free;
using System;

namespace App.Shared.FreeFramework.framework.camera
{
    [Serializable]
    public class CameraFollowAction : AbstractPlayerAction, IRule
    {
        private string target;

        public override void DoAction(IEventArgs args)
        {
            int id = 0;
            PlayerEntity p = GetPlayerEntity(args);
            if (p != null)
            {
                /*if (!p.gamePlay.IsDead())
                {
                    p.gamePlay.CameraEntityId = 0;
                    return;
                }*/

                if (DateTime.Now.Ticks / 10000 - p.statisticsData.Statistics.LastDeadTime <= 2990L) return;

                if (id == 0 && !string.IsNullOrEmpty(target))
                {
                    IParable para = args.GetUnit(target);
                    if (null != para)
                    {
                        if (para is FreeData)
                        {
                            FreeData fd = (FreeData)para;
                            id = fd.Player.entityKey.Value.EntityId;
                        }
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

        public int GetRuleID()
        {
            return (int)ERuleIds.CameraFollowAction;
        }
    }
}
