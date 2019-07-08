using Core.Components;
using Core.EntityComponent;

namespace App.Shared.Components.Player
{
    [Player]
    public class PlayerSprayComponent : IGameComponent
    {
        public float mLastCreateTime;

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerSpray;
        }
    }
}
