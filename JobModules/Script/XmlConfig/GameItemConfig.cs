using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlConfig
{
    public enum GameItemType
    {
        Bullet = 1,
        Supplies = 2
    }


    [XmlRoot("root")]
    public class GameItemConfig
    {
        public GameItemConfigItem[] Items;
    }

    [XmlType("child")]
    public class GameItemConfigItem : ItemBaseConfig
    {
        public int Num;
        public string Sound;
        public string Bundle;
        public string Prefab;
        public int Sing;
        public int Stack;
        public float Weight;
        public int PickSound;
        public float Size;
    }
}
