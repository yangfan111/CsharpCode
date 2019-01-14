using Core.Prediction.UserPrediction.Cmd;

namespace Core.GameModule.Interface
{
    
    public interface ISimpleParallelUserCmdExecuteSystem : IUserCmdExecuteSystem
    {
        ISimpleParallelUserCmdExecuteSystem CreateCopy();
    }
    public interface IComplexParallelUserCmdExecuteSystem : ISimpleParallelUserCmdExecuteSystem
    {
        void PreExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd);
        void PostExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd);
        IComplexParallelUserCmdExecuteSystem CreateCopy();
    }
    public interface IUserCmdExecuteSystem
    {
        void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd);
    }
    
    public interface IBeforeUserCmdExecuteSystem
    {
        void BeforeExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd);
    }
}