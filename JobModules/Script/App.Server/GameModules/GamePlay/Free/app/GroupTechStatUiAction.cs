using com.wd.free.action;
using System;
using System.Collections.Generic;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using Assets.App.Server.GameModules.GamePlay.Free;
using Core.Enums;
using App.Shared.GameModules.Player;

namespace App.Server.GameModules.GamePlay.Free.app
{
    [Serializable]
    public class GroupTechStatUiAction : AbstractGameAction
    {
        private static IComparer<TechStat> KdComparater = new KdStatComparator();
        private static IComparer<TechStat> KillComparater = new KillStatComparator();

        public override void DoAction(IEventArgs args)
        {
            SimpleProto builder = FreePool.Allocate();

            builder.Key = FreeMessageConstant.GroupTechStatUI;

            builder.Ins.Add(args.GameContext.player.count);

            List<TechStat> list = new List<TechStat>();

            int index = 0;
            foreach (PlayerEntity p in args.GameContext.player.GetInitializedPlayerEntities())
            {
                TechStat ts = new TechStat(p, index++);
                builder.Ps.Add(ts.ToMessage());
                list.Add(ts);
            }

            list.Sort(KillComparater);
            if (list.Count > 0 && list[0].kill > 0)
            {
                TechStat ts = list[0];
                builder.Ps[ts.index].Ins[2] |= (1 << (int)EUIGameTitleType.Ace);
            }
            if (list.Count > 1 && list[1].kill > 0)
            {
                TechStat ts = list[1];
                builder.Ps[ts.index].Ins[2] |= (1 << (int)EUIGameTitleType.Second);
            }
            if (list.Count > 2 && list[2].kill > 0)
            {
                TechStat ts = list[2];
                builder.Ps[ts.index].Ins[2] |= (1 << (int)EUIGameTitleType.Third);
            }

            list.Sort(KdComparater);
            if (list.Count > 0 && list[0].kd > 0)
            {
                TechStat ts = list[0];
                builder.Ps[ts.index].Ins[2] |= (1 << (int)EUIGameTitleType.KdKing);
            }

            foreach (PlayerEntity p in args.GameContext.player.GetInitializedPlayerEntities())
            {
                FreeMessageSender.SendMessage(p, builder);
            }
        }
    }

    class KdStatComparator : IComparer<TechStat>
    {
        public int Compare(TechStat x, TechStat y)
        {
            if (x.kd < y.kd)
            {
                return 1;
            }
            if (x.kd == y.kd)
            {
                if (x.kill < y.kill)
                {
                    return 1;
                }
                if (x.kill == y.kill)
                {
                    //最后击杀更晚的玩家排名更低
                    if (x.lastKillTime < y.lastKillTime)
                    {
                        return -1;
                    }
                    if (x.lastKillTime > y.lastKillTime)
                    {
                        return 1;
                    }
                    //未死亡的玩家排名更高&最后死亡时间更晚的玩家排名更高
                    if ((x.lastDeadTime == 0 && y.lastDeadTime != 0)
                        || (x.lastDeadTime > y.lastDeadTime && y.lastDeadTime != 0))
                    {
                        return -1;
                    }
                    if ((x.lastDeadTime != 0 && y.lastDeadTime == 0)
                        || (x.lastDeadTime < y.lastDeadTime && x.lastDeadTime != 0))
                    {
                        return 1;
                    }
                    return 0;
                }
            }
            return -1;
        }
    }

    class KillStatComparator : IComparer<TechStat>
    {
        public int Compare(TechStat x, TechStat y)
        {
            if (x.kill < y.kill)
            {
                return 1;
            }
            if (x.kill == y.kill)
            {
                if (x.dead > y.dead)
                {
                    return 1;
                }
                if (x.dead == y.dead)
                {
                    //最后击杀更晚的玩家排名更低
                    if (x.lastKillTime < y.lastKillTime)
                    {
                        return -1;
                    }
                    if (x.lastKillTime > y.lastKillTime)
                    {
                        return 1;
                    }
                    //最后死亡时间更晚的玩家排名更高
                    if (x.lastDeadTime > y.lastDeadTime && y.lastDeadTime != 0)
                    {
                        return -1;
                    }
                    if (x.lastDeadTime < y.lastDeadTime && x.lastDeadTime != 0)
                    {
                        return 1;
                    }
                    return 0;
                }
            }
            return -1;
        }
    }

    class TechStat
    {
        public string name;
        public string teamName;
        public int id;
        public int team;
        public int honor;
        public int kill;
        public int dead;
        public bool isDead;
        public int damage;
        public int assist;
        public int ping;
        public float kd;
        public int index;
        public int lastKillTime;
        public int lastDeadTime;
        public int c4PlantCount;
        public int c4DefuseCount;
        public bool hasC4;

        public TechStat(PlayerEntity player, int index)
        {
            this.id = (int)player.playerInfo.PlayerId;
            this.kill = player.statisticsData.Statistics.KillCount;
            this.dead = player.statisticsData.Statistics.DeadCount;
            this.assist = player.statisticsData.Statistics.AssistCount;
            this.damage = (int)player.statisticsData.Statistics.TotalDamage;
            this.team = player.playerInfo.Camp;
            this.isDead = player.gamePlay.IsDead();
            this.name = player.playerInfo.PlayerName;
            this.teamName = "";
            this.honor = 0;
            this.ping = player.pingStatistics.Ping;
            this.index = index;
            this.lastKillTime = player.statisticsData.Statistics.LastKillTime;
            this.lastDeadTime = player.statisticsData.Statistics.LastDeadTime;
            this.c4PlantCount = player.statisticsData.Statistics.C4PlantCount;
            this.c4DefuseCount = player.statisticsData.Statistics.C4DefuseCount;
            this.hasC4 = player.statisticsData.Statistics.HasC4;

            if (dead > 0)
            {
                this.kd = (float)kill / (float)dead;
            }
            else
            {
                this.kd = kill;
            }
            if (kill < 5)
            {
                kd = 0;
            }
        }

        public SimpleProto ToMessage()
        {
            SimpleProto msg = FreePool.Allocate();
            msg.Ins.Add(team);
            msg.Ins.Add(id);
            msg.Ins.Add(honor);
            msg.Ins.Add(kill);
            msg.Ins.Add(dead);
            msg.Ins.Add(assist);
            msg.Ins.Add(damage);
            msg.Ins.Add(ping);
            msg.Ins.Add(c4PlantCount);
            msg.Ins.Add(c4DefuseCount);

            msg.Bs.Add(isDead);
            msg.Bs.Add(hasC4);

            msg.Ss.Add(name);
            msg.Ss.Add(teamName);

            return msg;
        }
    }
}
