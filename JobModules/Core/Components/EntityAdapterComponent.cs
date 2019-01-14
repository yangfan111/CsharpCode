using Core.EntityComponent;

namespace Core.Components
{
    public interface FakeComponent
    {
        
    }
    
    public class EntityAdapterComponent : IGameComponent
    {
        public IGameEntity SelfAdapter;
        public int GetComponentId() { return (int)ECoreComponentIds.EntityAdapter; } 
    }
}