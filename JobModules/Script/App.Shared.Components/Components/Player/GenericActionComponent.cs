using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState;
using Core.Compare;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using App.Shared.Components.Oxygen;
using App.Shared.Components.GenericActions;

namespace App.Shared.Components.Player
{
    [Player]
    
    public class GenericActionComponent : IUserPredictionComponent
    {
        public int GetComponentId()
        {
            return (int)EComponentIds.GenericActionComponent;
        }

        public void CopyFrom(object rightComponent)
        {
        }

        public bool IsApproximatelyEqual(object right)
        {
            return false;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    public class GenericActionInterfaceComponent : IComponent
    {
        public IGenericAction GenericAction;
    }
    
    [Player]
    public class LadderActionInterfaceComponent : IComponent
    {
        public ILadderAction LadderAction;
    }
}
