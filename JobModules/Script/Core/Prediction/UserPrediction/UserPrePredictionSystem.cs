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
#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
    public class UserPrePredictionSystem : AbstractFrameworkSystem<IBeforeUserCmdExecuteSystem>
    {
        private LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<IBeforeUserCmdExecuteSystem>.LoggerName);
        private IList<IBeforeUserCmdExecuteSystem> _systems;

        private IUserCmd _currentCmd;
        private IPredictionInitManager _predictionInitManager;
        private IUserPredictionInfoProvider _predicatoinInfoProvider;
        private IGameStateProcessorFactory _gameStateProcessorFactory;
        private IStateProviderPool _stateProviderPool;
        private IStatePool _statePool;

        public UserPrePredictionSystem(IGameModule gameModule, 
            IUserPredictionInfoProvider handler, 
            IPredictionInitManager predictionInitManager,
            IGameStateProcessorFactory gameStateProcessorFactory)
        {
            _logger.Info("start");
            _predicatoinInfoProvider = handler;
            _predictionInitManager = predictionInitManager;
            _gameStateProcessorFactory = gameStateProcessorFactory;
            _stateProviderPool = gameStateProcessorFactory.GetProviderPool();
            _statePool = gameStateProcessorFactory.GetStatePool();
            _systems = gameModule.BeforeUserCmdExecuteSystems;
            Init();
        }


        public override IList<IBeforeUserCmdExecuteSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IBeforeUserCmdExecuteSystem system)
        {
            system.BeforeExecuteUserCmd(_predicatoinInfoProvider.UserCmdOwner, _currentCmd);
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
                var cmd = owner.LastTempCmd;
                
                if (cmd == null) return;
                _currentCmd = cmd;
                //过滤输入状态
                cmd.FilteredInput = owner.Filter(cmd);
                ExecuteSystems();
                cmd.FilteredInput = null;
                //cmd.PredicatedOnce = true;
                //owner.LastCmdSeq = cmd.Seq;
                //_predictionInitManager.SavePredictionCompoments(cmd.Seq);
    
            
                
            }
            finally
            {

                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UserPrediction);
            }

        }
    }
}
