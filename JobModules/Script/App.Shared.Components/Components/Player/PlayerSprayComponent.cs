using Core.Attack;
using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas;

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
