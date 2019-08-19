using Core.Compare;
using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.NetworkObject;

namespace Core.Prediction.UserPrediction
{
   
    /**
     *不要直接继承这个接口,这个接口不支持预测回滚
     * 
     */
    public interface IPredictionComponent : IGameComponent, INetworkObject, IRewindableComponent, IComparableComponent
    {
        
    }

    public interface IUserPredictionComponent : IPredictionComponent
    {
        
    }
}