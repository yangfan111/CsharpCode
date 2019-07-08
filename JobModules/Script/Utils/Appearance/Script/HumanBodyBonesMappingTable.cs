using System.Collections.Generic;
using UnityEngine;
using Utils.Utils;

namespace Utils.Appearance.Script
{
    public abstract class HumanBodyBonesMappingTable
    {
        public static Dictionary<HumanBodyBones, string> Table = new Dictionary<HumanBodyBones, string>(
            CommonIntEnumEqualityComparer<HumanBodyBones>.Instance)
        {
            // Body
            {HumanBodyBones.Hips,              "Bip01 Pelvis"},
            {HumanBodyBones.Spine,             "Bip01 Spine"},
            {HumanBodyBones.Chest,             "Bip01 Spine1"},
            // Left Arm
            {HumanBodyBones.LeftShoulder,      "Bip01 L Clavicle"},
            {HumanBodyBones.LeftUpperArm,      "Bip01 L UpperArm"},
            {HumanBodyBones.LeftLowerArm,      "Bip01 L Forearm"},
            {HumanBodyBones.LeftHand,          "Bip01 L Hand"},
            // Right Arm
            {HumanBodyBones.RightShoulder,     "Bip01 R Clavicle"},
            {HumanBodyBones.RightUpperArm,     "Bip01 R UpperArm"},
            {HumanBodyBones.RightLowerArm,     "Bip01 R Forearm"},
            {HumanBodyBones.RightHand,         "Bip01 R Hand"},
            // Left Leg
            {HumanBodyBones.LeftUpperLeg,      "Bip01 L Thigh"},
            {HumanBodyBones.LeftLowerLeg,      "Bip01 L Calf"},
            {HumanBodyBones.LeftFoot,          "Bip01 L Foot"},
            {HumanBodyBones.LeftToes,          "Bip01 L Toe0"},
            // Right Leg
            {HumanBodyBones.RightUpperLeg,      "Bip01 R Thigh"},
            {HumanBodyBones.RightLowerLeg,      "Bip01 R Calf"},
            {HumanBodyBones.RightFoot,          "Bip01 R Foot"},
            {HumanBodyBones.RightToes,          "Bip01 R Toe0"},
            // Head
            {HumanBodyBones.Neck,               "Bip01 Neck"},
            {HumanBodyBones.Head,               "Bip01 Head"},
        };
    }
}
