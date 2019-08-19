using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    /// <summary>
    /// 配件的类型，顺序和策划的配置一致
    /// </summary>
    public enum EWeaponPartType
    {
        None,
        /// <summary>
        /// 1弹匣
        /// </summary>
        Magazine=1,
        /// <summary>
        /// 2枪口：消焰器，补偿器，消音器
        /// </summary>
        Muzzle=2,
        /// <summary>
        /// 3导轨:瞄具
        /// </summary>
        UpperRail=3,
        /// <summary>
        /// 4侧导轨:S型激光辅助
        /// </summary>
        SideRail=4,
        /// <summary>
        /// 5握把 
        /// </summary>
        LowerRail=5,
        /// <summary>
        /// 6枪托
        /// </summary>
        Stock=6,
        /// <summary>
        /// 7枪膛
        /// </summary>
        Bore=7,
        /// <summary>
        /// 8
        /// </summary>
        Feed=8,
        /// <summary>
        /// 9
        /// </summary>
        Trigger=9,
        /// <summary>
        /// 10
        /// </summary>
        Interlock=10,
        /// <summary>
        /// 11
        /// </summary>
        Brake=11,
        Length,
    }

    [XmlType("child")]
    public class WeaponPartsConfigItem : ItemBaseConfig
    {
        public int Apply;    //需要废弃的字段(大厅中已经废弃！！！)  现在配件的限制在武器表的 ApplyParts//
        public string Res;
        public string Bundle;
        public int UnlockLv;
        public int GP;
        public int Gold;
        public int Bullet;//前置子弹数
        public float Weight;
        public float ReloadSpeed;//换弹时间
        public float LateralBase;//基础宽度
        public float LateralMax;//最大宽度
        public float LateralModifier;//连射时递增宽度
        public float UpBase;//基础高度
        public float UpMax;//最大高度
        public float UpModifier;//连射时递增高度
        public int Spark;//消除枪口火花
        public float FocusSpeed;//瞄准时间
        public float LateralTurnback;//左右偏移概率
        public float Breath;//呼吸摇晃的幅度
        public int FireSound;//消音
        public float Spread;//基础精准
        public float Speed;//子弹初始速度
        public float Fov;//瞄准后的fov变化
        public float AimOffset;//瞄准镜距离眼睛偏移
        public float Scale;//瞄准后镜头模型缩放
        public int PickSound;
        public int Default;
        public float FovMove;
        public int Workshop;
        public float BaseDamage;
        public float DistanceDecay;
        public float EmitVelocity;
        public float AttackInterval;
        public string DOFParameter; //景深
        public List<float> ShowRotation;  //展示角度
        public float ShowDistance;        //展示距离
        public string[] GetDOFParameters
               {
                   get
                   {
                       if (DOFParameterSplits == null)
                       {
                           DOFParameterSplits = DOFParameter.Split(',');
                       }
       
                       return DOFParameterSplits;
                   }
               }
               private string[] DOFParameterSplits; //景深
             
    }

    [XmlRoot("root")]
    public class WeaponPartsConfig
    {
        public WeaponPartsConfigItem[] Items;
    }
}
