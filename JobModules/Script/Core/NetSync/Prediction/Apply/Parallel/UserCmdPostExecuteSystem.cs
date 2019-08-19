using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace Core.Prediction.UserPrediction.Parallel
{
    class UserCmdPostExecuteSystem : ISimpleParallelUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd userCmd)
        {
            userCmd.FilteredInput = null;
            owner.LastCmdSeq = userCmd.Seq;
        }

        public ISimpleParallelUserCmdExecuteSystem CreateCopy()
        {
            return new UserCmdPostExecuteSystem();
        }
    }
}