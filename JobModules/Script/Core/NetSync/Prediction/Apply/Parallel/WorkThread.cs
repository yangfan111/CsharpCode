using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Core.GameModule.Interface;
using Core.Utils;
using Utils.Concurrent;
using Utils.Singleton;

namespace Core.Prediction.UserPrediction.Parallel
{
    public class WorkThread : AbstractThread, IParallelExecute
    {
        private readonly List<IUserCmdExecuteSystem> _systems;
        private readonly List<CustomProfileInfo> _systemsProfiles = new List<CustomProfileInfo>();

        private static LoggerAdapter _logger =
            new LoggerAdapter(typeof(WorkThread));

        private volatile bool _isStart = false;
        private BlockingQueue<CmdTask> _queue = new BlockingQueue<CmdTask>();
        private ITaskDisparcher _disparcher;
        private float _rate;

        public WorkThread(string name, List<IUserCmdExecuteSystem> systems): base(name)
        {
            _systems = systems;
            foreach (var userCmdExecuteSystem in _systems)
            {
                _systemsProfiles.Add(SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(name + "_"+ userCmdExecuteSystem.GetType().Name));
            }
        }

        public void SetTaskDisparcher(ITaskDisparcher disparcher)
        {
            _disparcher = disparcher;
        }

        public void StartInternal()
        {
            lock (this)
            {
                _logger.DebugFormat("Execute :{0} StartInternal, queueCount {1}", Name, _queue.Count);
                _isStart = true;

                _queue.Clear();
            }
        }

        public bool IsStart()
        {
            return _isStart;
        }

        public void Offer(CmdTask task)
        {
            _queue.Enqueue(task);
        }


        public void AllDone()
        {
            lock (this)
            {
                if (_isStart)
                {
                    _logger.DebugFormat("Execute :{0} AllDone", Name);
                    _isStart = false;
                }
            }
        }

        public void InternalRun()
        {
            while (_isStart)
            {
                CmdTask task =  _queue.Dequeue(1);
                if(task == null) continue;
                var systemIdx = task.TaskInfo.SystemIdx;
                try
                {
                    SingletonManager.Get<DurationHelp>().ProfileStart(_systemsProfiles[systemIdx]);
                    var sytstem = _systems[systemIdx];
                    sytstem.ExecuteUserCmd(task.Owner, task.UserCmd);
                    _logger.DebugFormat("{0} Execute :{1} seq:{2} player:{3}", Name, systemIdx, task.UserCmd.Seq,
                        task.Owner.OwnerEntityKey.EntityId);
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("Execute :{0} Exception:{1}", _systems[systemIdx], e);
                }
                finally
                {
                    SingletonManager.Get<DurationHelp>().ProfileEnd(_systemsProfiles[systemIdx]);
                    _disparcher.TaskEnd(task);
                }
            }
        }

        protected override void Run()
        {
            Thread.CurrentThread.Name = GetType().ToString();
            while (Running)
            {
                if (_isStart)
                {
                    InternalRun();
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        public override float Rate
        {
            get { return _rate; }
        }
    }
}