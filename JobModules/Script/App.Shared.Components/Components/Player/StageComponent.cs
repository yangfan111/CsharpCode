using Entitas;

namespace App.Shared.Components.Player
{
    public enum EPlayerLoginStage
    {
        CreateEntity,
        EnterRunning,
        Running
    }
    [Player]
    public class StageComponent:IComponent
    {
        public EPlayerLoginStage Value;

        public bool CanSendSnapshot()
        {
            return Value == EPlayerLoginStage.EnterRunning || Value == EPlayerLoginStage.Running;
        }
    }

    [Player]
    public class InitializedComponent : IComponent
    {
        
    }
}