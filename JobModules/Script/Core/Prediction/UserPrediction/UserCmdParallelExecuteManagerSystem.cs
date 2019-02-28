using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Core.EntityComponent;
using Core.GameInputFilter;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using Core.Prediction.UserPrediction.Parallel;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace Core.Prediction.UserPrediction
{
    public class UserCmdParallelExecuteManagerSystem : IExecuteSystem
    {
        private List<IUserCmdExecuteSystem> _systems;
        private List<IUserCmdExecuteSystem>[] _systemsPool;
        private List<TaskInfo> _taskInfos;

        private static LoggerAdapter _logger =
            new LoggerAdapter(LoggerNameHolder<UserCmdParallelExecuteManagerSystem>.LoggerName);

        private IUserCmdExecuteSystemHandler _handler;

        private WorkThread _mainThread;
        private TaskDispatcher _taskDisparcher;


        public UserCmdParallelExecuteManagerSystem(IGameModule gameModule,
            IUserCmdExecuteSystemHandler handler,
            IGameStateProcessorFactory gameStateProcessorFactory,
            int threadCount)
        {
            _systems = new List<IUserCmdExecuteSystem>();
            _taskInfos = new List<TaskInfo>();
            _systems.Add(new UserCmdPreExecuteSystem(gameStateProcessorFactory));
            _systems.AddRange(gameModule.UserCmdExecuteSystems);
            _systems.Add(new UserCmdPostExecuteSystem());

            _handler = handler;
            int count = _systems.Count;
            _systemsPool = new List<IUserCmdExecuteSystem>[threadCount];
            InitTask(threadCount, count);
            _mainThread = new WorkThread("MainThread", _systems);
            WorkThread[] slaveThreads = new WorkThread[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                slaveThreads[i] = new WorkThread(String.Format("SlaveThread_{0}", i) ,_systemsPool[i]);
            }

            _taskDisparcher = new TaskDispatcher(threadCount, _mainThread, slaveThreads, _taskInfos);

            _mainThread.SetTaskDisparcher(_taskDisparcher);
            for (var i = 0; i < threadCount; i++)
            {
                slaveThreads[i].SetTaskDisparcher(_taskDisparcher);
            }

            _taskDisparcher.Start();
        }

        private void InitTask(int threadCount, int count)
        {
            List<TaskInfo> temp = new List<TaskInfo>();
            for (int i = 0; i < threadCount; i++)
            {
                _systemsPool[i] = new List<IUserCmdExecuteSystem>(_systems.Count);
            }

            for (var i = 0; i < count; i++)
            {
                var system = _systems[i];

                if (system is ISimpleParallelUserCmdExecuteSystem)
                {
                    foreach (var userCmdExecuteSystems in _systemsPool)
                    {
                        userCmdExecuteSystems.Add(((ISimpleParallelUserCmdExecuteSystem) system).CreateCopy());
                    }


                    if (system is IComplexParallelUserCmdExecuteSystem)
                    {
                        _taskInfos.Add(new TaskInfo(i, TaskStep.PreExecute, TaskThread.MainThread));
                        _taskInfos.Add(new TaskInfo(i, TaskStep.Execute, TaskThread.SlaveThread));
                        _taskInfos.Add(new TaskInfo(i, TaskStep.PostExecute, TaskThread.MainThread));
                    }
                    else
                    {
                        _taskInfos.Add(new TaskInfo(i, TaskStep.Execute, TaskThread.SlaveThread));
                    }
                }
                else
                {
                    foreach (var userCmdExecuteSystems in _systemsPool)
                    {
                        userCmdExecuteSystems.Add(system);
                    }

                    _taskInfos.Add(new TaskInfo(i, TaskStep.Execute, TaskThread.MainThread));
                }
            }
        }

     
       
        public void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UserPrediction);
                var users = _handler.UserCmdOwnerList;
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UserPredictionCreateTasks);
                _taskDisparcher.CreateTasks(users);
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UserPredictionCreateTasks);
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UserPredictionInternalRun);
                _mainThread.InternalRun();
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UserPredictionInternalRun);
                
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UserPrediction);
            }
        }

        public static void RunSlaveThread()
        {
        }
    }

    public class UserCmdEndExecuteSystem : IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
        }
    }
}