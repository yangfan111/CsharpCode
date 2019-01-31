using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Core.CharacterBone
{
    public interface ICharacterBoneState
    {
        bool EnableIK { get; set; }
        float PitchHeadAngle { get; set; }
        float RotHeadAngle { get; set; }
        float PitchHandAngle { get; set; }
        float HeadRotProcess { get; set; }
        bool IsHeadRotCW { get; set; }
        float WeaponPitch { get; set; }
        float WeaponRot { get; set; }
        
        Vector3 FirstPersonPositionOffset { get; set; }
        Vector3 FirstPersonRotationOffset { get; set; }
        Vector3 FirstPersonSightOffset { get; set; }
        
        float ScreenRatio { get; set; }
        int RealWeaponId { get; set; }
        bool NeedChangeOffset { get; set; }
    }
}
