using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using Core.Replicaton;

namespace Core.Prediction
{
    public interface IPredictionBaseInfoProvider
    {
        bool IsReady();
    }
    public interface IPredictionExecuteInfoProvider:IPredictionBaseInfoProvider
    {
       
        IUserCmdOwner UserCmdOwner { get; }
    }
    public interface IPredictionRewindInfoProvider:IPredictionBaseInfoProvider
    {
        IGameEntity GetLocalEntity(EntityKey entityKey);
        ISnapshot LatestSnapshot { get; }
        void DestroyLocalEntity(IGameEntity entity);
        IGameEntity CreateAndGetLocalEntity(EntityKey entityKey);
        EntityMap LocalEntityMap { get; }
        EntityMap RemoteEntityMap { get; }

        int RemoteHistoryId { get; }

        bool IsLatestSnapshotChanged();
        void Update();
        void OnRewind(); // for unit testing
        
    
        bool IsRemoteEntityExists(EntityKey entityKey);

        void AfterPredictionInit(bool isRewinded);
    }
}