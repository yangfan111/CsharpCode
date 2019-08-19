using Core.EntityComponent;
using Core.EntityComponent;
using Core.Replicaton;
using UnityEngine;

namespace App.Client
{
    public class OfflineSnapshotSelector : ISnapshotSelector
    {
        public OfflineSnapshotSelector(EntityKey self, IGameContexts gameContexts)
        {
            _self = self;
            _snapshotFactory = new SnapshotFactory(gameContexts);
            
        }

        public void Init()
        {
            if (left != null)
                left.ReleaseReference();
            left = _snapshotFactory.GenerateSnapshot(_self,_position);
            left.AcquireReference();
            left.Self = _self;
            left.ServerTime = 1;
            left.SnapshotSeq = 1;
            left.VehicleSimulationTime = 0;

            if (right != null)
                right.ReleaseReference();
            right = _snapshotFactory.GenerateSnapshot(_self,_position);;
            right.AcquireReference();
            right.Self = _self;
            right.ServerTime = 2;
            right.SnapshotSeq = 2;
            right.VehicleSimulationTime = 0;
        }
        private EntityKey _self;
        private SnapshotFactory _snapshotFactory;
        private ISnapshot left;
        private ISnapshot right;
        private Vector3 _position = Vector3.zero;

        public SnapshotPair SelectSnapshot(int renderTime)
        {
            return new SnapshotPair(left, right, 1);
        }

        public ISnapshot LatestSnapshot { get { return right; } }
        public ISnapshot OldestSnapshot { get { return left; } }
        public void AddSnapshot(ISnapshot messageBody)
        {
        }

    
        public void Dispose()
        {
        }
    }
}