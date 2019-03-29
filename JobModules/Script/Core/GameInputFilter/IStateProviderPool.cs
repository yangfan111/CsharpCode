using Core.EntityComponent;

namespace Core.GameInputFilter
{
    public interface IStateProviderPool
    {
        IStateProvider GetStateProvider(EntityKey key);
    }
}
