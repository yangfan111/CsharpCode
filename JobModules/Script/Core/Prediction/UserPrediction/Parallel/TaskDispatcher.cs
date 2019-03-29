using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace Core.Prediction.UserPrediction.Parallel
{
    public interface ITaskDisparcher
    {
        void CreateTasks(List<IUserCmdOwner> users);
        void TaskEnd(CmdTask task);
    }

    class TaskDispatcher : AbstractThread, ITaskDisparcher
    {
        private static LoggerAdapter _logger =
            new LoggerAdapter(typeof(TaskDispatcher));


        private List<TaskInfo> _taskInfos;
        private int _threadCount;
        private float _rate;
        private WorkThread[] _slaveThreads;
        IParallelExecute _mainThread;

        public TaskDispatcher(int threadCount, IParallelExecute mainThread, WorkThread[] slaveThreads,
            List<TaskInfo> taskInfos): base("TaskDispatcher")
        {
            _threadCount = threadCount;
            _slaveThreads = new WorkThread[threadCount];
            _mainThread = mainThread;

            _taskInfos = taskInfos;
            _slaveThreads = slaveThreads;
            for (int i = 0; i < MaxUserNum; i++)
            {
                _taskDictionary[i] = new Queue<CmdTask>(128);
            }
        }

        private const int MaxUserNum = 200;
        private Queue<CmdTask>[] _taskDictionary = new Queue<CmdTask>[MaxUserNum];

        private volatile int[] _taskRunCounts = new int[MaxUserNum];
        private volatile bool _isStart;
        private volatile int _userCount;

        public bool IsStart
        {
            get { return _isStart; }
            private set { _isStart = value; }
        }

        public void CreateTasks(List<IUserCmdOwner> users)
        {
            int count = users.Count;
            int threadIdx = 0;
            for (int i = 0; i < MaxUserNum; i++)
            {
                if (_taskDictionary[i].Count > 0)
                    _taskDictionary[i].Clear();
                Interlocked.Exchange(ref _taskRunCounts[i], 0);
            }

            int taskCount = 0;
            IsStart = false;
            _userCount = count;
            for (var i = 0; i < count; i++)
            {
                IUserCmdOwner owner = users[i];
                var queue = _taskDictionary[i];
                foreach (var userCmd in owner.UserCmdList)
                {
                    foreach (var taskInfo in _taskInfos)
                    {
                        taskCount++;
                        queue.Enqueue(new CmdTask(i, owner, userCmd, taskInfo, threadIdx));
                    }
                }


                threadIdx++;
                threadIdx = threadIdx % _threadCount;
            }

            _logger.DebugFormat("CreateTasks count:{0}", taskCount);
            if (taskCount > 0)
            {
                _mainThread.StartInternal();
                for (var i = 0; i < _threadCount; i++)
                {
                    _slaveThreads[i].StartInternal();
                }

                IsStart = true;
            }
        }


        public void TaskEnd(CmdTask cmdTask)
        {
            var idx = cmdTask.PayerIdx;

            Interlocked.Decrement(ref _taskRunCounts[idx]);
            _logger.DebugFormat("{0} TaskEnd :{1} statue:{1}", idx, cmdTask.TaskInfo.SystemIdx, _taskRunCounts[idx]);
        }


        protected override void Run()
        {
            Thread.CurrentThread.Name = GetType().ToString();
            while (Running)
            {
                if (IsStart)
                {
                    try
                    {
                        int count = _userCount;
                        bool isAllDone = true;
                        for (int playerIdx = 0; playerIdx < count; playerIdx++)
                        {
                            var queue = _taskDictionary[playerIdx];
                            if (_taskRunCounts[playerIdx] == 0 && queue.Count == 0) //提交的任务和队列的任务个数都为0
                            {
                                continue;
                            }

                            isAllDone = false;

                            if (_taskRunCounts[playerIdx] == 0)
                            {
                                bool flag = true;
                                int lastThreadIdx = -1;
                                TaskThread lastThread = TaskThread.END;
                                int batchCount = 0;
                                bool hasTask = false;
                                while (flag)
                                {
                                    flag = false;

                                    if (queue.Count > 0)
                                    {
                                        CmdTask task = (CmdTask) queue.Peek();
                                        hasTask = true;
                                        if (lastThread == TaskThread.END)
                                        {
                                            lastThread = task.TaskInfo.Thread;
                                            lastThreadIdx = task.ThreadIdx;
                                        }

                                        if (task.TaskInfo.Thread == lastThread && task.ThreadIdx == lastThreadIdx)
                                        {
                                            batchCount++;
                                            flag = true;
                                            task = (CmdTask) queue.Dequeue();

                                            Interlocked.Increment(ref _taskRunCounts[playerIdx]);

                                            _logger.DebugFormat("Offer :{0} thread:{1} batch{2} _taskRunCounts{3}",
                                                task.TaskInfo.SystemIdx, task.TaskInfo.Thread, batchCount,
                                                _taskRunCounts[playerIdx]);
                                            DispatchTask(task);
                                        }
                                    }
                                }

                                if (!hasTask)
                                {
                                    _logger.DebugFormat("{0} no more tasks", playerIdx);
                                }
                            }
                        }

                        if (isAllDone)
                        {
                            NotifyALlThreadAllDone();
                            IsStart = false;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("{0}", e);
                        NotifyALlThreadAllDone();
                        IsStart = false;
                    }
                }
                else
                {
//                    NotifyALlThreadAllDone();
//                    isStart = false;
                    Thread.Sleep(1);
                }
            }
        }

        private void DispatchTask(CmdTask task)
        {
            if (task.TaskInfo.Thread == TaskThread.MainThread)
            {
                _mainThread.Offer(task);
            }
            else
            {
                _slaveThreads[task.ThreadIdx].Offer(task);
            }
        }

        private void NotifyALlThreadAllDone()
        {
            _mainThread.AllDone();
            foreach (var parallelExecute in _slaveThreads)
            {
                parallelExecute.AllDone();
            }
        }


        public void DispatcherTask()
        {
        }

        public override float Rate
        {
            get { return _rate; }
        }

        public override void Stop()
        {
            base.Stop();
            if (_slaveThreads != null)
            {
                foreach (var slaveThread in _slaveThreads)
                {
                    if (slaveThread != null)
                    {
                        slaveThread.Stop();
                    }
                }
            }
        }

        public override void Start()
        {
            base.Start();
            if (_slaveThreads != null)
            {
                foreach (var slaveThread in _slaveThreads)
                {
                    if (slaveThread != null)
                    {
                        slaveThread.Start();
                    }
                }
            }
        }
    }
}