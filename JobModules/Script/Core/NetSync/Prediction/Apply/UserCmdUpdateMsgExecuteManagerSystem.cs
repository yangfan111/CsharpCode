﻿using System.Collections.Generic;
using Core;
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
        void SyncToEntity(IPlayerUserCmdGetter getter, UpdateLatestPacakge paackge);
    }

    public class UserCmdUpdateMsgExecuteManagerSystem : AbstractFrameworkSystem<IUserCmdExecuteSystem>
    {
        private static LoggerAdapter _logger =
            new LoggerAdapter(LoggerNameHolder<UserCmdUpdateMsgExecuteManagerSystem>.LoggerName);

        private IServerUserCmdList _handler;
        private IList<IUserCmdExecuteSystem> _systems;
        private IUserCmd _currentCmd;
        private IPlayerUserCmdGetter _currentPlayerUserCmdGetter;
        private ISyncUpdateLatestMsgHandler _syncUpdateLatestMsgHandler;
        private CustomProfileInfo _syncToEntityProfile;
        private CustomProfileInfo _filtedInputProfile;

        public UserCmdUpdateMsgExecuteManagerSystem(IGameModule gameModule,
            IServerUserCmdList handler, ISyncUpdateLatestMsgHandler syncUpdateLatestMsgHandler)
        {
            _systems = gameModule.UserCmdExecuteSystems;
            _handler = handler;
            _syncUpdateLatestMsgHandler = syncUpdateLatestMsgHandler;
            _syncToEntityProfile =
                SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("UserPrediction_SyncToEntity"); 
            _filtedInputProfile =
                SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("UserPrediction_FiltedInput");
            Init();
        }

        public override IList<IUserCmdExecuteSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IUserCmdExecuteSystem system)
        {
            system.ExecuteUserCmd(_currentPlayerUserCmdGetter, _currentCmd);
        }

        public override void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UserPrediction);
                //UserCmd Seq是不通用的，以客户端生成为准，每个客户端不同
                foreach (IPlayerUserCmdGetter owner in _handler.UserCmdOwnerList)
                {
                    _currentPlayerUserCmdGetter = owner;
                    if (!owner.IsEnable())
                    {
                        _logger.ErrorFormat("player {0}is destroyed", owner.OwnerEntityKey);
                        continue;
                    }
                    if (owner.UpdateList.Count > 100)
                    {
                        _logger.ErrorFormat("!!!!!!!!!!!!!!!!!!!Too Many cmd:{0}  {1}",owner.OwnerEntityKey,
                            owner.UpdateList.Count);
                        owner.UpdateList.RemoveRange(0,owner.UpdateList.Count -100);
                    }

                    int executeCount = 0;
                    //一次updatePackage代表一次UserCmd
                    foreach (UpdateLatestPacakge updatePackage in owner.UpdateList)
                    {
                        try
                        {
                            _syncToEntityProfile.BeginProfileOnlyEnableProfile();
                            _syncUpdateLatestMsgHandler.SyncToEntity(_currentPlayerUserCmdGetter, updatePackage);
                        }
                        finally
                        {
                            _syncToEntityProfile.EndProfileOnlyEnableProfile();
                        }
                        //实际UserCmd只有一次而已
                        foreach (var userCmd in owner.UserCmdList)
                        {
                            _currentCmd = userCmd;
                            if (_currentCmd.Seq != owner.LastCmdSeq + 1)
                            {
                                _logger.ErrorFormat("{2} lost user cmd last {0}, cur {1}", owner.LastCmdSeq,
                                    _currentCmd.Seq, owner.OwnerEntityKey);
                            }


                            _logger.DebugFormat("processing user cmd {0}", _currentCmd);

                            try
                            {
                                _filtedInputProfile.BeginProfileOnlyEnableProfile();
                                userCmd.FilteredInput = owner.GetFiltedInput(userCmd);
                            }
                            finally
                            {
                                _filtedInputProfile.EndProfileOnlyEnableProfile();
                            }
                          
                            // _logger.InfoFormat("{0} execute cmd {1} ", update.Head.UserCmdSeq, userCmd.Seq);
                            ExecuteSystems();
                            userCmd.FilteredInput = null;
                            owner.LastCmdSeq = userCmd.Seq;
                        }

                        owner.LastestExecuteUserCmdSeq = updatePackage.Head.LastUserCmdSeq;
                        executeCount++;
                        if (executeCount > MaxEcecutePreFrame)
                        {
                            break;
                        }
                    }
                }
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UserPrediction);
            }
        }

        public int MaxEcecutePreFrame = 20;
    }
}