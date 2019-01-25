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
    public class UserPredictionSystem : AbstractFrameworkSystem<IUserCmdExecuteSystem>
    {
        private LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<UserPredictionSystem>.LoggerName);
        private IList<IUserCmdExecuteSystem> _systems;

        private IUserCmd _currentCmd;
        private IPredictionInitManager _predictionInitManager;
        private IPredictionExecuteInfoProvider _predicatoinInfoProvider;
        private IGameStateProcessorFactory _gameStateProcessorFactory;
        private IStateProviderPool _stateProviderPool;
        private IStatePool _statePool;

        public UserPredictionSystem(IGameModule gameModule, 
            IPredictionExecuteInfoProvider handler, 
            IPredictionInitManager predictionInitManager,
            IGameStateProcessorFactory gameStateProcessorFactory)
        {
            _logger.Info("start");
            _predicatoinInfoProvider = handler;
            _predictionInitManager = predictionInitManager;
            _systems = gameModule.UserCmdExecuteSystems;
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
            system.ExecuteUserCmd(_predicatoinInfoProvider.UserCmdOwner, _currentCmd);
        }


        public override void Execute()
        {
            try
            {
                if (!isStepExecute()) return;
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UserPrediction);
                if (!_predicatoinInfoProvider.IsReady())
                {
                    return;
                }

                var owner = _predicatoinInfoProvider.UserCmdOwner;
                bool PredicatedOnce = false;
                foreach (var cmd in owner.UserCmdList)
                {
                    _currentCmd = cmd;
                    if (_currentCmd.Seq > owner.LastCmdSeq)
                    {
                        //过滤输入状态
                        cmd.FilteredInput = owner.Filter(cmd);
                        if (_logger.IsDebugEnabled)
                        {
                            _logger.DebugFormat("processing user cmd {0}", cmd.ToString());
                        }
                        //如果当前是回滚帧数，就执行高帧数要执行的命令。
                        PredicatedOnce |= cmd.PredicatedOnce;
                        cmd.NeedStepPredication = cmd.PredicatedOnce | PredicatedOnce;
                        ExecuteSystems();
                        cmd.FilteredInput = null;
                        cmd.PredicatedOnce = true;
                        owner.LastCmdSeq = cmd.Seq;
                        _predictionInitManager.SavePredictionCompoments(cmd.Seq);

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
