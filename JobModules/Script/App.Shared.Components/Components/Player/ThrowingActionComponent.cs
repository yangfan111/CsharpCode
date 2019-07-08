using Core;
using Core.Components;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;

using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player]
    public class ThrowingActionComponent : IComponent
    {
        [DontInitilize] public ThrowingActionData ActionData;

        public int GetComponentId()
        {
            { { return (int)EComponentIds.PlayerThrowing; } }
        }
    }

    [Player, ]
    public class ThrowingUpdateComponent : IUpdateComponent
    {
        [NetworkProperty] public bool ReadyFly;
        public void CopyFrom(object rightComponent)
        {
            var updateComponent = rightComponent as ThrowingUpdateComponent;
            ReadyFly = updateComponent.ReadyFly; 
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerThrowingUpdateData;
        }
    }

    [Player]
    public class ThrowingLineComponent : IComponent
    {
        public GameObject Go;
    }

    [Player]
    public class ThrowingSphereComponent : SingleAssetComponent
    {
        public override int GetComponentId() { { return (int)EComponentIds.PlayerThrowingSphere; } }
    }
}
