using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Core.CharacterBone
{
    public interface ICharacterBoneState
    {
        float PitchHeadAngle { get; set; }
        float RotHeadAngle { get; set; }
        float PitchHandAngle { get; set; }
        float HeadRotProcess { get; set; }
        bool IsHeadRotCW { get; set; }
        float WeaponPitch { get; set; }

        //Quaternion NeckP3Pitch { get; set; }
        //Quaternion HeadP3Pitch { get; set; }
        //Quaternion NeckP3Yaw { get; set; }
        //Quaternion HeadP3Yaw { get; set; }
    }
}
