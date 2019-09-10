using System;
using System.Collections.Generic;
using App.Shared.Audio;
using App.Shared.Player.Events;
using Core;
using Core.Attack;
using Core.Components;
using Core.EntityComponent;
using Core.Event;
using Core.ObjectPool;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Utils.Configuration;
using WeaponConfigNs;

namespace App.Shared.GameModules.Player
{
    public static class PlayerEntityStatisticsExt
    {
        public static PlayerStatisticsControllerBase StatisticsController(this PlayerEntity player)
        {
            return GameModuleManagement.Get<PlayerStatisticsControllerBase>(player.entityKey.Value.EntityId);
        }
    }

    public class PlayerStatisticsControllerBase : ModuleLogicActivator<PlayerStatisticsControllerBase>
    {
        protected PlayerEntity entity;

        public virtual void Initialize(PlayerEntity entity)
        {
            this.entity = entity;
            entity.AddStatisticsServerData();
        }

        public virtual void Update(IUserCmd cmd)
        {
        }

        /// <summary>
        ///     目前只支持非霰弹枪，非tripple模式枪
        /// </summary>
        public virtual void ShowShootStatisticsTip(bool val)
        {
        }

        public virtual void HandleDispatchedEvt(StatisticsHitPlayerEvent hitPlayerEvent, bool isServer)
        {
            throw new NotImplementedException();
        }

        public void AddShootPlayer(int cmdSeq, IBulletEntityAgent bulletEntityAgent, PrecisionsVector3 hitPoint,
                                   EntityKey targetKey, PrecisionsVector3 positionValue, EBodyPart part,
                                   float totalDamage)
        {
            StatisticsHitPlayerEvent hitPlayerEvent =
                            (StatisticsHitPlayerEvent) EventInfos.Instance.Allocate(EEventType.HitPlayerStastics,
                                false);
            hitPlayerEvent.cmdSeq = cmdSeq;
            //    hitPlayerEvent.bulletRuntimeText = bulletEntityAgent.ToDynamicString();
            //  hitPlayerEvent.bulletBaseText    = bulletEntityAgent.ToBaseDataString();
            hitPlayerEvent.bodyPart       = part;
            hitPlayerEvent.totalDamage    = totalDamage;
            hitPlayerEvent.posValue       = positionValue;
            hitPlayerEvent.hitPoint       = hitPoint;
            hitPlayerEvent.serverTime     = bulletEntityAgent.ServerTime;
            hitPlayerEvent.statisticStr = bulletEntityAgent.StatisticsInfo.ToString();
            hitPlayerEvent.shootKey =
                            string.Format("{0}_{1}_to_{2}", cmdSeq, bulletEntityAgent.OwnerEntityKey, targetKey);
            hitPlayerEvent.shootTarget = targetKey; 

            // srcPlayer.localEvents.Events.AddEvent(e);

            AddShootPlayer(hitPlayerEvent);
        }

        protected virtual void AddShootPlayer(StatisticsHitPlayerEvent shootPlayerEvent)
        {
        }
    }

    public class ClientPlayerStatisticsController : PlayerStatisticsControllerBase
    {
        public override void Update(IUserCmd cmd)
        {
            //            var evts = entity.uploadEvents.Events.GetEvents[EEventType.HitPlayerStastics];
            //            foreach (var vv in evts)
            //            {
            //                EventInfos.Instance.Free(vv);
            //            }
            //
            //            evts.Clear();
        }

        // public override void ShowShootStatisticsTip(bool val)
        // {
        //     
        //     entity.tip.ForTest = val;
        //     if(val)
        //         entity.tip.Content = string.Format("Match:{0}/{1}",entity.statisticsServerData.ShootPlayerMatchCount,entity.statisticsServerData.RecordedShootTotalCount);
        // }

        protected override void AddShootPlayer(StatisticsHitPlayerEvent shootPlayerEvent)
        {
            entity.uploadEvents.Events.AddEvent(shootPlayerEvent);
        }
    }

    public class ServerPlayerStatisticsController : PlayerStatisticsControllerBase
    {
        //TODO:同一帧发射多发子弹情况处理
        private Dictionary<string, StatisticsCSPairData> battleShootStatisticsArchive =
                        new Dictionary<string, StatisticsCSPairData>(100);

