using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace Core.Prediction.UserPrediction.Parallel
{
    class UserCmdPostExecuteSystem : ISimpleParallelUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd userCmd)
        {
            userCmd.FilteredInput = null;
            getter.LastCmdSeq = userCmd.Seq;
        }

        public ISimpleParallelUserCmdExecuteSystem CreateCopy()
        {
            return new UserCmdPostExecuteSystem();
        }
    }
}