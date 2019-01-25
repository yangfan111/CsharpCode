using System.Collections.Generic;
using Core.EntityComponent;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.Patch;
using Core.Utils;
using Entitas.Utils;
using Utils.Singleton;

namespace Core.Prediction
{
    public class PredictionInitManager<TPredictionComponent> : IPredictionInitManager where TPredictionComponent:IPredictionComponent
    {
        private LoggerAdapter _logger = new LoggerAdapter("Core.Prediction.PredictionInitManager");
       

        public class CloneFilter : IEntityMapFilter
        {
            public bool IsIncludeComponent(IGameEntity entity, IGameComponent componentType)
            {
                return componentType is TPredictionComponent;
            }

            public bool IsIncludeEntity(IGameEntity entity)
            {
                return true;
            }


        }

        class SavedHistoryComparator : IComparer<SavedHistory>
        {
            public int Compare(SavedHistory x, SavedHistory y)
            {
                // ReSharper disable once PossibleNullReferenceException
                return x.HistoryId - y.HistoryId;
            }
        }
        private CloneFilter _filter = new CloneFilter();

        // should be round trip time / frame interval
        private const int MaxHistory = 200;
        private IPredictionRewindInfoProvider _handler;


        private List<SavedHistory> _histories = new List<SavedHistory>();
        private SavedHistoryComparator _savedHistoryComparator = new SavedHistoryComparator();
        

        public PredictionInitManager(IPredictionRewindInfoProvider handler)
        {
            _handler = handler;
        }

        public void PredictionInit()
        {
            _handler.Update();
            if (!_handler.IsReady())
            {
                return;
            }

            if (_handler.IsLatestSnapshotChanged())
            {
                bool isRewinded = DoPredictionInit();
                _handler.AfterPredictionInit(isRewinded);

            }
        }

        
        public bool DoPredictionInit()
        {
            EntityMap remoteEntityMap = _handler.RemoteEntityMap;
            bool shouldRewind = IsHistoryDifferentFrom(remoteEntityMap);

            if (shouldRewind)
            {
                RewindTo(remoteEntityMap);
                _handler.OnRewind();
                
                return true;
            }

            return false;
        }

       
        
        private void RewindTo(EntityMap remoteEntityMap)
        {
            PredictionRewindHandler<TPredictionComponent> rewindHandler = new PredictionRewindHandler<TPredictionComponent>(_handler);

            var localEntityMapClone = PredictionEntityMap.Allocate(false);
            localEntityMapClone.AddAll(_handler.LocalEntityMap);

            EntityMapComparator.Diff(localEntityMapClone, remoteEntityMap, rewindHandler, "predicateRewind");
           
            
            localEntityMapClone.ReleaseReference();
        }

        private bool IsHistoryDifferentFrom(EntityMap remoteEntityMap)
        {
            PredictionCompareHandler<TPredictionComponent> compareHandler = new PredictionCompareHandler<TPredictionComponent>(_handler.RemoteHistoryId);
            SavedHistory history = GetTargetHistory(_handler.RemoteHistoryId);
            if (history != null)
            {
                EntityMapComparator.Diff(history.EntityMap, remoteEntityMap, compareHandler, "predicteCompare");
                if (compareHandler.IsDiff)
                {
                    SingletonManager.Get<DurationHelp>().IncreaseRewindCount();
                    _logger.InfoFormat("should rewind for history diff, historyId {0} {1}", _handler.RemoteHistoryId,history.HistoryId);
                }
                return compareHandler.IsDiff;
            }
            int oldestHistory = _histories.Count > 0 ? _histories[0].HistoryId : 0;
            int latestHistory = _histories.Count > 0 ? _histories[_histories.Count-1].HistoryId : 0;
            _logger.InfoFormat("should rewind for history not saved, historyId {0}, saved history = {1}-{2}", _handler.RemoteHistoryId, oldestHistory, latestHistory);
            return true;
        }



        public SavedHistory GetTargetHistory(int cmdSeq)
        {
            int index = _histories.BinarySearch(new SavedHistory(cmdSeq, null), _savedHistoryComparator);
            if (index >= 0)
            {
                return _histories[index];
            }
            return null;
            
        }

        
        public void SavePredictionCompoments(int historyId)
        {
            

            EntityMap localEntites = _handler.LocalEntityMap;
#pragma warning disable RefCounter001,RefCounter002
            EntityMap remoteEntities = PredictionEntityMap.Allocate();
#pragma warning restore RefCounter001,RefCounter002
            EntityMapDeepCloner.Clone(remoteEntities, localEntites, _filter);

            SavedHistory history = GetTargetHistory(historyId);
            if (history == null)
            {
                _logger.DebugFormat("SavePredictionCompoments_1  {0}", historyId);
                history = new SavedHistory(historyId, remoteEntities);
                _histories.Add(history);
                
            }
            else
            {
                _logger.DebugFormat("Recplce SavePredictionCompoments  {0}", historyId);
                history.EntityMap.ReleaseReference();
                history.EntityMap = remoteEntities;
            }
            if (_histories.Count > MaxHistory)
            {
                var tdhistory = _histories[0];
                tdhistory.EntityMap.ReleaseReference();
                _histories.RemoveAt(0);
            }
        }
    }
}