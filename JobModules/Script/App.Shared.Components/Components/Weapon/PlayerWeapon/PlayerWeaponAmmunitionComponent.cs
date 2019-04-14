using System.Collections.Generic;
using Core;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Weapon
{
    
    /// <summary>
   
    [Player,]
    public class PlayerWeaponAmmunitionComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public int ReservedBullet300;
        [DontInitilize, NetworkProperty] public int ReservedBullet45;
        [DontInitilize, NetworkProperty] public int ReservedBullet50;
        [DontInitilize, NetworkProperty] public int ReservedBullet556;
        [DontInitilize, NetworkProperty] public int ReservedBullet762;
        [DontInitilize, NetworkProperty] public int ReservedBullet9;
        [DontInitilize, NetworkProperty] public int ReservedBullet12;
        [DontInitilize, NetworkProperty] public int ReservedBullet57;

        public void CopyFrom(object rightComponent)
        {
            var equip = rightComponent as PlayerWeaponAmmunitionComponent;
            if (null == equip)
            {
                return;
            }

            ReservedBullet300 = equip.ReservedBullet300;
            ReservedBullet45 = equip.ReservedBullet45;
            ReservedBullet50 = equip.ReservedBullet50;
            ReservedBullet556 = equip.ReservedBullet556;
            ReservedBullet762 = equip.ReservedBullet762;
            ReservedBullet9 = equip.ReservedBullet9;
            ReservedBullet12 = equip.ReservedBullet12;
            ReservedBullet57 = equip.ReservedBullet57;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponAmmunition;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var comp = right as PlayerWeaponAmmunitionComponent;
            return
                ReservedBullet12 == comp.ReservedBullet12
             && ReservedBullet300 == comp.ReservedBullet300
             && ReservedBullet45 == comp.ReservedBullet45
             && ReservedBullet50 == comp.ReservedBullet50
             && ReservedBullet556 == comp.ReservedBullet556
             && ReservedBullet762 == comp.ReservedBullet762
             && ReservedBullet9 == comp.ReservedBullet9
             && ReservedBullet57 == comp.ReservedBullet57;
        }
    }

}