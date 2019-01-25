using Core.EntityComponent;
using Core.Prediction;
using Core.SnapshotReplication.Serialization.NetworkObject;

namespace Core.SyncLatest
{
    

    public interface IVehicleLatestComponent : ILatestComponent,IGameComponent,INetworkObject
    {
        bool IsSyncLatest { get; set; }
    }

    public interface ISelfLatestComponent : ILatestComponent, IGameComponent, INetworkObject
    {

    }

    public interface INonSelfLatestComponent : ILatestComponent, IGameComponent, INetworkObject
    {

    }
}