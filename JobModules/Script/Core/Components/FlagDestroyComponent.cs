using Core.EntityComponent;

namespace Core.Components
{
    
    public class FlagDestroyComponent : IGameComponent
    {
        public int GetComponentId() { { return (int)ECoreComponentIds.FlagDestroy; } }
    }
}