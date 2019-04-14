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
    public class PlayerClientEventsUpdateComponent : IUpdateComponent
    {
//        [DontInitilize, NetworkProperty] public InterruptData InterruptHoldWeapon;
//        [DontInitilize, NetworkProperty] public InterruptData InterruptGunSightWeapon;
//        [DontInitilize, NetworkProperty] public InterruptData InterruptPullboltWeapon;
        [DontInitilize, NetworkProperty] public bool OpenUIFrame;


        public void Clear(){
//        {
//            InterruptGunSightWeapon.Reset();
//            InterruptPullboltWeapon.Reset();
//            InterruptHoldWeapon.Reset();
            OpenUIFrame = false;
        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as PlayerClientEventsUpdateComponent;
//            InterruptHoldWeapon.CopyFrom(remote.InterruptHoldWeapon);
//            InterruptPullboltWeapon.CopyFrom(remote.InterruptPullboltWeapon);
//            InterruptGunSightWeapon.CopyFrom(remote.InterruptGunSightWeapon);
            OpenUIFrame = remote.OpenUIFrame;
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