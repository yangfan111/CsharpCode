using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.NetworkObject;

namespace Core.Playback
{
    /// <summary>
    /// 支持回放的组件，会同步到客户端用于回放
    /// 需要实现
    /// 差值接口<see cref="IInterpolatableComponent.Interpolate"/> 
    /// 是否需要每一帧进行差值<see cref="IInterpolatableComponent.IsInterpolateEveryFrame"/>
    /// </summary>
    public interface IPlaybackComponent : IGameComponent, INetworkObject, IInterpolatableComponent
    {
        
    }
}