using Core.GameInputFilter;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace Core.Prediction.UserPrediction.Parallel
{
    class UserCmdPreExecuteSystem : ISimpleParallelUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger =
            new LoggerAdapter(typeof(UserCmdPreExecuteSystem));

        private IGameStateProcessorFactory _gameStateProcessorFactory;
        private IStateProviderPool _stateProviderPool;


        public UserCmdPreExecuteSystem(IGameStateProcessorFactory gameStateProcessorFactory
        )
        {
            _gameStateProcessorFactory = gameStateProcessorFactory;
            _stateProviderPool = gameStateProcessorFactory.GetProviderPool();
        }

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

            userCmd.FilteredInput = owner.Filter(userCmd);
        }

        public ISimpleParallelUserCmdExecuteSystem CreateCopy()
        {
            return new UserCmdPreExecuteSystem(_gameStateProcessorFactory);
        }
    }
}