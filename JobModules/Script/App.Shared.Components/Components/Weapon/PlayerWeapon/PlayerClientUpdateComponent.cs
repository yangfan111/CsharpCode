using Core.Components;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Weapon
{
    [Player]
    public class PlayerClientUpdateComponent : IUpdateComponent
    {
        [NetworkProperty] public short FootstepFrameGroup;

        [DontInitilize, NetworkProperty] public float LastSpreadOffsetX;
        [DontInitilize, NetworkProperty] public float LastSpreadOffsetY;
        [DontInitilize, NetworkProperty] public bool DestoryPreparedThrowingEntity;
        [DontInitilize] public bool OpenUIFrameSync;

        [DontInitilize]public byte LastMatType;

        public bool HasFootstepFrame
        {
            get { return FootstepFrameGroup > -1; }
        }


        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as PlayerClientUpdateComponent;
            FootstepFrameGroup = remote.FootstepFrameGroup;
            LastSpreadOffsetY  = remote.LastSpreadOffsetY;
            LastSpreadOffsetX  = remote.LastSpreadOffsetX;
            DestoryPreparedThrowingEntity = remote.DestoryPreparedThrowingEntity;

        }


        public int GetComponentId()
        {
            return (int) EComponentIds.WeaponClientUpdateComponent;
        }

        public void Clear()
        {
            //        {
            //            InterruptGunSightWeapon.Reset();
            //            InterruptPullboltWeapon.Reset();
            //            InterruptHoldWeapon.Reset();
            OpenUIFrameSync        = false;
            FootstepFrameGroup = -1;
            LastSpreadOffsetX  = LastSpreadOffsetY = 0;
            DestoryPreparedThrowingEntity = false;
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