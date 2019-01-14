using System.Collections.Generic;
using Core.GameInputFilter;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using Core.UpdateLatest;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace Core.Prediction.UserPrediction
{
    public interface ISyncUpdateLatestMsgHandler
    {
        void SyncToEntity(IUserCmdOwner owner, UpdateLatestPacakge paackge);
    }

    public class UserCmdUpdateMsgExecuteManagerSystem : AbstractFrameworkSystem<IUserCmdExecuteSystem>
    {
        private static LoggerAdapter _logger =
            new LoggerAdapter(LoggerNameHolder<UserCmdUpdateMsgExecuteManagerSystem>.LoggerName);

        private IUserCmdExecuteSystemHandler _handler;
        private IList<IUserCmdExecuteSystem> _systems;
        private IUserCmd _currentCmd;
        private IUserCmdOwner _currentUserCmdOwner;
        private ISyncUpdateLatestMsgHandler _syncUpdateLatestMsgHandler;

        public UserCmdUpdateMsgExecuteManagerSystem(IGameModule gameModule,
            IUserCmdExecuteSystemHandler handler, ISyncUpdateLatestMsgHandler syncUpdateLatestMsgHandler)
        {
            _systems = gameModule.UserCmdExecuteSystems;
            _handler = handler;
            _syncUpdateLatestMsgHandler = syncUpdateLatestMsgHandler;

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
                    foreach (var update in owner.UpdateList)
                    {
                        _syncUpdateLatestMsgHandler.SyncToEntity(_currentUserCmdOwner, update);

                        foreach (var userCmd in owner.UserCmdList)
                        {
                            _currentCmd = userCmd;
                            if (_currentCmd.Seq != owner.LastCmdSeq + 1)
                            {
                                _logger.ErrorFormat("{2} lost user cmd last {0}, cur {1}", owner.LastCmdSeq,
                                    _currentCmd.Seq, owner.OwnerEntityKey);
                            }


                            _logger.DebugFormat("processing user cmd {0}", _currentCmd);


                            userCmd.FilteredInput = owner.Filter(userCmd);
                            // _logger.InfoFormat("{0} execute cmd {1} ", update.Head.UserCmdSeq, userCmd.Seq);
                            ExecuteSystems();
                            userCmd.FilteredInput = null;
                            owner.LastCmdSeq = userCmd.Seq;
                        }

                        owner.LastestExecuteUserCmdSeq = update.Head.UserCmdSeq;
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