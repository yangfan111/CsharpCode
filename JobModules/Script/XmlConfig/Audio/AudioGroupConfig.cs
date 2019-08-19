
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class AudioGroupConfig
    {
        public AudioGroupItem[] Items { get; private set; }
    }
    [XmlType("item")]
    public class AudioGroupItem
    {
        public int Id;
        public string Group;
        public uint ConvertedGroupId;
        public int GroupType;
        public List<string> States;
        public uint [] ConvertedStateIds;

        public string DefaultState;
        public List<int> TriggerType;
        public int selectedIndex = 0;
        public string []statesArr;
        public string[] StateArr
        {
            get { if (statesArr == null)
                    statesArr = States.ToArray();
                return statesArr;
            }
        }

    }
}