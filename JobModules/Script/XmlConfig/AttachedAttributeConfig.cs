using System.Collections.Generic;

namespace XmlConfig
{
    public enum PartModifyType
    {
        Replace,
        Add,
        Scale,
    }

    public enum WeaponAttributeType
    {
        /// <summary>
        /// 前置子弹数
        /// </summary>
        Bullet,
        /// <summary>
        /// 换弹时间
        /// </summary>
        ReloadSpeed,
        /// <summary>
        /// 左右偏移概率
        /// </summary>
        LateralTurnback,
        /// <summary>
        /// 左右基础宽度
        /// </summary>
        LateralBase,
        /// <summary>
        /// 最大宽度
        /// </summary>
        LateralMax,
        /// <summary>
        /// 连射时递增宽度
        /// </summary>
        LateralModifier,
        /// <summary>
        /// 基础高度
        /// </summary>
        UpBase,
        /// <summary>
        /// 最大高度
        /// </summary>
        UpMax,
        /// <summary>
        /// 连射时递增高度
        /// </summary>
        UpModifier,
        /// <summary>
        /// 消除枪口火花
        /// </summary>
        Spark,
        /// <summary>
        /// 瞄准时间
        /// </summary>
        FocusSpeed,
        /// <summary>
        /// 呼吸摇晃的幅度
        /// </summary>
        Breath,
        /// <summary>
        /// 消音
        /// </summary>
        FireSound,
        /// <summary>
        /// 基础精准
        /// </summary>
        Spread,
        /// <summary>
        /// 子弹初始速度
        /// </summary>
        Speed,
        /// <summary>
        /// 瞄准后的fov变化
        /// </summary>
        Fov,
        BaseDamage,
        EmitVelocity,
        AttackInterval,
        DistanceDecay,
        Length,
    }

    public struct AttachedAttributeItem
    {
        public WeaponAttributeType Type;
        public float Val;
    }

    public class AttachedAttributeSet
    {
        public readonly List<AttachedAttributeItem> list;
        
        int[] attributesGuide = new int[(int)WeaponAttributeType.Length];
        public AttachedAttributeSet()
        {
            list = new List<AttachedAttributeItem>();
        }

        public void AddAttribute(AttachedAttributeItem item)
        {
            list.Add(item);
            attributesGuide[(int)item.Type] = list.Count;
        }

        public float GetAttribute(WeaponAttributeType attributeType)
        {
            var length = attributesGuide[(int) attributeType];
            if (length > 0)
                return list[length - 1].Val;
            return -999f;
        }
    }

    public class AttachmentConfigItem
    {
        public int Id;
        public string Name;
        public int AssetId;
        public AttachedAttributeItem[] Attributes;
    }
}
