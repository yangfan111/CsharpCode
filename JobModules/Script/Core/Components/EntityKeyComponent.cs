using Core.EntityComponent;
using Entitas.CodeGeneration.Attributes;

namespace Core.Components
{
    
    public class EntityKeyComponent : IGameComponent
    {
        [PrimaryEntityIndex(typeof(EntityKeyComparer))]
        public EntityKey Value;

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }

        public int GetComponentId() { return (int)ECoreComponentIds.EntityKey; } 
    }
}
