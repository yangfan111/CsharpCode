using System.Collections.Generic;
using Core.ObjectPool;
using Core.Prediction;
using Core.Replicaton;
using Core.SpatialPartition;
using UnityEngine;

namespace Core.EntityComponent
{
    
    public  class SnapshotFactory
    {
        CompensationFilter _compensationFilter = new CompensationFilter();
        private IGameContexts _gameContexts;

        public SnapshotFactory(IGameContexts gameContexts)
        {
            _gameContexts = gameContexts;
        }

        public ISnapshot GenerateOfflineSnapshot(EntityKey self, Vector3 position)
        {
            Snapshot snapshot = Snapshot.Allocate();

            EntityMapDeepCloner.Clone(snapshot.EntityMap, _gameContexts.MyEntityMap,
                new SendSnapshotFilter(self, position));
            return snapshot;
        }

        public ISnapshot GeneratePerPlayerSnapshot(int seq, EntityKey self, Vector3 position, Bin2DConfig config,
                                                   IBin2DManager bin, List<IGameEntity> preEntitys, bool isAccountStage,
                                                   bool isPrepareStage, SnapshotEntityInsert insertFun)
        {
            var helper = ObjectAllocatorHolder<SnapshotCreationHelper>.Allocate();
            var snapshot = helper.GeneratePerPlayerSnapshot(seq, self, position, config, bin, preEntitys, isAccountStage,
                isPrepareStage,null);
            helper.ReleaseReference();
            return snapshot;
        }
      
       
        public ISnapshot GenerateCompensationSnapshot()
        {
            Snapshot snapshot = Snapshot.Allocate();

            EntityMapDeepCloner.Clone(snapshot.EntityMap, _gameContexts.CompensationEntityMap, _compensationFilter);
            return snapshot;
        }

        public static ISnapshot CloneSnapshot(ISnapshot src)
        {
            Snapshot snapshot = Replicaton.CloneSnapshot.Allocate();
            snapshot.Header = src.Header;
            EntityMapDeepCloner.Clone(snapshot.EntityMap, src.EntityMap, DummyEntityMapFilter.Instance);
            return snapshot;
        }

     
    }
}