using Entitas;

namespace App.Shared.Components.Player
{
    public enum EPlayerLoginStage
    {
        CreateEntity,
        EnterRunning,
        WaitStart,
        Running,
        Observer,
        Account,
        Offline
    }
    [Player]
    public class StageComponent:IComponent
    {
        public EPlayerLoginStage Value;

        public bool CanSendSnapshot()
        {
            return Value == EPlayerLoginStage.EnterRunning || Value == EPlayerLoginStage.Running || Value == EPlayerLoginStage.WaitStart ||
                   Value == EPlayerLoginStage.Observer || Value == EPlayerLoginStage.Account;
        }

        public bool IsAccountStage()
        {
            return Value == EPlayerLoginStage.Account;
        }

        public bool IsWaitStage()
        {
            return Value == EPlayerLoginStage.WaitStart;
        }
    }

    [Player]
    public class InitializedComponent : IComponent
    {
        
    }
}