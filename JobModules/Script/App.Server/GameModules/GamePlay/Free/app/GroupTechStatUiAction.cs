using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Enums;
using Core.Free;
using Free.framework;
using System;
using System.Collections.Generic;

namespace App.Server.GameModules.GamePlay.Free.app
{
    [Serializable]
    public class GroupTechStatUiAction : AbstractGameAction, IRule
    {
        private static IComparer<TechStat> KdComparater = new KdStatComparator();
        private static IComparer<TechStat> HdComparater = new HdStatComparator();
        private static IComparer<TechStat> KillComparater = new KillStatComparator();
        private static IComparer<TechStat> HitDownComparater = new HitDownComparator();

        public override void DoAction(IEventArgs args)
        {
            SimpleProto builder = FreePool.Allocate();
            builder.Key = FreeMessageConstant.GroupTechStatUI;

            bool needSort = true;
            bool rescueEnabled = args.GameContext.session.commonSession.RoomInfo.TeamCapacity > 1;
            List<TechStat> list = new List<TechStat>();
            int index = 0;
            foreach (PlayerEntity p in args.GameContext.player.GetInitializedPlayerEntities())
            {
                TechStat ts = new TechStat(p, index++, rescueEnabled);
                builder.Ps.Add(ts.ToMessage());
                list.Add(ts);
                if (!p.statisticsData.Statistics.DataCollectSwitch)
                {
                    needSort = false;
                }
            }
            builder.Ins.Add(index);

            if (needSort)
            {
                if (rescueEnabled) list.Sort(HitDownComparater);
                else list.Sort(KillComparater);

                if (list.Count > 0 && ((list[0].kill > 0 && !rescueEnabled) || (list[0].hitDownCount > 0 && rescueEnabled)))
                {
                    builder.Ps[list[0].index].Ins[2] |= 1 << (int) EUIGameTitleType.Ace;
                }
                if (list.Count > 1 && ((list[1].kill > 0 && !rescueEnabled) || (list[1].hitDownCount > 0 && rescueEnabled)))
                {
                    builder.Ps[list[1].index].Ins[2] |= 1 << (int) EUIGameTitleType.Second;
                }
                if (list.Count > 2 && ((list[2].kill > 0 && !rescueEnabled) || (list[2].hitDownCount > 0 && rescueEnabled)))
                {
                    builder.Ps[list[2].index].Ins[2] |= 1 << (int) EUIGameTitleType.Third;
                }
                
                if (rescueEnabled) list.Sort(HdComparater);
                else list.Sort(KdComparater);

                if (list.Count > 0 && list[0].kd > 0)
                {
                    builder.Ps[list[0].index].Ins[2] |= 1 << (int) EUIGameTitleType.KdKing;
                }
            }

            foreach (PlayerEntity p in args.GameContext.player.GetInitializedPlayerEntities())
            {
                FreeMessageSender.SendMessage(p, builder);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.GroupTechStatUiAction;
        }
    }

    class KdStatComparator : IComparer<TechStat>
    {
        public int Compare(TechStat x, TechStat y)
        {
            if (x.kd < y.kd) return 1;
            if (x.kd > y.kd) return -1;
            if (x.kill < y.kill) return 1;
            if (x.kill > y.kill) return -1;
            if (x.lastKillTime < y.lastKillTime) return -1;
            if (x.lastKillTime > y.lastKillTime) return 1;
            if ((x.lastDeadTime == 0 && y.lastDeadTime != 0) || (x.lastDeadTime > y.lastDeadTime && y.lastDeadTime != 0)) return -1;
            if ((x.lastDeadTime != 0 && y.lastDeadTime == 0) || (x.lastDeadTime < y.lastDeadTime && x.lastDeadTime != 0)) return 1;
            return 0;
        }
    }

    class HdStatComparator : IComparer<TechStat>
    {
        public int Compare(TechStat x, TechStat y)
        {
            if (x.kd < y.kd) return 1;
            if (x.kd > y.kd) return -1;
            if (x.hitDownCount < y.hitDownCount) return 1;
            if (x.hitDownCount > y.hitDownCount) return -1;
            if (x.lastKillTime < y.lastKillTime) return -1;
            if (x.lastKillTime > y.lastKillTime) return 1;
            if ((x.lastDeadTime == 0 && y.lastDeadTime != 0) || (x.lastDeadTime > y.lastDeadTime && y.lastDeadTime != 0)) return -1;
            if ((x.lastDeadTime != 0 && y.lastDeadTime == 0) || (x.lastDeadTime < y.lastDeadTime && x.lastDeadTime != 0)) return 1;
            return 0;
        }
    }

