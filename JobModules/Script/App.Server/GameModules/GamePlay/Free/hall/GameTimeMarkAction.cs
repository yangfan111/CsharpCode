using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using System;

namespace App.Server.GameModules.GamePlay.Free.hall
{
    [Serializable]
    public class GameTimeMarkAction : AbstractGameAction, IRule
    {
        private int type;

        public override void DoAction(IEventArgs args)
        {
            FreeGameRule rule = (FreeGameRule) args.Rule;
            if (type == 0)
            {
                rule.GameStartTime = rule.ServerTime;
                foreach (PlayerEntity player in args.GameContext.player.GetEntities())
                {
                    if(player.hasStatisticsData) player.statisticsData.Statistics.GameJoinTime = rule.ServerTime;
                    SimpleProto sp = FreePool.Allocate();
                    sp.Key = FreeMessageConstant.PlaySound;
                    sp.Ks.Add(1);
                    sp.Ins.Add(args.GameContext.session.commonSession.RoomInfo.MapId);
                    FreeMessageSender.SendMessage(player, sp);
                }
            }

            if (type == 1)
            {
                rule.GameEndTime = rule.ServerTime;
                foreach (PlayerEntity player in args.GameContext.player.GetEntities())
                {
                    if (player.hasStatisticsData && player.statisticsData.Statistics.GameJoinTime > 0)
                    {
                        player.statisticsData.Statistics.GameTime = (int) (rule.ServerTime - player.statisticsData.Statistics.GameJoinTime);
                    }
                    if (player.gamePlay.IsDead())
                    {
                        player.statisticsData.Statistics.DeadTime += (int) (rule.ServerTime - player.statisticsData.Statistics.LastDeadTime);
                    }
                }
            }
        }

        public int GetRuleID()
        {
            return (int) ERuleIds.GameTimeMarkAction;
        }
    }
}
