using Core;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace Core.Prediction.UserPrediction.Parallel
{
    class UserCmdPreExecuteSystem : ISimpleParallelUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger =
            new LoggerAdapter(typeof(UserCmdPreExecuteSystem));

//        private PlayerStateCollectorPool _stateCollectorPool;
//
//        public UserCmdPreExecuteSystem(
//        )
//        {
//            _stateCollectorPool = gameStateProcessorFactory;
//        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd userCmd)
        {
            if (userCmd.Seq != owner.LastCmdSeq + 1)
            {
                _logger.ErrorFormat("{2} lost user cmd last {0}, cur {1}", owner.LastCmdSeq,
                    userCmd.Seq, owner.OwnerEntityKey);
            }

            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("processing user cmd {0}", userCmd);
            }

            userCmd.FilteredInput = owner.GetFiltedInput(userCmd);
        }

        public ISimpleParallelUserCmdExecuteSystem CreateCopy()
        {
            return new UserCmdPreExecuteSystem();
        }
    }
}