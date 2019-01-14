using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.NetworkObject;

namespace Core.Playback
{
    
    public interface IPlaybackComponent : IGameComponent, INetworkObject, IInterpolatableComponent
    {
        
    }
}