using Core.EntityComponent;
using Entitas;

namespace App.Shared.Components.Sound
{
    [Sound]
    public class ParentComponent : IComponent
    {
        public EntityKey Value;
    }
}
