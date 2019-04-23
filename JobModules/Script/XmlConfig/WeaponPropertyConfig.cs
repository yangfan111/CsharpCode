using Assets.XmlConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using XmlConfig;

namespace XmlConfig
{
    [XmlType("child")]
    public class WeaponPropertyConfigItem
    {
        public int Id;
        public int WeaponId;
        public string Name;
        public int Bullet;  //装弹数
        public int Bulletmax; //最大备弹数
        public float Power;     //威力
        public float Limitcycle;  //射速
        public float Accurate;    //精准
        public float Stability;   //稳定
        public float Penetrate;   //穿透
        public float Scope;         //范围      

        public int Xlv;
        public int Type;
        public int SubType;
        public float Weight;
        public float Maneuverability;
        public float Aim;
        public float Rake;
        public float PartsslotCnt;
        public float PartsslotOpenCnt;
    }

    [XmlRoot("root")]
    public class WeaponPropertyConfig
    {
        public WeaponPropertyConfigItem[] Items;
    }   
}
