using System.Collections.Generic;
using UnityEngine;
using Utils.Utils;

namespace Utils.Appearance
{
    public static class BoneName
    {
        // 人物骨骼
        public static string CharacterBipPelvisBoneName = "Bip01 Pelvis";
        public static string CharacterHeadBoneName = "Bip01 Head";
        public static string CharacterNeckBoneName = "Bip01 Neck";
        public static string CharacterLeftHandName = "Bip01 L Hand";
        public static string CharacterRightHandName = "Bip01 R Hand";
        public static string CharacterBipPelvisName = "Bip01 Pelvis";
        public static string CharacterSpineName = "Bip01 Spine";
        public static string CharacterSpine1Name = "Bip01 Spine1";
        public static string CharacterLeftClavicleName = "Bip01 L Clavicle";
        public static string CharacterRightClavicleName = "Bip01 R Clavicle";

        // 人物的挂载点
        public static string CharLeftHand = "LeftWeaponLocator";
        public static string CharRightHand = "RightWeaponLocator";
        public static string PrimaryWeaponOneOnCharacter = "WeaponAttachment1";
        public static string PrimaryWeaponTwoOnCharacter = "WeaponAttachment2";
        public static string SideArmOnCharacter = "WeaponAttachment3";
        public static string MeleeWeaponOnCharacter = "WeaponAttachment4";
        public static string ThrownWeaponOnCharacter = "WeaponAttachment5";
        public static string TacticWeaponOnCharacter = "WeaponAttachment8";
        public static string FirstPersonHandLocator = "BaseLocator";
        public static string FirstPersonSubHandLocator = "DirectionLocator";
        public static string FirstPersonCameraLocator = "CameraLocator";
        public static string AlternativeWeaponLocator = "BaseWeaponLocator";
        public static string ViewPoint = "ViewPoint";

        // 武器的挂载点
        public static string WeaponRightHand = "RightHandLocator";
        public static string WeaponLeftHand = "LeftHandLocator";
        public static string WeaponLeftIK = "LeftHandleLocator";
        public static string AttachmentLeftIKP1 = "LeftHandleLocator_P1";
        public static string AttachmentLeftIKP3 = "LeftHandleLocator_P3";
        public static string EjectionSocket = "EjectionSocket";
        // 机瞄瞄准点
        public static string WeaponSight = "SightsLocator";
        // 瞄具瞄准点
        public static string AttachmentSight = "SightsLocator1";
        public static string MuzzleSocket1 = "MuzzleSocket1";
        public static string MuzzleSocket = "MuzzleSocket";
        public static string MuzzleLocator = "MuzzleAttachmentSocket";
        public static string LowRailLocator = "UnderAttachmentSocket";
        public static string MagazineLocator = "MagazineAttachmentSocket";
        public static string ButtstockLocator = "GunstockAttachmentSocket";
        public static string ScopeLocator = "UpperAttachmentSocket";
        public static string WeaponEffectLocator = "Effect1";
        
        // 可隐藏机瞄
        public static string RemovableIronSights = "TWS";
        // 可隐藏导轨
        public static string RemovableRail = "Rail";

        // 配件/装备的对齐点
        public static string AttachmentLocator = "AttachmentLocator";

        //背包上武器挂载点
        public static string PrimaryWeaponOneOnBag = "WeaponAttachment6";
        public static string PrimaryWeaponTwoOnBag = "WeaponAttachment7";
        public static string WeaponAttachment6 = "WeaponAttachment6";
        public static string WeaponAttachment7 = "WeaponAttachment7";

        //载具挂载点
        public static string SteeringWheelLeftIK = "VehiclesLeftHandleLocator";
        public static string SteeringWheelRightIK = "VehiclesRightHandleLocator";

        public static Dictionary<HumanBodyBones, string> HumanBodyBoneToCustomBone = new Dictionary<HumanBodyBones, string>(CommonIntEnumEqualityComparer<HumanBodyBones>.Instance)
        {
            { HumanBodyBones.Head, CharacterHeadBoneName },
        };
    }
}
