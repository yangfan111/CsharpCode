using System.Collections.Generic;
using System.Linq;
using Core.EntityComponent;
using Core.Prediction.UserPrediction;
using Core.Replicaton;
using Core.SyncLatest;
using Core.Utils;
using Utils.Singleton;

namespace Core.Prediction
{
    /// <summary>
    /// 严格复制IUserPrediction所有组件
    /// 有多帧同时进来的情况下只执行最后一帧的Prediction比对
    /// </summary>
    public class PredictionManager
                    //<TPredictionComponent> : PredictionManager where TPredictionComponent:IPredictionComponent
    {  
        // should be round trip time / frame interval
        private const int MaxHistory = 200;

        //fst diff handler 
        private PredictionMapCheckDiffHandler checkDiffHandler;



        private GameEntityDefaultCompareAgent gameEntityComparator;
        private GameEntityDefaultCompareAgent gameEntityRewindComparator;
        private Queue<HistoryEntityMap> histories = new Queue<HistoryEntityMap>();
        private LoggerAdapter logger = new LoggerAdapter("Core.Prediction.PredictionInitManager");

        private AbstractPredictionProvider provider;
        private PredictionMapRewindDiffHandler rewindDiffHandler;

        //scd diff handler

        public PredictionManager(AbstractPredictionProvider provider)
        {
            this.provider              = provider;
            checkDiffHandler           = new PredictionMapCheckDiffHandler();
            gameEntityComparator       = new GameEntityDefaultCompareAgent(checkDiffHandler);
            rewindDiffHandler          = new PredictionMapRewindDiffHandler(provider);
            gameEntityRewindComparator = new GameEntityDefaultCompareAgent(rewindDiffHandler);
        }

        public void RewindFirstSnapshot(EntityKey self)
        {
            var localEntityMapClone = PredictionEntityMap.Allocate(false);
            provider.SetSelf(self);
            provider.Update();
            localEntityMapClone.AddAll(provider.LocalEntityMap);
            logger.InfoFormat("{0}   {1}", provider.LocalEntityMap.Count, localEntityMapClone.Count);
            checkDiffHandler.SetRemoteCmdSeq(provider.LastSelfUserCmdSeqId);
            EntityMapCompareExecutor.Diff(localEntityMapClone, provider.RemoteEntityMap, checkDiffHandler,
                "RewindFirstSnapshotompare", gameEntityComparator);
            foreach (var gameEntity in provider.RemoteEntityMap.ToArray())
            {
                foreach (var gameComponent in gameEntity.ComponentList)
                {
                    logger.InfoFormat("{0}", gameComponent);
                }
            }

            EntityMapCompareExecutor.Diff(localEntityMapClone, provider.RemoteEntityMap, rewindDiffHandler,
                "rewindFirstSnapshot", gameEntityRewindComparator);
            RefCounterRecycler.Instance.ReleaseReference(localEntityMapClone);
        }

        public void PredictionInitUpdate()
        {
            provider.Update();
            if (!provider.IsReady())
            {
                return;
            }

            if (provider.IsLatestSnapshotChanged())
            {
                bool isRewinded = DoPredictionInit();
                provider.AfterPredictionInit(isRewinded);
            }
        }


        public bool DoPredictionInit()
        {
            EntityMap remoteEntityMap = provider.RemoteEntityMap;
            bool      shouldRewind    = IsHistoryDifferentFrom(remoteEntityMap);

            if (shouldRewind)
            {
                RewindTo(remoteEntityMap);
                provider.OnRewind();

                return true;
            }

            return false;
        }


        private void RewindTo(EntityMap remoteEntityMap)
        {
          
            var localEntityMapClone = PredictionEntityMap.Allocate(false);
            localEntityMapClone.AddAll(provider.LocalEntityMap);
            EntityMapCompareExecutor.Diff(localEntityMapClone, remoteEntityMap, rewindDiffHandler, "predicateRewind",gameEntityRewindComparator);
            RefCounterRecycler.Instance.ReleaseReference(localEntityMapClone);
        }

        private bool IsHistoryDifferentFrom(EntityMap remoteEntityMap)
        {
            checkDiffHandler.SetRemoteCmdSeq(provider.LastSelfUserCmdSeqId);
            HistoryEntityMap historyEntityMap = GetTargetHistory(provider.LastSelfUserCmdSeqId);
            if (historyEntityMap != null)
            {
                EntityMapCompareExecutor.Diff(historyEntityMap.EntityMap, remoteEntityMap, checkDiffHandler, "predicteCompare",gameEntityComparator);
                if (checkDiffHandler.IsDiff)
                {
                    SingletonManager.Get<DurationHelp>().IncreaseRewindCount();
                    logger.InfoFormat("should rewind for history diff, historyId {0} {1}", provider.LastSelfUserCmdSeqId,
                        historyEntityMap.SeqId);
                }

                return checkDiffHandler.IsDiff;
            }

            int oldestHistory = histories.Count > 0 ? histories.First().SeqId : 0;
            int latestHistory = histories.Count > 0 ? histories.Last().SeqId : 0;
            logger.InfoFormat("should rewind for history not saved, historyId {0}, saved history = {1}-{2}",
                provider.LastSelfUserCmdSeqId, oldestHistory, latestHistory);
            return true;
        }


        public HistoryEntityMap GetTargetHistory(int cmdSeq)
        {
            foreach (var hinfo in histories)
            {
                if (hinfo.SeqId == cmdSeq)
                    return hinfo;
            }

            return null;
        }
        //frame update

        public void SavePredictionCompoments(int seqHistoryId)
        {
            EntityMap localEntites = provider.LocalEntityMap;
#pragma warning disable RefCounter001,RefCounter002
            EntityMap remoteEntities = PredictionEntityMap.Allocate();
#pragma warning restore RefCounter001,RefCounter002
            EntityMapDeepCloner.Clone(remoteEntities, localEntites, CloneFilter.Instance);

            HistoryEntityMap historyEntityMap = GetTargetHistory(seqHistoryId);
            if (historyEntityMap == null)
            {
                logger.DebugFormat("SavePredictionCompoments1  {0}", seqHistoryId);
                historyEntityMap = new HistoryEntityMap(seqHistoryId, remoteEntities);
                histories.Enqueue(historyEntityMap);
            }
            else
            {
                logger.DebugFormat("Recplce SavePredictionCompoments  {0}", seqHistoryId);
                RefCounterRecycler.Instance.ReleaseReference(historyEntityMap.EntityMap);
                historyEntityMap.EntityMap = remoteEntities;
            }

            if (histories.Count > MaxHistory)
            {
                var tdhistory = histories.Dequeue();
                RefCounterRecycler.Instance.ReleaseReference(tdhistory.EntityMap);
            }
        }


        public class CloneFilter : IEntityMapFilter
        {
            public bool IsIncludeComponent(IGameEntity entity, IGameComponent componentType)
            {
                return componentType is IUserPredictionComponent;
            }

            public bool IsIncludeEntity(IGameEntity entity)
            {
                return true;
            }
            public static readonly CloneFilter Instance = new CloneFilter();
        }
        public class HistoryEntityMap
        {
            public HistoryEntityMap(int seqId, EntityMap entityMap)
            {
                SeqId = seqId;
                EntityMap = entityMap;
            }

            public int SeqId;
            public EntityMap EntityMap;

            
        }
    }
}