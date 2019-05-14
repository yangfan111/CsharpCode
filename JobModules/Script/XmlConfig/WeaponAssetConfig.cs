using System.Collections.Generic;
using System.Xml.Serialization;
using WeaponConfigNs;

namespace XmlConfig
{
    public enum WeaponPartLocation
    {
        None =-1,
        Muzzle,    
        LowRail,
        Magazine,
        Buttstock,
        Scope,
        SideRail,
        EndOfTheWorld
    }

    public enum WeaponPartType
    {
        // 用于判断加载或卸载
        Null,
        Default,
        RedDot,
        HoloSight,
        Suppressor,
        FlashHider,
        Compensator,
        TwoTimesScope,
        FourTimesScope,
        EightTimesScope,
        QuickDrawMagazine,
        ExtendedMagazine,
        DualMagazine,
        Stock,
        Foregrip,
        TriangleGrip,
    }

    public class WeaponAssetItem
    {
        [XmlAttribute]
        public int Id;
        [XmlAttribute]
        public float SightDistance;

        // 第三人称模型
        public AssetInfo WeaponP3;
        // 第一人称模型
        public AssetInfo WeaponP1;
        // 第三人称动画资源
        public WeaponAnimation AnimationP3;
        // 第一人称动画资源
        public WeaponAnimation AnimationP1;
        // 左手握持动作需要用IK
        public bool LeftHandIK;
        // 枪的可用配件位置，可用配件类型，都可以放这儿
        public AttachmentTypeToId[] Attachments;
        public AssetInfo Icon;
    }

    public class WeaponAnimation
    {
        public AssetInfo Female;
        public AssetInfo Male;
    }

    public class AttachmentTypeToId
    {
        public WeaponPartType Type;
        public WeaponPartLocation Location;
        public int Id;
    }

    public class AttachmentAssetItem
    {
        [XmlAttribute]
        public int Id;

        public AssetInfo AssetAddress;
    }

    [XmlRoot]
    public class WeaponAssetConfig
    {
        public WeaponAssetItem[] Items;
        public AttachmentAssetItem[] Attachments;
        public WeaponAnimation EmptyHandedP3;
        public WeaponAnimation EmptyHandedP1;
    }

    [XmlType("child")]
    public class WeaponAvatarConfigItem : ItemBaseConfig
    {
        public string ModelBundle;
        public string ResP3;
        public string ResP1;
        public string AnimBundle;
        public string AnimFemaleP3;
        public string AnimFemaleP1;
        public string AnimMaleP3;
        public string AnimMaleP1;
        public bool LeftHandIK;
        public float SightDistance;
        public string KillIcon;
        public int ApplyWeaponsId;
        public int Renewal;
        public string StarReticleBundle;
        public List<string> StarReticle;
        public string EffectBundle;
        public List<string> SpecialEffect;
        public float Size;

        public string HallAnimBundle;
        public string HallAnimMale;
        public string HallAnimFemale;
    }

    [XmlRoot("root")]
    public class WeaponAvatarConfig
    {
        public WeaponAvatarConfigItem[] Items;
    }
}
