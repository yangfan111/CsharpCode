using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace Core.Prediction.UserPrediction.Parallel
{
    public enum TaskStep
    {
        PreExecute,
        Execute,
        PostExecute
    }

    public enum TaskThread
    {
        MainThread,
        SlaveThread,
        END
        
    }

    public enum TaskStatus
    {
        Running,
        Idel,
        Complete
    }

    public class CmdTask
    {
        public int PayerIdx;
        public IPlayerUserCmdGetter Getter;
        public IUserCmd UserCmd;
        public int ThreadIdx;
        public TaskInfo TaskInfo;


        public CmdTask(int payerIdx, IPlayerUserCmdGetter getter, IUserCmd userCmd, TaskInfo taskInfo, int threadIdx)
        {
            PayerIdx = payerIdx;
            Getter = getter;
            UserCmd = userCmd;
            TaskInfo = taskInfo;
            ThreadIdx = threadIdx;
           
        }

       
    }

  
    public struct TaskInfo
    {
        public int SystemIdx;
        public TaskStep Step;
        public TaskThread Thread;

        public TaskInfo(int systemIdx, TaskStep step, TaskThread thread)
        {
            SystemIdx = systemIdx;
            Step = step;
            Thread = thread;
        }
    }

    public interface IParallelExecute
    {
        void Offer(CmdTask task);
        void AllDone();
        void InternalRun();
        void SetTaskDisparcher(ITaskDisparcher disparcher);
        void StartInternal();
        bool IsStart();
    }
}