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

namespace App.Shared.Components.Player
{
    [Player]
    
    public class OxygenEnergyComponent : IUserPredictionComponent, IPredictedOxygenState
    {
        [NetworkProperty(1000,-1000,0.01f)] public float CurrentOxygen { get; set; }

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerOxygenEnergy;
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as OxygenEnergyComponent;
            if (right != null)
            {
                CurrentOxygen = right.CurrentOxygen;
            }
        }

        public bool IsApproximatelyEqual(object right)
        {
            var rightObj = right as OxygenEnergyComponent;
            if (rightObj != null)
            {
                return CompareUtility.IsApproximatelyEqual(CurrentOxygen, rightObj.CurrentOxygen, 0.02f);
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("CurrentOxygen: {0}", CurrentOxygen);
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    public class OxygenEnergyInterfaceComponent : IComponent
    {
        public IOxygenEnergy Oxygen;
    }
}
