using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Weapon
{
  
    [Player,]
    public class PlayerWeaponServerUpdateComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public bool UpdateHeldAppearance;
        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponServerUpdate;
        }
        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as PlayerWeaponServerUpdateComponent;
            return UpdateHeldAppearance == remote.UpdateHeldAppearance;
        }
        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as PlayerWeaponServerUpdateComponent;
            UpdateHeldAppearance = remote.UpdateHeldAppearance;

        }



    }
}