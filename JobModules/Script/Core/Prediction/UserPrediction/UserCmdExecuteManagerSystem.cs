using System.Collections.Generic;
using Core.GameInputFilter;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace Core.Prediction.UserPrediction
{
    public interface IUserCmdExecuteSystemHandler
    {
        List<IUserCmdOwner> UserCmdOwnerList { get; }
    }

    public class UserCmdExecuteManagerSystem : AbstractFrameworkSystem<IUserCmdExecuteSystem>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<UserCmdExecuteManagerSystem>.LoggerName);
        private IUserCmdExecuteSystemHandler _handler;
        private IList<IUserCmdExecuteSystem> _systems;
        private IUserCmd _currentCmd;
        private IUserCmdOwner _currentUserCmdOwner;
        private IGameStateProcessorFactory _gameStateProcessorFactory;
        private IStateProviderPool _stateProviderPool;
        private IStatePool _statePool;

        public UserCmdExecuteManagerSystem(IGameModule gameModule, 
            IUserCmdExecuteSystemHandler handler, 
            IGameStateProcessorFactory gameStateProcessorFactory)
        {
            _systems = gameModule.UserCmdExecuteSystems;
            _handler = handler;
            _gameStateProcessorFactory = gameStateProcessorFactory;
            _stateProviderPool = gameStateProcessorFactory.GetProviderPool();
            _statePool = gameStateProcessorFactory.GetStatePool();
            Init();
        }

        public override IList<IUserCmdExecuteSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IUserCmdExecuteSystem system)
        {
            system.ExecuteUserCmd(_currentUserCmdOwner, _currentCmd);
        }

        public override void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UserPrediction);
                foreach (IUserCmdOwner owner in _handler.UserCmdOwnerList)
                {
                    _currentUserCmdOwner = owner;
                    foreach (var userCmd in owner.UserCmdList)
                    {
                        //_logger.InfoFormat("{0} execute cmd {1} ", owner.OwnerEntityKey, userCmd.Seq);

                        _currentCmd = userCmd;
                        if (_currentCmd.Seq != owner.LastCmdSeq + 1)
                        {
                            _logger.ErrorFormat("{2} lost user cmd last {0}, cur {1}", owner.LastCmdSeq,
                                _currentCmd.Seq, owner.OwnerEntityKey);
                        }

                        if (_logger.IsDebugEnabled)
                        {
                            _logger.DebugFormat("processing user cmd {0}", _currentCmd);
                        }
                        userCmd.FilteredInput = owner.Filter(userCmd);
                        ExecuteSystems();
                        userCmd.FilteredInput = null;
                        owner.LastCmdSeq = userCmd.Seq;
                    }
                }
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UserPrediction);
            }
        }
    }
}