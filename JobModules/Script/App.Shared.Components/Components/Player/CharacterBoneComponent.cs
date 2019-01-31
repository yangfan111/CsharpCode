using Core.CharacterBone;
using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace App.Shared.Components.Player
{
    [Player]
    public class CharacterBoneComponent : IPlaybackComponent, IUpdateComponent
    {
        [DontInitilize] [NetworkProperty] public bool EnableIK;
        [DontInitilize] [NetworkProperty] public float PitchHeadAngle;
        [DontInitilize] [NetworkProperty] public float RotHeadAngle;
        [DontInitilize] [NetworkProperty] public float PitchHandAngle;
        [DontInitilize] [NetworkProperty] public float HeadRotProcess;
        [DontInitilize] [NetworkProperty] public bool IsHeadRotCW;
        [NetworkProperty] public float WeaponPitch;
        [NetworkProperty] public float WeaponRot;

        [DontInitilize] [NetworkProperty] public Vector3 FirstPersonPositionOffset;
        [DontInitilize] [NetworkProperty] public Vector3 FirstPersonRotationOffset;
        [DontInitilize] [NetworkProperty] public Vector3 FirstPersonSightOffset;

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
            PitchHandAngle = leftComponent.PitchHandAngle +
                             (rightComponent.PitchHandAngle - leftComponent.PitchHandAngle) * ratio;
            HeadRotProcess = leftComponent.HeadRotProcess +
                             (rightComponent.HeadRotProcess - leftComponent.HeadRotProcess) * ratio;
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

            IsHeadRotCW = leftComponent.IsHeadRotCW;
            NeedChangeOffset = leftComponent.NeedChangeOffset;
            RealWeaponId = leftComponent.RealWeaponId;

            EnableIK = leftComponent.EnableIK;
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as CharacterBoneComponent;
            PitchHeadAngle = right.PitchHeadAngle;
            RotHeadAngle = right.RotHeadAngle;
            PitchHandAngle = right.PitchHandAngle;
            HeadRotProcess = right.HeadRotProcess;
            IsHeadRotCW = right.IsHeadRotCW;
            WeaponPitch = right.WeaponPitch;
            WeaponRot = right.WeaponRot;

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