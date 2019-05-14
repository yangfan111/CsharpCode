using Core;
using Core.Attack;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using WeaponConfigNs;

namespace App.Shared.Components.Player
{
    [Player]
    public class MeleeAttackInfoComponent : IComponent
    {
        [DontInitilize] public MeleeAttackInfo AttackInfo;
        [DontInitilize] public MeleeFireLogicConfig AttackConfig;
    }

    [Player, ]
    public class MeleeAttackInfoSyncComponent : IUserPredictionComponent 
    {
        [NetworkProperty] public int AttackTime;
        [DontInitilize] public int BeforeAttackTime;
        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerMeleeAttacker;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var comp = right as MeleeAttackInfoSyncComponent;
            return AttackTime == comp.AttackTime;
        }

        public void CopyFrom(object rightComponent)
        {
            var comp = rightComponent as MeleeAttackInfoSyncComponent;
            AttackTime = comp.AttackTime; 
        }

        public void RewindTo(object rightComponent)
        {
           CopyFrom(rightComponent);
        }
    }
}
