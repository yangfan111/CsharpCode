using System.Collections.Generic;
using Core;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Weapon
{
    
   
    [Player,]
    public class PlayerWeaponClientUpdateComponent : IUpdateComponent
    {
        [DontInitilize, NetworkProperty] public InterruptData InterruptHoldWeapon;
        [DontInitilize, NetworkProperty] public InterruptData InterruptGunSightWeapon;
        [DontInitilize, NetworkProperty] public InterruptData InterruptPullboltWeapon;
        [DontInitilize, NetworkProperty] public bool UIOpenFrame;


        public void Clear()
        {
            InterruptGunSightWeapon.Reset();
            InterruptPullboltWeapon.Reset();
            InterruptHoldWeapon.Reset();
            UIOpenFrame = false;
        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as PlayerWeaponClientUpdateComponent;
            InterruptHoldWeapon.CopyFrom(remote.InterruptHoldWeapon);
            InterruptPullboltWeapon.CopyFrom(remote.InterruptPullboltWeapon);
            InterruptGunSightWeapon.CopyFrom(remote.InterruptGunSightWeapon);
            UIOpenFrame = remote.UIOpenFrame;
        }

        public InterruptData this[EInterruptType type]
        {
            get
            {
                switch (type)
                {
                    case EInterruptType.GunSight:
                        return InterruptGunSightWeapon;
                    case EInterruptType.HoldWeapon:
                        return InterruptHoldWeapon;
                    default:
                        return InterruptPullboltWeapon;
                }
            }
            set
            {
                switch (type)
                {
                    case EInterruptType.GunSight:
                        InterruptGunSightWeapon = value;
                        break;
                    case EInterruptType.HoldWeapon:
                        InterruptHoldWeapon = value;
                        break;
                    case EInterruptType.Pullbolt:
                        InterruptPullboltWeapon = value;
                        break;

                }
            }
        }
        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponClientUpdateComponent;
        }

        //        public void RewindTo(object rightComponent)
        //        {
        //            CopyFrom(rightComponent);
        //        }
        //
        //        public bool IsApproximatelyEqual(object right)
        //        {
        //            var rightC = right as PlayerWeaponUpdateComponent;
        //            return rightC.UpdateHeldAppearance == UpdateHeldAppearance && rightC.TacticWeapon == TacticWeapon;
        //        }
    }
}