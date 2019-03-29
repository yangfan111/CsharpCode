using System;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    [Player]
    [Serializable]
    public class TriggerEventComponent :  IUpdateComponent, IUserPredictionComponent
    {
        [NetworkProperty][DontInitilize] public bool NeedUnmountWeapon;

        public int GetComponentId()
        {
            return (int) EComponentIds.TriggerEvent;
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as TriggerEventComponent;
            NeedUnmountWeapon = right.NeedUnmountWeapon;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as TriggerEventComponent;
            return NeedUnmountWeapon == r.NeedUnmountWeapon;
        }
        
        public void Reset()
        {
            NeedUnmountWeapon = false;
        }

    }
}