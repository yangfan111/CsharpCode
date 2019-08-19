using Core.CharacterBone;
using Core.Components;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Vector3 = UnityEngine.Vector3;

namespace App.Shared.Components.Player
{
    [Player]
    public class CharacterBoneComponent : IPlaybackComponent, IUpdateComponent
    {
        [DontInitilize] [NetworkProperty] public bool EnableIK;
        [DontInitilize] [NetworkProperty(SyncFieldScale.Pitch)] public float PitchHeadAngle;
        [DontInitilize] [NetworkProperty(SyncFieldScale.Yaw)] public float RotHeadAngle;
        [DontInitilize] [NetworkProperty(SyncFieldScale.Pitch)] public float CurrentPitchHandAngle;
        [NetworkProperty(SyncFieldScale.Pitch)] public float WeaponPitch;
        [NetworkProperty(SyncFieldScale.Yaw)] public float WeaponRot;
        [NetworkProperty] public bool IsWeaponRotState;

        [DontInitilize] [NetworkProperty(100,-100,0.00001f)] public Vector3 FirstPersonPositionOffset;
        [DontInitilize] [NetworkProperty(100,-100,0.00001f)] public Vector3 FirstPersonRotationOffset;
        [DontInitilize] [NetworkProperty(100,-100,0.00001f)] public Vector3 FirstPersonSightOffset;

        public float ScreenRatio;
        public int RealWeaponId;
        public bool NeedChangeOffset;

        public int GetComponentId()
        {
            return (int) EComponentIds.CharacterBone;
        }

        public bool IsInterpolateEveryFrame()
        {
            return true;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CharacterBoneComponent leftComponent = (CharacterBoneComponent) left;
            CharacterBoneComponent rightComponent = (CharacterBoneComponent) right;
            var ratio = interpolationInfo.Ratio;

            PitchHeadAngle = leftComponent.PitchHeadAngle +
                             (rightComponent.PitchHeadAngle - leftComponent.PitchHeadAngle) * ratio;
            RotHeadAngle = leftComponent.RotHeadAngle +
                           (rightComponent.RotHeadAngle - leftComponent.RotHeadAngle) * ratio;
            CurrentPitchHandAngle = leftComponent.CurrentPitchHandAngle +
                             (rightComponent.CurrentPitchHandAngle - leftComponent.CurrentPitchHandAngle) * ratio;
            WeaponPitch = leftComponent.WeaponPitch + (rightComponent.WeaponPitch - leftComponent.WeaponPitch) * ratio;
            WeaponRot = leftComponent.WeaponRot + (rightComponent.WeaponRot - leftComponent.WeaponRot) * ratio;
            ScreenRatio = leftComponent.ScreenRatio + (rightComponent.ScreenRatio - leftComponent.ScreenRatio) * ratio;

            FirstPersonPositionOffset = leftComponent.FirstPersonPositionOffset +
                                        (rightComponent.FirstPersonPositionOffset -
                                         leftComponent.FirstPersonPositionOffset) * ratio;
            FirstPersonRotationOffset = leftComponent.FirstPersonRotationOffset +
                                        (rightComponent.FirstPersonRotationOffset -
                                         leftComponent.FirstPersonRotationOffset) * ratio;
            FirstPersonSightOffset = leftComponent.FirstPersonSightOffset +
                                     (rightComponent.FirstPersonSightOffset - leftComponent.FirstPersonSightOffset) *
                                     ratio;

            NeedChangeOffset = leftComponent.NeedChangeOffset;
            RealWeaponId = leftComponent.RealWeaponId;

            EnableIK = leftComponent.EnableIK;
            
            IsWeaponRotState = leftComponent.IsWeaponRotState;
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as CharacterBoneComponent;
            PitchHeadAngle = right.PitchHeadAngle;
            RotHeadAngle = right.RotHeadAngle;
            CurrentPitchHandAngle = right.CurrentPitchHandAngle;
            WeaponPitch = right.WeaponPitch;
            WeaponRot = right.WeaponRot;
            IsWeaponRotState = right.IsWeaponRotState;

            FirstPersonPositionOffset = right.FirstPersonPositionOffset;
            FirstPersonRotationOffset = right.FirstPersonRotationOffset;
            FirstPersonSightOffset = right.FirstPersonSightOffset;
            
            ScreenRatio = right.ScreenRatio;
            RealWeaponId = right.RealWeaponId;
            NeedChangeOffset = right.NeedChangeOffset;

            EnableIK = right.EnableIK;
        }
    }

    [Player]
    public class CharacterBoneInterfaceComponent : IComponent
    {
        public ICharacterBone CharacterBone;
    }
}
