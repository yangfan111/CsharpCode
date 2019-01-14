using System;
using System.Text;
using Core.Compensation;
using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.SyncLatest;
using Core.UpdateLatest;
using Core.Utils;
using Entitas;

namespace Core.EntitasAdpater
{
    public class ComponentIndexLookUp<TEntity> : IComponentTypeLookup where TEntity : Entity
    {
        private static LoggerAdapter _logger = new LoggerAdapter(type: typeof(ComponentIndexLookUp<>));
        private Type[] _typesByEntitasIndex;
        private Type[] _typesByComponentId;
        private int[] _indexByComponentId;

        public Type[] AllTypesByEntitasIndex
        {
            get { return _typesByEntitasIndex; }
        }

        public int EntityAdapterComponentIndex { get; private set; }
        public int EntityKeyComponentIndex { get; private set; }
        public int FlagCompensationComponentIndex { get; private set; }
        public int FlagDestroyComponentIndex { get; private set; }
        public int FlagSelfComponentIndex { get; private set; }
        public int FlagSyncNonSelfComponentIndex { get; private set; }
        public int FlagSyncSelfComponentIndex { get; private set; }
        public int PositionComponentIndex { get; private set; }
        public int FlagPositionFilterComponentIndex { get; private set; }
        public int OwnerIdComponentIndex { get; private set; }
        public int FlagImmutabilityComponentIndex { get; private set; }
        public int LifeTimeComponentIndex { get; private set; }

        public int[] AssetComponentIndexs  {
            get { return _assetComponentIndexsByComponentId; }
        }

        public int[] SelfIndexs
        {
            get { return _selfIndexByComponentId; }
        }

        public int[] NoSelfIndexs
        {
            get { return _noselfIndexByComponentId; }
        }

        public int[] UpdateLatestIndexs
        {
            get { return _updateLatestIndexByComponentId; }
        }

        public int[] SyncLatestIndexs
        {
            get { return _syncLatestIndexByComponentId; }
        }  
        public int[] PlaybackIndexs
        {
            get { return _playbackIndexByComponentId; }
        }

      

        public int[] CompensationIndexs
        {
            get { return _compensationIndexsIndexByComponentId; }
        }

        int[] _selfIndexByComponentId = new int[0];
        int[] _noselfIndexByComponentId = new int[0];
        int[] _updateLatestIndexByComponentId = new int[0];
        int[] _syncLatestIndexByComponentId = new int[0];
        int[] _playbackIndexByComponentId = new int[0];
        int[] _compensationIndexsIndexByComponentId = new int[0];
        int[] _assetComponentIndexsByComponentId= new int[0];

        public ComponentIndexLookUp(Type[] typesByEntitasIndex)
        {
            _typesByEntitasIndex = typesByEntitasIndex;
            _typesByComponentId = new Type[0];
            _indexByComponentId = new int[0];

            int selfIndex = 0;
            int noselfIndex = 0;
            int updateLatestIndex = 0;
            int syncLatestIndex = 0;
            int playbackIndex = 0;
            int compensationIndex = 0;
            int assetComponentIndex = 0;
            for (int i = 0; i < _typesByEntitasIndex.Length; i++)
            {
                Type compType = _typesByEntitasIndex[i];
                if (typeof(IGameComponent).IsAssignableFrom(compType))
                {
                  
                    var comp = (IGameComponent) Activator.CreateInstance(compType);
                    _logger.InfoFormat("{0}  id:{1} index;{2}", compType, comp.GetComponentId(), i );
                    ArrayUtility.SafeSet(ref _typesByComponentId, comp.GetComponentId(), compType);
                    ArrayUtility.SafeSet(ref _indexByComponentId, comp.GetComponentId(), i, -1);
                    if (comp is IPredictionComponent || comp is ISelfLatestComponent)
                    {
                        ArrayUtility.SafeSet(ref _selfIndexByComponentId, selfIndex, i,-1);
                        selfIndex++;
                    }

                    if (comp is IPlaybackComponent || comp is INonSelfLatestComponent)
                    {
                        ArrayUtility.SafeSet(ref _noselfIndexByComponentId, noselfIndex, i,-1);
                        noselfIndex++;
                    }

                    if (comp is IUpdateComponent)
                    {
                        ArrayUtility.SafeSet(ref _updateLatestIndexByComponentId, updateLatestIndex, i, - 1);
                        updateLatestIndex++;
                    }

                    if (comp is ILatestComponent)
                    {
                      
                        ArrayUtility.SafeSet(ref _syncLatestIndexByComponentId, syncLatestIndex, i, - 1);
                        syncLatestIndex++;                     
                    } 
                    if (comp is IPlaybackComponent)
                    {
                      
                        ArrayUtility.SafeSet(ref _playbackIndexByComponentId, playbackIndex, i, - 1);
                        playbackIndex++;                     
                    }
                    if (comp is ICompensationComponent)
                    {
                       
                        ArrayUtility.SafeSet(ref _compensationIndexsIndexByComponentId, compensationIndex, i, - 1);
                        compensationIndex++;                     
                    }

                    if (comp is IAssetComponent)
                    {
                        ArrayUtility.SafeSet(ref _assetComponentIndexsByComponentId, assetComponentIndex, i, - 1);
                        assetComponentIndex++;
                    }
                }
            }


            EntityAdapterComponentIndex = GetComponentIndex(typeof(EntityAdapterComponent));
            EntityKeyComponentIndex = GetComponentIndex(typeof(EntityKeyComponent));
            FlagCompensationComponentIndex = GetComponentIndex(typeof(FlagCompensationComponent));
            FlagDestroyComponentIndex = GetComponentIndex(typeof(FlagDestroyComponent));
            FlagSelfComponentIndex = GetComponentIndex(typeof(FlagSelfComponent));
            FlagSyncNonSelfComponentIndex = GetComponentIndex(typeof(FlagSyncNonSelfComponent));
            FlagSyncSelfComponentIndex = GetComponentIndex(typeof(FlagSyncSelfComponent));
            PositionComponentIndex = GetComponentIndex(typeof(PositionComponent));
            FlagPositionFilterComponentIndex = GetComponentIndex(typeof(PositionFilterComponent));
            OwnerIdComponentIndex = GetComponentIndex(typeof(OwnerIdComponent));
            FlagImmutabilityComponentIndex = GetComponentIndex(typeof(FlagImmutabilityComponent));
            LifeTimeComponentIndex = GetComponentIndex(typeof(LifeTimeComponent));
        }

       

        public int[] IndexByComponentId
        {
            get { return _indexByComponentId; }
        }

        private int GetComponentIndex(Type type)
        {
            for (int i = 0; i < _typesByEntitasIndex.Length; i++)
            {
                if (_typesByEntitasIndex[i] == type)
                    return i;
            }

            return -1;
        }

        public int GetComponentIndex(int componentId)
        {
            return _indexByComponentId[componentId];
        }

        public Type GetComponentType(int componentId)
        {
            return _typesByComponentId[componentId];
        }


        public int MaxIndex
        {
            get { return _typesByEntitasIndex.Length; }
        }

        public int GetComponentIndex<T>() where T : IGameComponent
        {
            int idx = ComponentIndex<TEntity, T>.Index;
            if (idx == ComponentIndex<TEntity, T>.UnInitialized)
            {
                idx = GetComponentIndex(typeof(T));
                ComponentIndex<TEntity, T>.Index = idx;
                if (idx < 0)
                    _logger.WarnFormat("entity type {0} don't support component type {1}", typeof(TEntity), typeof(T));
            }

            return ComponentIndex<TEntity, T>.Index;
        }
    }
}