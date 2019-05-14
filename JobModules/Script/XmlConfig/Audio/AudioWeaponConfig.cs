
using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class AudioWeaponConfig
    {
        public AudioWeaponItem[] Items { get; private set; }
    }
    [XmlType("Weapon")]
    public class AudioWeaponItem
    {
        public int Id;
        public int Tid;
        public string Name;
        public int Fire;
    //    public int FireStop;
        public int ReloadStart;
     //   public int ReloadEnd;
//        public int SwitchIn;
//        public int SwitchOff;
      //  public int PullBolt;
        public int BulletDrop;
        public int BulletFly;
     //   public int SwitchFireMode;
//        public int OnShoulder;
//        public int OffShoulder;
        public int ClipDrop;
        //public int Silencer;
        public int Left1;
        public int Left2;
        public int Right;
        [XmlElement("HitList")]
        public HitList list;
    }
    public class HitList
    {
        public int Body;
        public int Armor;
        public int Concrete;
        public int Iron;
        public int Wood;
        public int Glass;
        public int Water;
        public int Rubber;
        public int Sand;

    }
} 