using Core.Components;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    [Player, UniquePrefix("HasAdd")]
    public class RaycastComponent : IComponent
    {

    }

    [Player, ]
    public class RaycastTargetComponent : IUpdateComponent
    {
        [NetworkProperty] public int Key;

        public void CopyFrom(object rightComponent)
        {
            var raycastComponent = rightComponent as RaycastTargetComponent;
            Key = raycastComponent.Key;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerCast;
        }
    }
}
