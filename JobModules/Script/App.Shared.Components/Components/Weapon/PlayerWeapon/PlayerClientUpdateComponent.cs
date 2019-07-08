using Core.Components;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Weapon
{
    [Player,]
    public class PlayerClientUpdateComponent : IUpdateComponent
    {
//        [DontInitilize, NetworkProperty] public InterruptData InterruptHoldWeapon;
//        [DontInitilize, NetworkProperty] public InterruptData InterruptGunSightWeapon;
//        [DontInitilize, NetworkProperty] public InterruptData InterruptPullboltWeapon;
        [DontInitilize, NetworkProperty] public bool OpenUIFrame;
        [NetworkProperty] public short FootstepFrameGroup;
        public bool HasFootstepFrame
        {
            get { return FootstepFrameGroup > -1; }
        }

        public void Clear(){
//        {
//            InterruptGunSightWeapon.Reset();
//            InterruptPullboltWeapon.Reset();
//            InterruptHoldWeapon.Reset();
            OpenUIFrame = false;
            FootstepFrameGroup = -1;
        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as PlayerClientUpdateComponent;
            OpenUIFrame = remote.OpenUIFrame;
            FootstepFrameGroup = remote.FootstepFrameGroup;
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