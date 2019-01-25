using Core.GameInputFilter;
using Entitas;

namespace App.Shared.Components.Player
{
    [Player]
    public class PlayerStateProviderComponent : IComponent
    {
        public IStateProvider Provider; 
    }
}   