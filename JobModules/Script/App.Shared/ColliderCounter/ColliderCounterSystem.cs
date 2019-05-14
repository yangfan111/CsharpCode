using App.Shared;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Utils.Singleton;

namespace App.Shared
{
    public class ColliderCounterSystem:IUserCmdExecuteSystem
    {
        private StaticColliderCounter scCounter;

        public ColliderCounterSystem(Contexts contexts)
        {
            scCounter = SingletonManager.Get<StaticColliderCounter>();
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            scCounter.CalcuColliderNum(cmd.Seq);
        }
    }
}