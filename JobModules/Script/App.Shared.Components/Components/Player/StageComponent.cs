using Entitas;

namespace App.Shared.Components.Player
{
    public enum EPlayerLoginStage
    {
        CreateEntity,
        EnterRunning,
        Running,
        Observer,
        Offline
    }
    [Player]
    public class StageComponent:IComponent
    {
        public EPlayerLoginStage Value;

        public bool CanSendSnapshot()
        {
            return Value == EPlayerLoginStage.EnterRunning || Value == EPlayerLoginStage.Running || Value == EPlayerLoginStage.Observer;
        }
    }

    [Player]
    public class InitializedComponent : IComponent
    {
        
    }
}