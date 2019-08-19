using System;
using System.Collections.Generic;
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

namespace Core.EntityComponent
{
    public class GameComponentIndexLookUp<TEntity> : IComponentTypeLookup where TEntity : Entity
    {
        private static LoggerAdapter logger = new LoggerAdapter(type: typeof(GameComponentIndexLookUp<>));
        //以ComponentId为索引的数组
        Type[] typesByComponentId;
        int[] indexsByComponentId;
        //IPredictionComponent | ISelfLatestComponent
        int[] selfIndexByComponentId = new int[0];
        //playback | INonSelfLatestComponent 
        int[] noselfIndexByComponentId = new int[0];
        //IUpdateComponent
        int[] updateLatestIndexByComponentId = new int[0];
        //ILatestComponent
        int[] syncLatestIndexByComponentId = new int[0];
        //IPlaybackComponent
        int[] playbackIndexByComponentId = new int[0];
        //ICompensationComponent
        int[] compensationIndexsIndexByComponentId = new int[0];
        //IAssetComponent
        int[] assetComponentIndexsByComponentId= new int[0];
        
        
        public Type[] AllTypesByEntitasIndex { get; private set; }

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
            get { return assetComponentIndexsByComponentId; }
        }

        public int[] SelfIndexs
        {
            get { return selfIndexByComponentId; }
        }

        public int[] NoSelfIndexs
        {
            get { return noselfIndexByComponentId; }
        }

        public int[] UpdateLatestIndexs
        {
            get { return updateLatestIndexByComponentId; }
        }

        public int[] SyncLatestIndexs
        {
            get { return syncLatestIndexByComponentId; }
        }  
        public int[] PlaybackIndexs
        {
            get { return playbackIndexByComponentId; }
        }

      

        public int[] CompensationIndexs
        {
            get { return compensationIndexsIndexByComponentId; }
        }

        public readonly List<int> NeedNotifyIndexs;
      

        public GameComponentIndexLookUp(Type[] typesByEntitasIndex)
        {
            AllTypesByEntitasIndex = typesByEntitasIndex;
            typesByComponentId = new Type[0];
            indexsByComponentId = new int[0];

            int selfIndex = 0;
            int noselfIndex = 0;
            int updateLatestIndex = 0;
            int syncLatestIndex = 0;
            int playbackIndex = 0;
            int compensationIndex = 0;
            int assetComponentIndex = 0;
            for (int i = 0; i < typesByEntitasIndex.Length; i++)
            {
                Type compType = typesByEntitasIndex[i];
                if (typeof(IGameComponent).IsAssignableFrom(compType))
                {
                  
                    var comp = (IGameComponent) Activator.CreateInstance(compType);
                    logger.DebugFormat("{0}  id:{1} index;{2}", compType, comp.GetComponentId(), i );
                    ArrayUtility.SafeSet(ref typesByComponentId, comp.GetComponentId(), compType);
                    ArrayUtility.SafeSet(ref indexsByComponentId, comp.GetComponentId(), i, -1);
                    if (comp is IPredictionComponent || comp is ISelfLatestComponent)
                    {
                        ArrayUtility.SafeSet(ref selfIndexByComponentId, selfIndex, i,-1);
                        selfIndex++;
                    }
                    
                    if (comp is IPlaybackComponent || comp is INonSelfLatestComponent)
                    {
                        ArrayUtility.SafeSet(ref noselfIndexByComponentId, noselfIndex, i,-1);
                        noselfIndex++;
                        if (comp is IPlaybackComponent)
                        {
                      
                            ArrayUtility.SafeSet(ref playbackIndexByComponentId, playbackIndex, i, - 1);
                            playbackIndex++;                     
                        }
                    }

                    if (comp is IUpdateComponent)
                    {
                        ArrayUtility.SafeSet(ref updateLatestIndexByComponentId, updateLatestIndex, i, - 1);
                        updateLatestIndex++;
                    }

                    if (comp is ILatestComponent)
                    {
                      
                        ArrayUtility.SafeSet(ref syncLatestIndexByComponentId, syncLatestIndex, i, - 1);
                        syncLatestIndex++;                     
                    } 
                   
                    if (comp is ICompensationComponent)
                    {
                       
                        ArrayUtility.SafeSet(ref compensationIndexsIndexByComponentId, compensationIndex, i, - 1);
                        compensationIndex++;                     
                    }

                    if (comp is IAssetComponent)
                    {
                        ArrayUtility.SafeSet(ref assetComponentIndexsByComponentId, assetComponentIndex, i, - 1);
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
            
            NeedNotifyIndexs = new List<int>();
            NeedNotifyIndexs.Add(PositionComponentIndex);
            NeedNotifyIndexs.Add(EntityKeyComponentIndex);
            NeedNotifyIndexs.Add(OwnerIdComponentIndex);
            NeedNotifyIndexs.Add(FlagCompensationComponentIndex);
            NeedNotifyIndexs.Add(FlagDestroyComponentIndex);

            NeedNotifyIndexs.Add(FlagSelfComponentIndex);
            NeedNotifyIndexs.Add(FlagSyncSelfComponentIndex);
            NeedNotifyIndexs.Add(FlagSyncNonSelfComponentIndex);
        }

       

        public int[] IndexByComponentId
        {
            get { return indexsByComponentId; }
        }

        private int GetComponentIndex(Type type)
        {
            for (int i = 0; i < AllTypesByEntitasIndex.Length; i++)
            {
                if (AllTypesByEntitasIndex[i] == type)
                    return i;
            }

            return -1;
        }

        public int GetComponentIndex(int componentId)
        {
            return indexsByComponentId[componentId];
        }

        public Type GetComponentType(int componentId)
        {
            return typesByComponentId[componentId];
        }


        public int MaxIndex
        {
            get { return AllTypesByEntitasIndex.Length; }
        }
      
        public int GetComponentIndex<T>() where T : IGameComponent
        {
            int idx = ComponentIndex<TEntity, T>.Index;
            if (idx == ComponentIndex<TEntity, T>.UnInitialized)
            {
                idx = GetComponentIndex(typeof(T));
                ComponentIndex<TEntity, T>.Index = idx;
                if (idx < 0)
                    logger.WarnFormat("entity type {0} don't support component type {1}", typeof(TEntity), typeof(T));
            }

            return ComponentIndex<TEntity, T>.Index;
        }
    }
}