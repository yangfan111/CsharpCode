using System.Collections.Generic;
using Core.EntityComponent;
using Core.Network;
using Core.ObjectPool;
using Core.Replicaton;
using Core.SpatialPartition;

namespace App.Server
{
       public class CreateSnapshotTask : BaseRefCounter
    {
        public Contexts _contexts;
        public IBin2DManager Bin;
        public Bin2DConfig Bin2DConfig;
        public INetworkChannel Channel;
        public PlayerEntity Player;
        public List<IGameEntity> PreEntitas = new List<IGameEntity>();
        public int ServerTime;
        public int SnapshotSeq;
        public int Status;
        public int VehicleSimulationTime;
        public ISnapshot Snapshot { get; set; }

        public CreateSnapshotTask Build(PlayerEntity player, Bin2DConfig bin2DConfig,
                                          IBin2DManager bin, int serverTime, int snapshotSeq, int vehicleSimulationTime,
                                          INetworkChannel channel, Contexts _newContexts)
        {
            Player                = player;
            Bin2DConfig           = bin2DConfig;
            Bin                   = bin;
            ServerTime            = serverTime;
            SnapshotSeq           = snapshotSeq;
            VehicleSimulationTime = vehicleSimulationTime;
            Channel               = channel;
            PreEntitas.Clear();
            _contexts = _newContexts;
            Status    = 0;
            return this;
        }

        protected override void OnCleanUp()
        {
            Player                = null;
            Bin2DConfig           = null;
            Bin                   = null;
            ServerTime            = 0;
            SnapshotSeq           = 0;
            VehicleSimulationTime = 0;
            Channel               = null;

            PreEntitas.Clear();
            _contexts = null;
            ObjectAllocatorHolder<CreateSnapshotTask>.Free(this);
        }

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(CreateSnapshotTask))
            {
            }

            public override object MakeObject()
            {
                return new CreateSnapshotTask();
            }
        }
    }
}