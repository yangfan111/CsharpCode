using Core.EntityComponent;

namespace Core.GameInputFilter
{
    public interface IGameStateProcessorFactory
    {
        IStatePool GetStatePool();
        IStateProviderPool GetProviderPool();
    }
}
