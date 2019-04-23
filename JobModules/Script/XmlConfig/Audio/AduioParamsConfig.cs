
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace XmlConfig
{
    [XmlRoot("root")]
    public class AudioParamConfig
    {
        public AudioParamItem[] Items { get; private set; }
    }
    [XmlType("item")]
    public class AudioParamItem
    {
        public int Id;
        public string Name;
        //public float Range;
        public int Type;

    }
    
}