        public List<string> removedKeyBuffer = new List<string>();


        private Contexts contexts;
        
        public override void Update(IUserCmd cmd)
        {
            var evt = entity.uploadEvents.Events.GetFirstEvent(EEventType.HitPlayerStastics);
            if (evt != null)
                HandleDispatchedEvt(evt as StatisticsHitPlayerEvent, false);
            LateUpdate(cmd.RenderTime);
            // if(battleShootStatisticsArchive.Count>0)
            //     DebugUtil.MyLog("battleShootStatisticsArchive:{0}",battleShootStatisticsArchive.Count);
        }

        private void LateUpdate(int renderTime)
        {
            removedKeyBuffer.Clear();
            foreach (var valueData in battleShootStatisticsArchive.Values)
            {
                if (valueData.IsMatched)
                {
                    // DebugUtil.AppendMatchedShootText(valueData.ShootC.shootKey, valueData.ShootC.ToString(),
                    //     valueData.ShootS.ToString());
                    ++entity.statisticsServerData.ShootPlayerMatchCount;
                    removedKeyBuffer.Add(valueData.ShootC.shootKey);
                }
                else if (!valueData.Valid(renderTime))
                {
                 
                    if (valueData.IsServer)
                    {
                        ++entity.statisticsServerData.ServerShootPlayerExtraCount;
                    }
                    else
                    {
                    //    DebugUtil.AppendMissShootText(valueData.ValuableKey, valueData.ValuableStr, valueData.IsServer);

                //        var shootTarget = contexts.GetEntityWithEntityKey(valueData.ShootC.shootTarget);
                        
                        ++entity.statisticsServerData.ClientShootPlayerMissCount;
                    }

                    removedKeyBuffer.Add(valueData.ValuableKey);
                }
            }

            foreach (var key in removedKeyBuffer)
            {
                battleShootStatisticsArchive[key].Free();
                battleShootStatisticsArchive.Remove(key);
            }
        }

        public override void HandleDispatchedEvt(StatisticsHitPlayerEvent hitPlayerEvent, bool isServer)
        {
            StatisticsCSPairData statisticsCsPairData;
            if (!battleShootStatisticsArchive.TryGetValue(hitPlayerEvent.shootKey, out statisticsCsPairData))
                statisticsCsPairData = new StatisticsCSPairData();
            var statisticsHit = ObjectAllocatorHolder<StatisticsHitPlayerEvent>.Allocate();
            statisticsHit.RewindTo(hitPlayerEvent);
            if (isServer)
                statisticsCsPairData.ShootS = statisticsHit;
            else
                statisticsCsPairData.ShootC = statisticsHit;

            battleShootStatisticsArchive[hitPlayerEvent.shootKey] = statisticsCsPairData;
        }

        protected override void AddShootPlayer(StatisticsHitPlayerEvent shootPlayerEvent)
        {
            entity.statisticsServerData.ServerShootPlayerTotalCount += 1;
            entity.localEvents.Events.AddEvent(shootPlayerEvent);
        }

        struct StatisticsCSPairData
        {
            public StatisticsHitPlayerEvent ShootC;
            public StatisticsHitPlayerEvent ShootS;

            public bool IsMatched
            {
                get { return ShootS != null && ShootC != null; }
            }

            public void Free()
            {
                if (ShootS != null)
                    ObjectAllocatorHolder<StatisticsHitPlayerEvent>.Free(ShootS);
                if (ShootC != null)
                    ObjectAllocatorHolder<StatisticsHitPlayerEvent>.Free(ShootC);
            }

            public string ValuableStr
            {
                get
                {
                    if (ShootC != null)
                        return ShootC.ToString();
                    return ShootS.ToString();
                }
            }

            public string ValuableKey
            {
                get
                {
                    if (ShootC != null)
                        return ShootC.shootKey;
                    return ShootS.shootKey;
                }
            }

            public bool IsServer
            {
                get { return ShootS != null; }
            }

            public bool Valid(int currentTime)
            {
                var startTime = (ShootC != null) ? ShootC.serverTime : ShootS.serverTime;
                return ((currentTime - startTime) < GlobalConst.ServerShootStatisticsMaxDeltaTime);
            }
        }
    }
}