    class KillStatComparator : IComparer<TechStat>
    {
        public int Compare(TechStat x, TechStat y)
        {
            if (x.kill < y.kill) return 1;
            if (x.kill > y.kill) return -1;
            if (x.dead > y.dead) return 1;
            if (x.dead < y.dead) return -1;
            if (x.assist > y.assist) return -1;
            if (x.assist < y.assist) return 1;
            if (x.damage > y.damage) return -1;
            if (x.damage < y.damage) return 1;
            if (x.lastKillTime < y.lastKillTime) return -1;
            if (x.lastKillTime > y.lastKillTime) return 1;
            if ((x.lastDeadTime == 0 && y.lastDeadTime != 0) || (x.lastDeadTime > y.lastDeadTime && y.lastDeadTime != 0)) return -1;
            if ((x.lastDeadTime != 0 && y.lastDeadTime == 0) || (x.lastDeadTime < y.lastDeadTime && x.lastDeadTime != 0)) return 1;
            return 0;
        }
    }

    class HitDownComparator : IComparer<TechStat>
    {
        public int Compare(TechStat x, TechStat y)
        {
            if (x.hitDownCount < y.hitDownCount) return 1;
            if (x.hitDownCount > y.hitDownCount) return -1;
            if (x.dead > y.dead) return 1;
            if (x.dead < y.dead) return -1;
            if (x.resqueCount > y.resqueCount) return -1;
            if (x.resqueCount < y.resqueCount) return 1;
            if (x.damage > y.damage) return -1;
            if (x.damage < y.damage) return 1;
            if (x.lastKillTime < y.lastKillTime) return -1;
            if (x.lastKillTime > y.lastKillTime) return 1;
            if ((x.lastDeadTime == 0 && y.lastDeadTime != 0) || (x.lastDeadTime > y.lastDeadTime && y.lastDeadTime != 0)) return -1;
            if ((x.lastDeadTime != 0 && y.lastDeadTime == 0) || (x.lastDeadTime < y.lastDeadTime && x.lastDeadTime != 0)) return 1;
            return 0;
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
        public int badgeId;
        public bool isHurt;
        public int hitDownCount;
        public int resqueCount;

        public TechStat(PlayerEntity player, int index, bool rescueEnabled)
        {
            this.id = (int) player.playerInfo.PlayerId;
            this.kill = player.statisticsData.Statistics.KillCount;
            this.dead = player.statisticsData.Statistics.DeadCount;
            this.assist = player.statisticsData.Statistics.AssistCount;
            this.damage = Convert.ToInt32(player.statisticsData.Statistics.TotalDamage);
            this.team = player.playerInfo.Camp;
            this.isDead = player.gamePlay.IsDead();
            this.name = player.playerInfo.PlayerName;
            this.teamName = "";
            this.honor = 0;
            this.ping = player.pingStatistics.Ping;
            this.index = index;
            this.lastKillTime = (int) player.statisticsData.Statistics.LastKillTime;
            this.lastDeadTime = (int) player.statisticsData.Statistics.LastDeadTime;
            this.c4PlantCount = player.statisticsData.Statistics.C4PlantCount;
            this.c4DefuseCount = player.statisticsData.Statistics.C4DefuseCount;
            this.hasC4 = player.statisticsData.Statistics.HasC4;
            this.badgeId = player.playerInfo.BadgeId;
            this.isHurt = player.gamePlay.IsLifeState(EPlayerLifeState.Dying);
            this.hitDownCount = player.statisticsData.Statistics.HitDownCount;
            this.resqueCount = player.statisticsData.Statistics.SaveCount;

            if (rescueEnabled)
            {
                if (hitDownCount < 5)
                {
                    kd = 0;
                }
                else
                {
                    kd = dead > 0 ? hitDownCount / dead : hitDownCount;
                }
            }
            else
            {
                if (kill < 5)
                {
                    kd = 0;
                }
                else
                {
                    kd = dead > 0 ? kill / dead : kill;
                }
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
            msg.Ins.Add(badgeId);
            msg.Ins.Add(hitDownCount);
            msg.Ins.Add(resqueCount);

            msg.Bs.Add(isDead);
            msg.Bs.Add(hasC4);
            msg.Bs.Add(isHurt);

            msg.Ss.Add(name);
            msg.Ss.Add(teamName);

            return msg;
        }
    }
}
