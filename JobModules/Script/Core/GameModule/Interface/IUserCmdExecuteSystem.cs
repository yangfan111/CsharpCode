using Core.Prediction.UserPrediction.Cmd;

namespace Core.GameModule.Interface
{
    
    public interface ISimpleParallelUserCmdExecuteSystem : IUserCmdExecuteSystem
    {
        ISimpleParallelUserCmdExecuteSystem CreateCopy();
    }
    public interface IComplexParallelUserCmdExecuteSystem : ISimpleParallelUserCmdExecuteSystem
    {
        void PreExecuteUserCmd(IPlayerUserCmdGetter owner, IUserCmd cmd);
        void PostExecuteUserCmd(IPlayerUserCmdGetter owner, IUserCmd cmd);
        IComplexParallelUserCmdExecuteSystem CreateCopy();
    }
    public interface IUserCmdExecuteSystem:IUserSystem
    {
        void ExecuteUserCmd(IPlayerUserCmdGetter owner, IUserCmd cmd);
    }
    
   
}