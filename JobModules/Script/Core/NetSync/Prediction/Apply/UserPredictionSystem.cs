using System.Collections.Generic;
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
        private IUserCmd currentCmd;
        private LoggerAdapter logger = new LoggerAdapter("UserPrediction");
        private PredictionManager predictionManager;

        private AbstractPredictionProvider provider;

        private IList<IUserCmdExecuteSystem> systems;
        //        private PlayerStateCollectorPool gameStateProcessorFactory;
        //        private IStateProviderPool stateProviderPool;
        //        private IPlayerStateMap playerStateMap;

        public UserPredictionSystem(IGameModule gameModule, AbstractPredictionProvider provider,
                                    PredictionManager predictionManager)
        {
            logger.Info("start");
            this.provider          = provider;
            this.predictionManager = predictionManager;
            systems                = gameModule.UserCmdExecuteSystems;
            //            gameStateProcessorFactory = gameStateProcessorFactory;
            //            stateProviderPool = gameStateProcessorFactory.GetProviderPool();
            //            playerStateMap = gameStateProcessorFactory.GetStatePool();
            Init();
        }


        public override IList<IUserCmdExecuteSystem> Systems
        {
            get { return systems; }
        }
        /// remoteOwnerEntity.UserOwner 
        public override void SingleExecute(IUserCmdExecuteSystem system)
        {
            system.ExecuteUserCmd(provider.UserCmdOwner, currentCmd);
        }


        public override void Execute()
        {
            try
            {
                if (!isStepExecute()) return;
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UserPrediction);
                if (!provider.IsReady())
                {
                    logger.InfoFormat("predicatoinInfoProvider is not ready");
                    return;
                }
                //当前playerContexts玩家的UserCmdPrediction，可能与ewindProvider.RemoteHistoryId不一致！！！！！
                IPlayerUserCmdGetter playerUserCmdGetter = provider.UserCmdOwner;
                // logger.InfoFormat("processing user cmd last:{0} {1} {2}", owner.LastCmdSeq, owner.UserCmdList.Count, owner.UserCmdList.Count>0?owner.UserCmdList.Last().Seq:-1);
                foreach (var cmd in playerUserCmdGetter.UserCmdList)
                {
                    currentCmd = cmd;
                    if (currentCmd.Seq > playerUserCmdGetter.LastCmdSeq)
                    {
                        //过滤输入状态
                        cmd.FilteredInput = playerUserCmdGetter.GetFiltedInput(cmd);

                        //logger.InfoFormat("processing user cmd {0}", cmd);

                        //如果当前是回滚帧数，就执行高帧数要执行的命令。

                        ExecuteSystems();
                        cmd.FilteredInput  = null;
                        cmd.PredicatedOnce = true;
                        playerUserCmdGetter.LastCmdSeq   = cmd.Seq;
                        predictionManager.SavePredictionCompoments(cmd.Seq);
                     
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