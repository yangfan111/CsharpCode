using Core;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using WeaponConfigNs;

namespace App.Shared.Components.Weapon
{
    [Weapon, UniquePrefix("is")]
    public class ActiveComponent : IUserPredictionComponent
    {
        public void CopyFrom(object rightComponent)
        {
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.WeaponActiveFlag;
        }

        public bool IsApproximatelyEqual(object right)
        {
            return true;
        }

        public void RewindTo(object rightComponent)
        {
        }
    }

    [Weapon]
    public class WeaponScanComponent : IComponent
    {
        [DontInitilize] public WeaponScanStruct Value;

        public void CopyFrom(WeaponBasicDataComponent basic)
        {
            Value.ConfigId = basic.ConfigId;
            Value.AvatarId = basic.WeaponAvatarId;
            Value.Bullet = basic.Bullet;
            Value.ReservedBullet = basic.ReservedBullet;
        }

        public void CopyFrom(WeaponPartsAchive basic)
        {
            Value.UpperRail = basic.UpperRail;
            Value.LowerRail = basic.LowerRail;
            Value.Magazine = basic.Magazine;
            Value.Muzzle = basic.Muzzle;
            Value.SideRail = basic.SideRail;
            Value.Stock = basic.Stock;
            Value.Bore = basic.Bore;
            Value.Feed = basic.Feed;
            Value.Interlock = basic.Interlock;
            Value.Trigger = basic.Trigger;
            Value.Brake = basic.Brake;
        }
    }

 
    [Weapon]
    public class WeaponClientUpdateComponent : IUpdateComponent
    {
        //[DontInitilize, NetworkProperty] public bool PullBoltEnd;   //是否拉过栓了
        //[DontInitilize, NetworkProperty] public bool IsPullingBolt; //是否在拉栓中
        //[DontInitilize, NetworkProperty] public bool PullBoltInterrupt; 
        //[DontInitilize, NetworkProperty] public bool IsInterruptSightView;
        //[DontInitilize, NetworkProperty] public bool IsRecoverSightView;
        public static readonly                  WeaponClientUpdateComponent Empty = new WeaponClientUpdateComponent();

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponClientUpdateComponent;
            //PullBoltEnd = remote.PullBoltEnd;
            //IsPullingBolt = remote.IsPullingBolt;
            //IsRecoverSightView = remote.IsRecoverSightView;
            //IsInterruptSightView = remote.IsInterruptSightView;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.WeaponClientData;
        }
    }
}